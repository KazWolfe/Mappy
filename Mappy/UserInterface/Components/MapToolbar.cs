using System.Numerics;
using Dalamud.Interface;
using Dalamud.Utility;
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
            if (ImGui.BeginChild("###Toolbar", regionAvailable with { Y = 40.0f * ImGuiHelpers.GlobalScale }, true))
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
                ImGui.SameLine();
                DrawLockUnlockWidget();
                ImGui.SameLine();
                DrawCursorPosition();
            }
            ImGui.EndChild();
            ImGui.PopStyleColor();
            
            MapSelect.Draw();
        }
    }

    private void DrawMapLayersWidget()
    {
        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        ImGui.BeginDisabled(Service.MapManager.MapLayers.Count == 0);
        if (ImGui.BeginCombo("###LayerCombo", Service.MapManager.Map?.GetName() ?? "Unable To Get Map Data"))
        {
            foreach (var layer in Service.MapManager.MapLayers)
            {
                var subAreaName = layer.GetSubName();
                    
                if(subAreaName == string.Empty) continue;

                if (ImGui.Selectable(subAreaName))
                {
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
        if (ImGui.Button(FontAwesomeIcon.MapMarkerAlt.ToIconString(), ImGuiHelpers.ScaledVector2(23.0f)))
        {
            MapManager.MoveMapToPlayer();
            Service.Configuration.FollowPlayer.Value = !Service.Configuration.FollowPlayer.Value;
            Service.Configuration.Save();
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

        if (ImGui.Button(FontAwesomeIcon.Crosshairs.ToIconString(), ImGuiHelpers.ScaledVector2(23.0f)))
        {
            MapManager.MoveMapToPlayer();
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

        if (ImGui.Button(FontAwesomeIcon.Cog.ToIconString(), ImGuiHelpers.ScaledVector2(25.0f, 23.0f)))
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


    private void DrawLockUnlockWidget()
    {
        ImGui.PushID("LockUnlockWidget");

        if (Service.Configuration.HideWindowFrame.Value)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushID("OpenLock");
            if (ImGui.Button(FontAwesomeIcon.Unlock.ToIconString(), ImGuiHelpers.ScaledVector2(25.0f, 23.0f)))
            {
                Service.Configuration.HideWindowFrame.Value = false;
                Service.Configuration.LockWindow.Value = false;
                Service.Configuration.Save();
            }
            ImGui.PopFont();
            
            if (ImGui.IsItemHovered())
            {
                Utilities.Draw.DrawTooltip(Strings.Configuration.ShowAndUnlock, Colors.White);
            }
            ImGui.PopID();
        }
        else
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.PushID("ClosedLock");
            if (ImGui.Button(FontAwesomeIcon.Lock.ToIconString(), ImGuiHelpers.ScaledVector2(25.0f, 23.0f)))
            {
                Service.Configuration.HideWindowFrame.Value = true;
                Service.Configuration.LockWindow.Value = true;
                Service.Configuration.Save();
            }
            ImGui.PopFont();
            
            if (ImGui.IsItemHovered())
            {
                Utilities.Draw.DrawTooltip(Strings.Configuration.HideAndLock, Colors.White);
            }
            ImGui.PopID();
        }
        
        ImGui.PopID();
    }

    private void DrawCursorPosition()
    {
        if (MapSelect.ShowMapSelectOverlay) return;
        
        var cursorScreenPosition = ImGui.GetMousePos();

        if (MapWindow.IsBoundedBy(cursorScreenPosition, MapWindow.MapContentsStart, MapWindow.MapContentsStart + MapRenderer.Viewport.Size))
        {
            var cursorPosition = Service.MapManager.GetTexturePosition(ImGui.GetMousePos() - MapWindow.MapContentsStart);

            if (Service.MapManager.Map is not null)
            {
                var mapCoordinates = MapUtil.WorldToMap(cursorPosition, Service.MapManager.Map);

                var regionAvailable = ImGui.GetContentRegionMax();
                var coordinateString = $"( {mapCoordinates.X:F1}, {mapCoordinates.Y:F1} )";
                var stringSize = ImGui.CalcTextSize(coordinateString);

                var currentPosition = ImGui.GetCursorPos();
                ImGui.SetCursorPos(regionAvailable with {X = regionAvailable.X - stringSize.X, Y = currentPosition.Y});
                ImGui.Text(coordinateString);
            }
        }
    }
}