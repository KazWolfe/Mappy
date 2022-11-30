using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PlayerMapComponent
{
    private static MapData MapData => Service.MapManager.MapData;
    
    public void Draw()
    {
        if (!MapData.DataAvailable) return;
        if (Service.ClientState.LocalPlayer is not { } player) return;

        DrawLookLine(player);
        DrawBluePlayerIcon(player);
    }

    private void DrawBluePlayerIcon(GameObject player)
    {
        if (Service.IconManager.GetIconTexture(60443) is { } playerIcon)
        {
            var angle = -player.Rotation + 0.5f * MathF.PI;
            var viewportOffset = MapData.Viewport.ScaledTopLeft;
            
            var center = MapData.GetScaledGameObjectPosition(player.Position) + ImGui.GetWindowPos();
            
            DrawImageRotated(playerIcon, -viewportOffset + center, angle, 1.25f);
        }
    }

    private void DrawLookLine(GameObject player)
    {
        var angle = GetCameraRotation();
        var center = -MapData.Viewport.ScaledTopLeft + MapData.GetScaledGameObjectPosition(player.Position) + ImGui.GetWindowPos();

        var lineLength = 90.0f * MapData.Viewport.Scale;
        
        DrawAngledLineFromCenter(center, lineLength, angle - 0.25f * MathF.PI);
        DrawAngledLineFromCenter(center, lineLength, angle + 0.25f * MathF.PI);
        DrawLineArcFromCenter(center, lineLength, angle);
        
        DrawFilledSemiCircle(center, lineLength, angle);
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

    private static Vector2 ImRotate(Vector2 v, float cosA, float sinA) 
    { 
        return new Vector2(v.X * cosA - v.Y * sinA, v.X * sinA + v.Y * cosA);
    }

    private unsafe float GetCameraRotation()
    {
        var cameraManager = CameraManager.Instance()->CurrentCamera;

        var yaw = -1 * Math.Atan2(-1 * cameraManager->Vector_4.X, -1 * cameraManager->Vector_2.X);

        return -((float) yaw - 0.5f * MathF.PI);
    }

    // private static Vector2 GetTextureSize(TextureWrap texture) => new(texture.Width, texture.Height);
    //
    // private static Vector2 GetScaledTextureCenter(TextureWrap texture, MapViewport viewport) => GetTextureSize(texture) / 2.0f * viewport.Scale;
    //
    // private static Vector2 GetPlayerCenteredViewport(TextureWrap texture, MapViewport viewport, Map map, Vector3 playerPosition)
    // {
    //     return -viewport.ScaledTopLeft + GetPlayerPosition(playerPosition, map, viewport) + GetScaledTextureCenter(texture, viewport);
    // }
    //
    // private static Vector2 GetPlayerPosition(Vector3 position, Map map, MapViewport viewport)
    // {
    //     var mapScalar = GetMapScalar(map);
    //
    //     var playerPosition = new Vector2(position.X, position.Z) * mapScalar * viewport.Scale;
    //     var mapOffset = new Vector2(map.OffsetX, map.OffsetY) * mapScalar * viewport.Scale;
    //
    //     return playerPosition + mapOffset;
    // }
    //
    // private static float GetMapScalar(Map map) => map.SizeFactor / 100.0f;

    private static void DrawImageRotated(TextureWrap texture, Vector2 center, float angle, float iconScale)
    {
        var size = new Vector2(texture.Width, texture.Height) * iconScale;

        var cosA = MathF.Cos(angle + 0.5f * MathF.PI);
        var sinA = MathF.Sin(angle + 0.5f * MathF.PI);

        Vector2[] vectors =
        {
            center + ImRotate(new Vector2(-size.X * 0.5f, -size.Y * 0.5f), cosA, sinA),
            center + ImRotate(new Vector2(+size.X * 0.5f, -size.Y * 0.5f), cosA, sinA),
            center + ImRotate(new Vector2(+size.X * 0.5f, +size.Y * 0.5f), cosA, sinA),
            center + ImRotate(new Vector2(-size.X * 0.5f, +size.Y * 0.5f), cosA, sinA)
        };

        var windowDrawList = ImGui.GetWindowDrawList();
        windowDrawList.AddImageQuad(texture.ImGuiHandle, vectors[0], vectors[1], vectors[2], vectors[3]);
    }
}