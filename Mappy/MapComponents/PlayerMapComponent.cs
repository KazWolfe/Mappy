using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PlayerMapComponent : IMapComponent
{
    public void Update(uint mapID)
    {
    }

    public void Draw()
    {
        if (!Service.MapManager.PlayerInCurrentMap) return;
        if (Service.ClientState.LocalPlayer is not { } player) return;
        
        DrawLookLine(player);
        DrawBluePlayerIcon(player);
    }
    
    private void DrawBluePlayerIcon(GameObject player)
    {
        var icon = Service.Cache.IconCache.GetIconTexture(60443); 
        MapRenderer.DrawImageRotated(icon, player, 0.6f);
    }

    private void DrawLookLine(GameObject player)
    {
        var angle = GetCameraRotation();

        var playerPosition = Service.MapManager.GetObjectPosition(player);
        var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(playerPosition);

        var lineLength = 90.0f * MapRenderer.Viewport.Scale;
        
        DrawAngledLineFromCenter(drawPosition, lineLength, angle - 0.25f * MathF.PI);
        DrawAngledLineFromCenter(drawPosition, lineLength, angle + 0.25f * MathF.PI);
        DrawLineArcFromCenter(drawPosition, lineLength, angle);
        
        DrawFilledSemiCircle(drawPosition, lineLength, angle);
    }

    private static void DrawAngledLineFromCenter(Vector2 center, float lineLength, float angle)
    {
        var lineSegment = new Vector2(lineLength * MathF.Cos(angle), lineLength * MathF.Sin(angle));
        ImGui.GetWindowDrawList().AddLine(center, center + lineSegment, ImGui.GetColorU32(Colors.Grey), 2.0f);
    }

    private static void DrawLineArcFromCenter(Vector2 center, float distance, float rotation)
    {
        var start = rotation - 0.25f * MathF.PI - 0.0025f;
        var stop = rotation + 0.25f * MathF.PI + 0.005f;
        
        ImGui.GetWindowDrawList().PathArcTo(center, distance, start, stop);
        ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(Colors.Grey), ImDrawFlags.None, 2.0f);
    }

    private static void DrawFilledSemiCircle(Vector2 center, float distance, float rotation)
    {
        var startAngle = rotation - 0.25f * MathF.PI - 0.0025f;
        var stopAngle = rotation + 0.25f * MathF.PI + 0.005f;
        
        ImGui.GetWindowDrawList().PathArcTo(center, distance, startAngle, stopAngle);
        ImGui.GetWindowDrawList().PathLineTo(center);
        
        ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(Colors.Blue with {W = 0.20f}));
    }
    
    private unsafe float GetCameraRotation()
    {
        var cameraManager = CameraManager.Instance()->CurrentCamera;

        var yaw = MathF.Atan2(-1 * cameraManager->Vector_4.X, -1 * cameraManager->Vector_2.X);

        return yaw + 0.5f * MathF.PI;
    }
}