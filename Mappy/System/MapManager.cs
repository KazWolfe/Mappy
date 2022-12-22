using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;
using csFramework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Mappy.System;

public unsafe class MapManager : IDisposable
{
    private AgentMap* MapAgent => csFramework.Instance()->GetUiModule()->GetAgentModule()->GetAgentMap();
    private TextureWrap? lastTexture;
    public TextureWrap? MapTexture
    {
        get
        {
            if (Service.Cache.MapTextureCache.GetMapTexture(LoadedMapId) is { } mapTexture)
            {
                lastTexture = mapTexture;
                LoadingNextMap = false;
                return mapTexture;
            }

            LoadingNextMap = true;
            return lastTexture;
        }
    }

    public Vector2 MapTextureSize => new(MapTexture?.Width ?? 2048, MapTexture?.Height ?? 2048);
    public List<Map> MapLayers { get; private set; } = new();
    public Map? Map { get; private set; }
    public bool PlayerInCurrentMap => MapAgent->CurrentMapId == LoadedMapId;
    public uint PlayerLocationMapID => MapAgent->CurrentMapId;
    public bool LoadingNextMap { get; private set; }
    public uint LoadedMapId { get; private set; }

    private uint lastMapId;
    private bool loadInProgress;
    private readonly Dictionary<uint, ViewportData> viewportPosition = new();

    public List<IMapComponent> MapComponents { get; }
    
    public MapManager()
    {
        MapComponents = Service.ModuleManager.GetMapComponents().ToList();
        
        Service.Framework.Update += OnFrameworkUpdate;
    }
    
    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
    }
    
    private void OnFrameworkUpdate(Framework framework)
    {
        if (MapAgent is null) return;
        if (Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51]) return;

        var currentMapId = MapAgent->CurrentMapId;
        if (lastMapId != currentMapId)
        {
            PluginLog.Debug($"Map ID Updated: {currentMapId}");
            LoadMap(MapAgent->CurrentMapId);

            lastMapId = currentMapId;
        }

        if (Service.Configuration.FollowPlayer.Value)
        {
            CenterOnPlayer();
        }
    }

    public Vector2 GetObjectPosition(GameObject gameObject) => GetObjectPosition(gameObject.Position);
    
    public Vector2 GetObjectPosition(Vector3 position) => GetObjectPosition(new Vector2(position.X, position.Z));

    public Vector2 GetObjectPosition(Vector2 position)
    {
        return position * ((Map?.SizeFactor ?? 100.0f) / 100.0f)
               + new Vector2(Map?.OffsetX ?? 0, Map?.OffsetY ?? 0) * ((Map?.SizeFactor ?? 100.0f) / 100.0f)
               + MapTextureSize / 2.0f;
    }
    
    public Vector2 GetTextureOffsetPosition(Vector2 coordinates) =>
        coordinates * ((Map?.SizeFactor ?? 100.0f) / 100.0f)
        + new Vector2(Map?.OffsetX ?? 0, Map?.OffsetY ?? 0) * ((Map?.SizeFactor ?? 100.0f) / 100.0f)
        + MapTextureSize / 2.0f;
    
    public Vector2 GetTexturePosition(Vector2 coordinates)
    {
        return (coordinates / MapRenderer.Viewport.Scale
                - MapRenderer.Viewport.Size / 2.0f / MapRenderer.Viewport.Scale
                + MapRenderer.Viewport.Center - MapTextureSize / 2.0f)
                / ((Map?.SizeFactor ?? 100.0f) / 100.0f)
                - new Vector2(Map?.OffsetX ?? 0, Map?.OffsetY ?? 0);
    }
    
    public void LoadMap(uint mapId, Vector2? newViewportPosition = null)
    {
        if (!loadInProgress)
        {
            loadInProgress = true;
            Task.Run(() => InternalLoadMap(mapId, newViewportPosition));
        }
    }

    private void InternalLoadMap(uint mapID, Vector2? newViewportPosition)
    {
        if (LoadedMapId == mapID || mapID == uint.MaxValue)
        {
            loadInProgress = false;
            SetViewport(mapID, newViewportPosition);
            return;
        }
        
        PluginLog.Debug($"Loading Map: {mapID}");

        viewportPosition[LoadedMapId] = new ViewportData(MapRenderer.Viewport.Center, MapRenderer.Viewport.Scale);
        
        LoadedMapId = mapID;

        Map = Service.Cache.MapCache.GetRow(mapID);

        MapLayers = Service.DataManager.GetExcelSheet<Map>()!
            .Where(eachMap => eachMap.PlaceName.Row == Map.PlaceName.Row)
            .Where(eachMap => eachMap.MapIndex != 0)
            .OrderBy(eachMap => eachMap.MapIndex)
            .ToList();
            
        MapComponents.ForEach(component => component.Update(mapID));
        SetViewport(mapID, newViewportPosition);

        loadInProgress = false;
    }

    private void SetViewport(uint mapID, Vector2? newViewportPosition)
    {
        if (newViewportPosition is {} newPosition)
        {
            Service.Configuration.FollowPlayer.Value = false;
            MapRenderer.SetViewportCenter(GetTextureOffsetPosition(newPosition));
            MapRenderer.SetViewportZoom(0.8f);
        }
        else
        {
            viewportPosition.TryGetValue(mapID, out var viewportData);
            
            switch (PlayerInCurrentMap)
            {
                // Player isn't in current map, and we haven't been here before, center viewport
                default: 
                    MapRenderer.SetViewportCenter(MapTextureSize / 2.0f);
                    MapRenderer.SetViewportZoom(0.4f);
                    break;
                
                // Player isn't in current map, and we have been here before, load saved viewport
                case false when viewportData is not null:
                    MapRenderer.SetViewportCenter(viewportData.Center);
                    MapRenderer.SetViewportZoom(viewportData.Scale);
                    break;
                
                // Player is in current map, go to player
                case true:
                    CenterOnPlayer();
                    break;
            }
        }
    }

    public static void MoveMapToPlayer()
    {
        if (!Service.MapManager.PlayerInCurrentMap)
        {
            Service.MapManager.LoadMap(Service.MapManager.PlayerLocationMapID);
        }
    }
    
    public static void CenterOnPlayer()
    {
        if (Service.ClientState.LocalPlayer is { } player && Service.MapManager.PlayerInCurrentMap)
        {
            MapRenderer.SetViewportCenter(Service.MapManager.GetObjectPosition(player));
        }
    }
}