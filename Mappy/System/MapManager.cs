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
using Mappy.Interfaces;
using Mappy.MapComponents;
using Mappy.Utilities;
using csFramework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Mappy.System;

public unsafe class MapManager : IDisposable
{
    private AgentMap* MapAgent => csFramework.Instance()->GetUiModule()->GetAgentModule()->GetAgentMap();
    public TextureWrap? MapTexture;
    public List<Map> MapLayers { get; private set; } = new();
    public Map? Map;
    public bool PlayerInCurrentMap => MapAgent->CurrentMapId == loadedMapId;
    public uint PlayerLocationMapID => MapAgent->CurrentMapId;
    
    private uint loadedMapId;
    private uint lastMapId;

    public List<IMapComponent> MapComponents { get; } = new()
    {
        new MapMarkersMapComponent(),
        new GatheringPointMapComponent(),
        new FateMapComponent(),
        
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
            LoadSelectedMap(MapAgent->CurrentMapId);
            lastMapId = currentMapId;
        }

        if (Service.Configuration.FollowPlayer.Value)
        {
            CenterOnPlayer();
        }
    }

    public void LoadSelectedMap(uint mapID)
    {
        LoadMap(mapID);
    }

    public Vector2 GetObjectPosition(GameObject gameObject) => GetObjectPosition(gameObject.Position);
    public Vector2 GetObjectPosition(Vector3 position)
    {
        return new Vector2(position.X, position.Z) * MapAgent->CurrentMapSizeFactorFloat
               - new Vector2(MapAgent->CurrentOffsetX, MapAgent->CurrentOffsetY) * MapAgent->CurrentMapSizeFactorFloat
               + new Vector2(MapTexture?.Width ?? 0, MapTexture?.Height ?? 0) / 2.0f;
    }
    
    private void LoadMap(uint mapId)
    {
        Task.Run(() => 
        {
            try
            {
                var lastMapTexture = MapTexture;
                
                Map = Service.Cache.MapCache.GetRow(mapId);
                var path = GetPathFromMap(Map);
                var tex = Service.DataManager.GetImGuiTexture(path);
                
                loadedMapId = mapId;

                MapLayers = Service.DataManager.GetExcelSheet<Map>()!
                    .Where(eachMap => eachMap.TerritoryType.Row == Map.TerritoryType.Row)
                    .Where(eachMap => !eachMap.IsEvent)
                    .ToList();

                LogMapLayers();

                if (tex is not null && tex.ImGuiHandle != IntPtr.Zero)
                {
                    MapTexture = tex;
                    lastMapTexture?.Dispose();
                    MapComponents.ForEach(component => component.Update(mapId));
                    
                    PluginLog.Debug($"Player here? {PlayerInCurrentMap}");
                    
                    if (!Service.Configuration.FollowPlayer.Value && !PlayerInCurrentMap)
                    {
                        var newCenter = new Vector2(MapTexture.Width, MapTexture.Height) / 2.0f;
                        MapRenderer.SetViewportCenter(newCenter);
                    }
                    else if(PlayerInCurrentMap)
                    {
                        CenterOnPlayer();
                    }
                } 
                else 
                {
                    tex?.Dispose();
                }
            } 
            catch (Exception ex) 
            {
                PluginLog.LogError($"Failed loading texture for map {mapId} - {ex.Message}");
            }
        });
    }
    
    private string GetPathFromMap(Map map)
    {
        var mapKey = map.Id.RawString;
        var rawKey = mapKey.Replace("/", "");
        return $"ui/map/{mapKey}/{rawKey}_m.tex";
    }

    private void LogMapLayers()
    {
        var message = $"Loaded {MapLayers.Count} Map Layers\n";

        message = MapLayers.Aggregate(message, (current, layer) => current + $"{layer.PlaceNameSub.Value?.Name.RawString ?? "NameString Was Null"}\n");

        PluginLog.Debug(message);
    }

    public void CenterOnPlayer()
    {
        if (!Service.MapManager.PlayerInCurrentMap)
        {
            Service.MapManager.LoadSelectedMap(Service.MapManager.PlayerLocationMapID);
        }
            
        if (Service.ClientState.LocalPlayer is { } player)
        {
            MapRenderer.SetViewportCenter(Service.MapManager.GetObjectPosition(player));
        }
    }
}