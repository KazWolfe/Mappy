using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using Mappy.Localization;
using Mappy.System;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.UserInterface.Components;

public class MapToolbar
{
    public MapSelectOverlay MapSelect { get; }= new();

    public void Draw(bool isWindowFocused)
    {
        if (!isWindowFocused) MapSelect.ShowMapSelectOverlay = false;
        if (isWindowFocused || Service.Configuration.AlwaysShowToolbar.Value)
        {
            var regionAvailable = ImGui.GetContentRegionAvail();
            
            ImGui.SetCursorPos(Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.0f, 0.0f, 0.0f, 0.80f));        
            if (ImGui.BeginChild("###Toolbar", regionAvailable with { Y = 40.0f }, true))
            {
                DrawMapLayersWidget();
                ImGui.SameLine();
                DrawFollowPlayerWidget();
                ImGui.SameLine();
                DrawRecenterOnPlayerWidget();
                ImGui.SameLine();
                MapSelect.DrawWidget();
                ImGui.SameLine();
                DrawConfigurationButton();
            }
            ImGui.EndChild();
            ImGui.PopStyleColor();
            
            MapSelect.Draw();
        }
    }

    private void DrawMapLayersWidget()
    {
        ImGui.PushItemWidth(250.0f * ImGuiHelpers.GlobalScale);
        ImGui.BeginDisabled(Service.MapManager.MapLayers.Count == 0);
        if (ImGui.BeginCombo("###LayerCombo", Service.MapManager.Map?.GetName() ?? "Unable To Get Map Data"))
        {
            foreach (var layer in Service.MapManager.MapLayers)
            {
                var subAreaName = layer.GetSubName();
                    
                if(subAreaName == string.Empty) continue;

                if (ImGui.Selectable(subAreaName))
                {
                    Service.Configuration.FollowPlayer.Value = false;
                    Service.MapManager.LoadMap(layer.RowId);
                }
            }
            ImGui.EndCombo();
        }
        ImGui.EndDisabled();
    }

    private void DrawFollowPlayerWidget()
    {
        ImGui.PushID("FollowPlayerButton");
        ImGui.PushFont(UiBuilder.IconFont);

        var followPlayer = Service.Configuration.FollowPlayer.Value;

        if (followPlayer) ImGui.PushStyleColor(ImGuiCol.Button, Colors.Red);
        if (ImGui.Button(FontAwesomeIcon.MapMarkerAlt.ToIconString(), new Vector2(23.0f)))
        {
            Service.Configuration.FollowPlayer.Value = !Service.Configuration.FollowPlayer.Value;
        }
        if (followPlayer) ImGui.PopStyleColor();

        ImGui.PopFont();
        
        if (ImGui.IsItemHovered())
        {
            Utilities.Draw.DrawTooltip(Strings.Map.FollowPlayer, Colors.White);
        }

        ImGui.PopID();
    }

    private void DrawRecenterOnPlayerWidget()
    {
        ImGui.PushID("CenterOnPlayer");
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Crosshairs.ToIconString(), new Vector2(23.0f)))
        {
            MapManager.CenterOnPlayer();
        }

        ImGui.PopFont();
        
        if (ImGui.IsItemHovered())
        {
            Utilities.Draw.DrawTooltip(Strings.Map.CenterOnPlayer, Colors.White);
        }

        ImGui.PopID();
    }

    private void DrawConfigurationButton()
    {
        ImGui.PushID("ConfigurationButton");
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Cog.ToIconString(), new Vector2(25.0f, 23.0f)))
        {
            if (Service.WindowManager.GetWindowOfType<ConfigurationWindow>(out var configurationWindow))
            {
                configurationWindow.IsOpen = !configurationWindow.IsOpen;
                configurationWindow.Collapsed = false;
            }
        }

        ImGui.PopFont();
                
        if (ImGui.IsItemHovered())
        {
            Utilities.Draw.DrawTooltip(Strings.Map.Settings, Colors.White);
        }
        
        ImGui.PopID();
    }
}