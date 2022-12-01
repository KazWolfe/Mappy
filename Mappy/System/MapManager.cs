using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.MapComponents;
using Mappy.UserInterface.Windows;

namespace Mappy.System;

public unsafe class MapManager : IDisposable
{
    public MapData MapData { get; } = new();
    public bool FollowPlayer => Service.Configuration.FollowPlayer.Value;

    private readonly PlayerMapComponent player = new();
    private readonly GatheringPointMapComponent gatheringPoints = new();
    private readonly MapMarkersMapComponent mapMarkers = new();
    
    [Signature("48 8D 15 ?? ?? ?? ?? 48 83 C1 08 44 8B C7", ScanType = ScanType.StaticAddress)]
    private readonly byte* mapPath = null!;
    
    private string lastMapPath = string.Empty;

    public MapManager()
    {
        SignatureHelper.Initialise(this);

        Service.Framework.Update += OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51]) return;

        if (mapPath is not null)
        {
            var pathString = Encoding.UTF8.GetString(mapPath + 0x1011CB, 27).Trim('\0');

            if (pathString == string.Empty) return;
            
            if (lastMapPath != pathString)
            {
                PluginLog.Debug($"Map Path Updated: {pathString}");
                UpdateCurrentMap(pathString);
                lastMapPath = pathString;
            }
        }
    }

    public void Dispose()
    {
        MapData.Dispose();
        Service.Framework.Update -= OnFrameworkUpdate;
    }
    
    private void UpdateCurrentMap(string mapTexturePath)
    {
        MapData.LoadMap(mapTexturePath);
        mapMarkers.LoadMarkers();
    }

    public void DrawMap()
    {
        if (!MapData.DataAvailable) return;
        
        if (FollowPlayer && MapData.PlayerInCurrentMap())
        {
            CenterOnPlayer();
        }
            
        DrawMapImage();

        mapMarkers.Draw();
        gatheringPoints.Draw();
        player.Draw();
        
        DrawMapLayers();
    }
    
    private void DrawMapLayers()
    {
        var regionSize = ImGuiHelpers.ScaledVector2(175.0f, 150.0f);
        var regionAvailable = ImGui.GetContentRegionAvail();
        ImGui.SetCursorPos(regionAvailable with {Y = 0, X = 0});
        
        ImGui.PushItemWidth(250.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginCombo("###LayerCombo", MapData.GetCurrentMapName()))
        {
            foreach (var layer in MapData.MapLayers)
            {
                if (GetLayerSubName(layer, out var layerSubName))
                {
                    if (ImGui.Selectable(layerSubName))
                    {
                        MapData.SelectMapLayer(layer);
                        mapMarkers.LoadMarkers();
                    }
                }
            }
        }
    }

    private void CenterOnPlayer()
    {
        if (!MapData.DataAvailable) return;
        
        if (Service.ClientState.LocalPlayer is { } localPlayer)
        {
            var playerPosition = MapData.GetGameObjectPosition(localPlayer.Position);
            
            MapData.Viewport.Center = playerPosition;
        }
    }
    
    private void DrawMapImage()
    {
        if (!MapData.DataAvailable) return;
        
        var textureSize = MapData.GetScaledMapTextureSize();
        
        MapData.SetDrawPosition();

        if (Service.WindowManager.GetWindowOfType<MapWindow>(out var mapWindow))
        {
            if (Service.Configuration.FadeWhenUnfocused.Value && !mapWindow.IsFocused)
            {
                var fadePercent = 1.0f - Service.Configuration.FadePercent.Value;
                ImGui.Image(MapData.Texture.ImGuiHandle, textureSize,Vector2.Zero, Vector2.One, new Vector4(1.0f, 1.0f, 1.0f, fadePercent));
            }
            else
            {
                ImGui.Image(MapData.Texture.ImGuiHandle, textureSize);
            }
        }
    }

    private bool GetLayerSubName(Map map, [NotNullWhen(true)] out string? subName)
    {
        subName = map.PlaceNameSub.Value?.Name.RawString;

        return !string.IsNullOrEmpty(subName);
    }
}