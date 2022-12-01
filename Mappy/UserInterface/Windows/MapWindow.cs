using System;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Mappy.DataModels;
using Condition = Mappy.Utilities.Condition;

namespace Mappy.UserInterface.Windows;

public class MapWindow : Window, IDisposable
{
    private static MapData MapData => Service.MapManager.MapData;
    
    private Vector2 mouseDragStart;
    private bool dragStarted;
    private Vector2 lastWindowSize = Vector2.Zero;

    public MapWindow() : base("Mappy Map Window")
    {

        Flags |= ImGuiWindowFlags.NoFocusOnAppearing;
        Flags |= ImGuiWindowFlags.NoNavFocus;
        Flags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
        Flags |= ImGuiWindowFlags.NoMove;
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        IsOpen = true;
    }

    public void Dispose()
    {
    }

    public override void PreOpenCheck()
    {
        if (Service.Configuration.KeepOpen.Value) IsOpen = true;
        
        if (Service.ClientState.IsPvP) IsOpen = false;
        if (!Service.ClientState.IsLoggedIn) IsOpen = false;
        if (Service.Configuration.HideInDuties.Value && Condition.IsBoundByDuty()) IsOpen = false; 
    }

    public override void PreDraw()
    {

        
        if (Service.Configuration.HideWindowFrame.Value)
        {
            Flags |= ImGuiWindowFlags.NoDecoration;
            
            ImGui.PushStyleColor(ImGuiCol.WindowBg, Vector4.Zero);
        }
        else
        {
            Flags &= ~ImGuiWindowFlags.NoDecoration;
            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }
    }

    public override void Draw()
    {
        EvaluateWindowResize();
        
        EvaluateInputs();
        
        if (ImGui.BeginChild("###MapFrame", ImGui.GetContentRegionAvail(), false, Flags))
        {
            MapData.Viewport.Size = ImGui.GetContentRegionAvail();
            Service.MapManager.DrawMap();
        }
        ImGui.EndChild();
    }

    public override void PostDraw()
    {
        if (Service.Configuration.HideWindowFrame.Value)
        {
            ImGui.PopStyleColor();
        }
    }

    private void EvaluateWindowResize()
    {
        var windowSize = ImGui.GetWindowSize();

        if (lastWindowSize != windowSize && lastWindowSize != Vector2.Zero)
        {
            var delta = windowSize - lastWindowSize;

            MapData.Viewport.Center += delta / MapData.Viewport.Scale;
        }

        lastWindowSize = windowSize;
    }

    private void EvaluateInputs()
    {
        SetMoveFlags(IsInWindowHeader());

        if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow) || IsFocused)
        {
            EvaluateDrag();

            // if (dragStarted)
            // {
            //     Service.MapManager.FollowPlayer = false;
            // }

            EvaluateZoom();
        }
    }
    
    private static void EvaluateZoom()
    {
        if (IsCursorInWindow() && !IsInWindowHeader())
        {
            // Mouse Wheel Up
            if (ImGui.GetIO().MouseWheel > 0)
            {
                MapData.Viewport.Scale += 0.2f;
            }
            // Mouse Wheel Down
            else if (ImGui.GetIO().MouseWheel < 0)
            {
                MapData.Viewport.Scale -= 0.2f;
            }
        }
    }

    private void EvaluateDrag()
    {
        if (IsCursorInWindow() && !IsInWindowHeader())
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && !ImGui.IsMouseDragging(ImGuiMouseButton.Left))
            {
                mouseDragStart = ImGui.GetMousePos();
                dragStarted = true;
            }
            else if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && !dragStarted)
            {
                mouseDragStart = ImGui.GetMousePos();
                dragStarted = true;
            }
            else if(!ImGui.IsMouseDragging(ImGuiMouseButton.Left))
            {
                dragStarted = false;
            }
        }
        
        if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && dragStarted)
        {
            MapData.Viewport.Center -= (ImGui.GetMousePos() - mouseDragStart) / MapData.Viewport.Scale;
            mouseDragStart = ImGui.GetMousePos();
        }
    }

    private void SetMoveFlags(bool enableMoving)
    {
        if (enableMoving && !Service.Configuration.LockWindow.Value)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
            Flags &= ~ImGuiWindowFlags.NoResize;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
            Flags |= ImGuiWindowFlags.NoResize;
        }
    }
    
    private static bool IsCursorInWindow()
    {
        var windowStart = ImGui.GetWindowPos();
        var windowSize = ImGui.GetWindowSize();

        return IsBoundedBy(ImGui.GetMousePos(), windowStart, windowStart + windowSize);
    }

    private static bool IsInWindowHeader()
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