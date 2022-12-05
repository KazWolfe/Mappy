using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Mappy.Localization;
using Mappy.Utilities;
using Condition = Mappy.Utilities.Condition;

namespace Mappy.UserInterface.Windows;

public class MapWindow : Window, IDisposable
{
    private Vector2 mouseDragStart;
    private bool dragStarted;
    private Vector2 lastWindowSize = Vector2.Zero;

    private const ImGuiWindowFlags AlwaysActiveFlags =
        ImGuiWindowFlags.NoFocusOnAppearing |
        ImGuiWindowFlags.NoNav |
        ImGuiWindowFlags.NoBringToFrontOnFocus |
        ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse;

    private const ImGuiWindowFlags NoDecorationFlags =
        ImGuiWindowFlags.NoDecoration |
        ImGuiWindowFlags.NoBackground;

    private const ImGuiWindowFlags NoMoveResizeFlags =
        ImGuiWindowFlags.NoMove |
        ImGuiWindowFlags.NoResize;

    public MapWindow() : base("Mappy Map Window")
    {
        IsOpen = Service.Configuration.KeepOpen.Value;
    }

    public void Dispose()
    {
    }

    public override void PreOpenCheck()
    {
        if (Service.Configuration.KeepOpen.Value) IsOpen = true;
        
        if (Service.ClientState.IsPvP) IsOpen = false;
        if (!Service.ClientState.IsLoggedIn) IsOpen = false;
        if (Condition.BetweenAreas()) IsOpen = false;
        if (Service.Configuration.HideInDuties.Value && Condition.IsBoundByDuty()) IsOpen = false; 
    }
    
    public override void Draw()
    {
        SetFlags();
        ReadMouse();

        if (ImGui.BeginChild("###MapFrame", ImGui.GetContentRegionAvail(), false, Flags))
        {
            var shouldFadeMap = Service.Configuration.FadeWhenUnfocused.Value && !IsFocused;
            MapRenderer.DrawMap(shouldFadeMap);

            foreach (var component in Service.MapManager.MapComponents)
            {
                component.Draw();
            }

            if (IsFocused)
            {
                DrawToolbar();
            }
        }
        ImGui.EndChild();
    }

    private void DrawToolbar()
    {
        var regionAvailable = ImGui.GetContentRegionAvail();
        ImGui.SetCursorPos(regionAvailable with {Y = 0, X = 0});

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.0f, 0.0f, 0.0f, 0.80f));        
        if (ImGui.BeginChild("###Toolbar", regionAvailable with { Y = 40.0f }, true))
        {
            DrawMapLayersWidget();
            ImGui.SameLine();
            DrawFollowPlayerWidget();
            ImGui.SameLine();
            DrawRecenterOnPlayerWidget();
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    private void DrawMapLayersWidget()
    {
        ImGui.PushItemWidth(250.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginCombo("###LayerCombo", Service.MapManager.Map?.GetName() ?? "Unable To Get Map Data"))
        {
            var mapLayers = Service.MapManager.MapLayers;
            
            if (mapLayers.Count == 1)
            {
                ImGui.TextColored(Colors.Orange, Strings.Map.NoLayers);
            }
            else
            {
                foreach (var layer in mapLayers)
                {
                    var subAreaName = layer.GetSubName();
                    
                    if(subAreaName == string.Empty) continue;

                    if (ImGui.Selectable(subAreaName))
                    {
                        Service.MapManager.LoadSelectedMap(layer.RowId);
                    }
                }
            }
            ImGui.EndCombo();
        }
    }

    private void DrawFollowPlayerWidget()
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

    private void DrawRecenterOnPlayerWidget()
    {
        ImGui.PushID("CenterOnPlayer");
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Crosshairs.ToIconString(), new Vector2(23.0f)))
        {
            Service.MapManager.CenterOnPlayer();
        }

        ImGui.PopFont();
        ImGui.PopID();
    }

    private void ReadMouse()
    {
        if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow) || IsFocused)
        {
            if (IsCursorInWindow() && !IsCursorInWindowHeader())
            {
                if (ImGui.GetIO().MouseWheel > 0) // Mouse Wheel Up
                {
                    MapRenderer.ZoomIn(0.2f);
                }
                else if (ImGui.GetIO().MouseWheel < 0) // Mouse Wheel Down
                {
                    MapRenderer.ZoomOut(0.2f);
                }

                // Don't allow a drag to start if the window size is changing
                if (ImGui.GetWindowSize() == lastWindowSize)
                {
                    if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && !dragStarted)
                    {
                        mouseDragStart = ImGui.GetMousePos();
                        dragStarted = true;
                    }
                }
                else
                {
                    lastWindowSize = ImGui.GetWindowSize();
                    dragStarted = false;
                }
            }

            if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && dragStarted)
            {
                var delta = mouseDragStart - ImGui.GetMousePos();
                MapRenderer.MoveViewportCenter(delta);
                mouseDragStart = ImGui.GetMousePos();
            }
            else
            {
                dragStarted = false;
            }

            if (dragStarted)
            {
                Service.Configuration.FollowPlayer.Value = false;
            }
        }
    }
    private void SetFlags()
    {
        Flags = AlwaysActiveFlags;

        if (Service.Configuration.LockWindow.Value) Flags |= NoMoveResizeFlags;
        if (Service.Configuration.HideWindowFrame.Value) Flags |= NoDecorationFlags;
        if (!IsCursorInWindowHeader()) Flags |= ImGuiWindowFlags.NoMove;
    }
    private static bool IsCursorInWindow()
    {
        var windowStart = ImGui.GetWindowPos();
        var windowSize = ImGui.GetWindowSize();

        return IsBoundedBy(ImGui.GetMousePos(), windowStart, windowStart + windowSize);
    }
    private static bool IsCursorInWindowHeader()
    {
        var windowStart = ImGui.GetWindowPos();
        var headerSize = ImGui.GetWindowSize() with { Y = ImGui.GetWindowContentRegionMin().Y };
        
        return IsBoundedBy(ImGui.GetMousePos(), windowStart, windowStart + headerSize);
    }
    private static bool IsBoundedBy(Vector2 cursor, Vector2 minBounds, Vector2 maxBounds)
    {
        if (cursor.X >= minBounds.X && cursor.Y >= minBounds.Y)
        {
            if (cursor.X <= maxBounds.X && cursor.Y <= maxBounds.Y)
            {
                return true;
            }
        }

        return false;
    }
}