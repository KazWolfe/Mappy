using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;
using csFramework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Mappy.System;

public unsafe class MapManager : IDisposable
{
    public MapData MapData { get; } = new();
    public bool FollowPlayer => Service.Configuration.FollowPlayer.Value;

    private readonly List<IMapComponent> mapComponents = new()
    {
        new GatheringPointMapComponent(),
        new FateMapComponent(),
        new MapMarkersMapComponent(),
        new PlayerMapComponent(),
    };

    private AgentMap* MapAgent => csFramework.Instance()->GetUiModule()->GetAgentModule()->GetAgentMap();
    
    private string lastMapPath = string.Empty;

    public MapManager()
    {
        SignatureHelper.Initialise(this);

        Service.Framework.Update += OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51]) return;

        var pathString = MapAgent->CurrentMapPath.ToString();
        if (pathString == string.Empty) return;
            
        if (lastMapPath != pathString)
        {
            PluginLog.Debug($"Map Path Updated: {pathString}.tex");
            UpdateCurrentMap(pathString);
            lastMapPath = pathString;
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
        RefreshMapComponents();
    }

    private void RefreshMapComponents()
    {
        foreach (var component in mapComponents)
        {
            component.Refresh();
        }
    }

    public void DrawMap()
    {
        if (!MapData.DataAvailable) return;
        
        if (FollowPlayer && MapData.PlayerInCurrentMap())
        {
            CenterOnPlayer();
        }
            
        DrawMapImage();

        foreach (var mapComponent in mapComponents)
        {
            mapComponent.Draw();
        }

        if (Service.WindowManager.GetWindowOfType<MapWindow>(out var mapWindow) && mapWindow.IsFocused)
        {
            DrawMapLayers();
        }
    }
    
    private void DrawMapLayers()
    {
        var regionAvailable = ImGui.GetContentRegionAvail();
        ImGui.SetCursorPos(regionAvailable with {Y = 0, X = 0});

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.0f, 0.0f, 0.0f, 0.80f));        
        if (ImGui.BeginChild("###Toolbar", regionAvailable with { Y = 40.0f }, true))
        {
            LayerSelectionCombo();
            ImGui.SameLine();
            FollowPlayerButton();
            ImGui.SameLine();
            RecenterOnPlayer();
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
        
    }

    private void RecenterOnPlayer()
    {
        ImGui.PushID("CenterOnPlayer");
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Crosshairs.ToIconString(), new Vector2(23.0f)))
        {
            MapData.LoadMap(MapAgent->CurrentMapPath.ToString());
            if (Service.ClientState.LocalPlayer is { } player)
            {
                MapData.Viewport.Center = MapData.GetGameObjectPosition(player.Position);
            }
        }

        ImGui.PopFont();
        ImGui.PopID();
    }

    private void FollowPlayerButton()
    {
        ImGui.PushID("FollowPlayerButton");
        ImGui.PushFont(UiBuilder.IconFont);

        var followPlayer = Service.Configuration.FollowPlayer.Value;

        if (followPlayer) ImGui.PushStyleColor(ImGuiCol.Button, Colors.Red);
        if (ImGui.Button(FontAwesomeIcon.MapMarker.ToIconString(), new Vector2(23.0f)))
        {
            Service.Configuration.FollowPlayer.Value = !Service.Configuration.FollowPlayer.Value;
        }

        if (followPlayer) ImGui.PopStyleColor();

        ImGui.PopFont();
        ImGui.PopID();
    }

    private void LayerSelectionCombo()
    {
        ImGui.PushItemWidth(250.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginCombo("###LayerCombo", MapData.GetCurrentMapName()))
        {
            if (MapData.MapLayers.Count == 1)
            {
                ImGui.TextColored(Colors.Orange, Strings.Map.NoLayers);
            }
            else
            {
                foreach (var layer in MapData.MapLayers)
                {
                    if (GetLayerSubName(layer, out var layerSubName))
                    {
                        if (ImGui.Selectable(layerSubName))
                        {
                            MapData.SelectMapLayer(layer);
                            RefreshMapComponents();
                        }
                    }
                }
            }
            ImGui.EndCombo();
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
                ImGui.Image(MapData.Texture.ImGuiHandle, textureSize,Vector2.Zero, Vector2.One, Vector4.One with { W = fadePercent });
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