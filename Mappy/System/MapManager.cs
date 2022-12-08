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
            if (Service.Cache.MapTextureCache.GetMapTexture(loadedMapId) is { } mapTexture)
            {
                lastTexture = mapTexture;
                LoadingNextMap = false;
                return mapTexture;
            }

            LoadingNextMap = true;
            return lastTexture;
        }
    }

    private TextureWrap? lastTexture;
    public List<Map> MapLayers { get; private set; } = new();
    public Map? Map;
    public bool PlayerInCurrentMap => MapAgent->CurrentMapId == loadedMapId;
    public uint PlayerLocationMapID => MapAgent->CurrentMapId;

    public bool LoadingNextMap { get; private set; }

    private uint loadedMapId;

    private uint lastMapId;

    public List<IMapComponent> MapComponents { get; } = new()
    {
        new MapMarkersMapComponent(),
        new GatheringPointMapComponent(),
        new FateMapComponent(),
        new AllianceMemberMapComponent(),
        new PetMapComponent(),
        new PartyMapComponent(),
        new WaymarkMapComponent(),
        
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
    public Vector2 GetObjectPosition(Vector3 position)
    {
        return new Vector2(position.X, position.Z) * MapAgent->CurrentMapSizeFactorFloat
               - new Vector2(MapAgent->CurrentOffsetX, MapAgent->CurrentOffsetY) * MapAgent->CurrentMapSizeFactorFloat
               + new Vector2(MapTexture?.Width ?? 0, MapTexture?.Height ?? 0) / 2.0f;
    }
    
    public void LoadMap(uint mapId)
    {
        loadedMapId = mapId;
        
        Map = Service.Cache.MapCache.GetRow(mapId);

        MapLayers = Service.DataManager.GetExcelSheet<Map>()!
            .Where(eachMap => eachMap.TerritoryType.Row == Map.TerritoryType.Row)
            .Where(eachMap => eachMap.MapIndex != 0)
            .ToList();
            
        MapComponents.ForEach(component => component.Update(mapId));
                    
        if (!Service.Configuration.FollowPlayer.Value && !PlayerInCurrentMap && MapTexture is not null)
        {
            var newCenter = new Vector2(MapTexture.Width, MapTexture.Height) / 2.0f;
            MapRenderer.SetViewportCenter(newCenter);
            MapRenderer.SetViewportZoom(0.4f);
        }
        else if (PlayerInCurrentMap)
        {
            if (Service.ClientState.LocalPlayer is { } player)
            {
                MapRenderer.SetViewportCenter(Service.MapManager.GetObjectPosition(player));
            }
        }
    }
    
    public static void CenterOnPlayer()
    {
        if (!Service.MapManager.PlayerInCurrentMap)
        {
            Service.MapManager.LoadMap(Service.MapManager.PlayerLocationMapID);
        }
            
        if (Service.ClientState.LocalPlayer is { } player)
        {
            MapRenderer.SetViewportCenter(Service.MapManager.GetObjectPosition(player));
        }
    }
}