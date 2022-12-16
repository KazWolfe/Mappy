using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Mappy.System;
using Mappy.UserInterface.Components;
using Mappy.Utilities;
using Condition = Mappy.Utilities.Condition;

namespace Mappy.UserInterface.Windows;

public class MapWindow : Window
{
    private Vector2 mouseDragStart;
    private bool dragStarted;
    private Vector2 lastWindowSize = Vector2.Zero;
    private readonly MapToolbar toolbar = new();
    
    public static Vector2 MapContentsStart { get; private set; }
    
    public MapWindow() : base("Mappy Map Window")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(470,200),
            MaximumSize = new Vector2(9999,9999)
        };

        IsOpen = Service.Configuration.KeepOpen.Value;
    }

    public override void PreOpenCheck()
    {
        if (Service.Configuration.KeepOpen.Value) IsOpen = true;
        if (Service.ClientState.IsPvP) IsOpen = false;
        if (!Service.ClientState.IsLoggedIn) IsOpen = false;
    }

    public override bool DrawConditions()
    {
        if (Service.Configuration.HideInDuties.Value && Condition.IsBoundByDuty()) return false;
        if (Service.Configuration.HideInCombat.Value && Service.Condition[ConditionFlag.InCombat]) return false;
        if (Service.Configuration.HideBetweenAreas.Value && Condition.BetweenAreas()) return false;

        return true;
    }

    public override void Draw()
    {
        SetFlags();
        if (!toolbar.MapSelect.ShowMapSelectOverlay) ReadMouse();

        MapContentsStart = ImGui.GetCursorScreenPos();
        if (ImGui.BeginChild("###MapFrame", ImGui.GetContentRegionAvail(), false, Flags))
        {
            var shouldFadeMap = Service.Configuration.FadeWhenUnfocused.Value && !IsFocused;
            MapRenderer.DrawMap(shouldFadeMap);

            var shouldDrawComponents = !Service.MapManager.LoadingNextMap && !toolbar.MapSelect.ShowMapSelectOverlay;
            foreach (var component in Service.MapManager.MapComponents.TakeWhile(_ => shouldDrawComponents))
            {
                component.Draw();
            }

            toolbar.Draw(IsFocused);
            Service.ContextMenu.Draw();
        }
        ImGui.EndChild();
    }

    private void ReadMouse()
    {
        if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow) || IsFocused)
        {
            if (IsCursorInWindow() && !IsCursorInWindowHeader())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    Service.ContextMenu.Show(ContextMenuType.General);
                }
                
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
        Flags = WindowFlags.AlwaysActiveFlags;

        if (Service.Configuration.LockWindow.Value) Flags |= WindowFlags.NoMoveResizeFlags;
        if (Service.Configuration.HideWindowFrame.Value) Flags |= WindowFlags.NoDecorationFlags;
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
    
    public static bool IsBoundedBy(Vector2 cursor, Vector2 minBounds, Vector2 maxBounds)
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