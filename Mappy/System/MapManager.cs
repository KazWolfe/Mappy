using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.Interfaces;
using Mappy.MapComponents;
using Mappy.Utilities;
using csFramework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Mappy.System;

public unsafe class MapManager : IDisposable
{
    private AgentMap* MapAgent => csFramework.Instance()->GetUiModule()->GetAgentModule()->GetAgentMap();
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

    public Vector2 MapTextureSize => new(MapTexture?.Width ?? 0, MapTexture?.Height ?? 0);
    
    private TextureWrap? lastTexture;
    public List<Map> MapLayers { get; private set; } = new();
    public Map? Map;
    public bool PlayerInCurrentMap => MapAgent->CurrentMapId == LoadedMapId;
    public uint PlayerLocationMapID => MapAgent->CurrentMapId;

    public bool LoadingNextMap { get; private set; }

    public uint LoadedMapId { get; private set; }

    private uint lastMapId;

    public List<IMapComponent> MapComponents { get; } = new()
    {
        new FateMapComponent(),
        new MapMarkersMapComponent(),
        new GatheringPointMapComponent(),
        new QuestMapComponent(),
        new AllianceMemberMapComponent(),
        new PetMapComponent(),
        new PartyMemberMapComponent(),
        new WaymarkMapComponent(),
        new TemporaryMarkersMapComponent(),

        new PlayerMapComponent(), // Render the player last
    };
    
    public MapManager()
    {
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
        return position * MapAgent->CurrentMapSizeFactorFloat
               - new Vector2(MapAgent->CurrentOffsetX, MapAgent->CurrentOffsetY) * MapAgent->CurrentMapSizeFactorFloat
               + MapTextureSize / 2.0f;
    }
    
    public Vector2 GetTextureOffsetPosition(Vector2 coordinates) =>
        coordinates * ((Map?.SizeFactor ?? 100.0f) / 100.0f)
        + MapTextureSize / 2.0f;
    
    public Vector2 GetTexturePosition(Vector2 coordinates)
    {
        return (coordinates / MapRenderer.Viewport.Scale
            - MapRenderer.Viewport.Size / 2.0f / MapRenderer.Viewport.Scale
            + MapRenderer.Viewport.Center - MapTextureSize / 2.0f) 
            / ((Map?.SizeFactor ?? 100.0f) / 100.0f);
    }
    
    public void LoadMap(uint mapId)
    {
        if (LoadedMapId == mapId) return;
        
        PluginLog.Debug($"Loading Map: {mapId}");
        
        LoadedMapId = mapId;
        
        Map = Service.Cache.MapCache.GetRow(mapId);

        MapLayers = Service.DataManager.GetExcelSheet<Map>()!
            .Where(eachMap => eachMap.PlaceName.Row == Map.PlaceName.Row)
            .Where(eachMap => eachMap.MapIndex != 0)
            .ToList();
            
        MapComponents.ForEach(component => component.Update(mapId));
                    
        if (!PlayerInCurrentMap && MapTexture is not null)
        {
            if (Service.Configuration.FollowPlayer.Value)
            {
                if (Service.ClientState.LocalPlayer is { } player)
                {
                    MapRenderer.SetViewportCenter(Service.MapManager.GetObjectPosition(player));
                }
            }
            else
            {
                var newCenter = MapTextureSize / 2.0f;
                MapRenderer.SetViewportCenter(newCenter);
                MapRenderer.SetViewportZoom(0.4f);
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