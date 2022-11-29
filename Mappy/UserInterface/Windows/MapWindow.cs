using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;

namespace Mappy.UserInterface.Windows;

public class MapWindow : Window, IDisposable
{
    public MapWindow() : base("Mappy Map Window")
    {
        // SizeConstraints = new WindowSizeConstraints
        // {
        //     MinimumSize = new Vector2(350,350),
        //     MaximumSize = new Vector2(350,350)
        // };

        Flags |= ImGuiWindowFlags.NoMove;
        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        IsOpen = true;
    }

    public void Dispose()
    {
    }

    public override void PreDraw()
    {
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void Draw()
    {
        CheckMapDrag();
        
        if (ImGui.BeginChild("###MapFrame", ImGui.GetContentRegionAvail(), false, Flags))
        {
            Service.MapManager.MapViewport.Size = ImGui.GetContentRegionAvail();
            Service.MapManager.DrawMap();
        }
        ImGui.EndChild();
    }

    private Vector2 mouseDragStart;
    public bool DragStarted;
    
    private void CheckMapDrag()
    {
        if (IsInWindowHeader())
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
        
        if (IsCursorInWindow() && !IsInWindowHeader())
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && !ImGui.IsMouseDragging(ImGuiMouseButton.Left))
            {
                mouseDragStart = ImGui.GetMousePos();
                DragStarted = true;
            }
            else if(!ImGui.IsMouseDragging(ImGuiMouseButton.Left))
            {
                DragStarted = false;
            }

            // Mouse Wheel Up
            if (ImGui.GetIO().MouseWheel > 0)
            {
                Service.MapManager.ZoomIn(0.20f);
            }
            // Mouse Wheel Down
            else if (ImGui.GetIO().MouseWheel < 0)
            {
                Service.MapManager.ZoomOut(0.20f);
            }
        }
        
        if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && DragStarted)
        {
            Service.MapManager.MapViewport.Center -= (ImGui.GetMousePos() - mouseDragStart) / Service.MapManager.MapViewport.Scale;
            mouseDragStart = ImGui.GetMousePos();
        }
    }

    private bool IsCursorInWindow()
    {
        var windowStart = ImGui.GetWindowPos();
        var windowSize = ImGui.GetWindowSize();

        return IsBoundedBy(ImGui.GetMousePos(), windowStart, windowStart + windowSize);
    }

    private bool IsInWindowHeader()
    {
        var windowStart = ImGui.GetWindowPos();
        var headerSize = ImGui.GetWindowSize() with { Y = ImGui.GetWindowContentRegionMin().Y };
        
        return IsBoundedBy(ImGui.GetMousePos(), windowStart, windowStart + headerSize);
    }
    
    private bool IsBoundedBy(Vector2 cursor, Vector2 minBounds, Vector2 maxBounds)
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