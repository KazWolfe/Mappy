using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PlayerMapComponentSettings
{
    public Setting<Vector4> OutlineColor = new(Colors.Grey);
    public Setting<Vector4> FillColor = new(Colors.Blue with {W = 0.20f});
    public Setting<float> IconScale = new(0.6f);
    public Setting<float> ConeRadius = new(90.0f);
    public Setting<float> ConeAngle = new(90.0f);
    public Setting<float> OutlineThickness = new(2.0f);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowCone = new(true);
}

public class PlayerMapComponent : IMapComponent
{
    private static PlayerMapComponentSettings Settings => Service.Configuration.PlayerSettings;
    
    public void Update(uint mapID)
    {
    }

    public void Draw()
    {
        if (!Service.MapManager.PlayerInCurrentMap) return;
        if (Service.ClientState.LocalPlayer is not { } player) return;
        
        if(Settings.ShowCone.Value) DrawLookLine(player);
        if(Settings.ShowIcon.Value) DrawBluePlayerIcon(player);
    }
    
    private void DrawBluePlayerIcon(GameObject player)
    {
        var icon = Service.Cache.IconCache.GetIconTexture(60443); 
        MapRenderer.DrawImageRotated(icon, player, Settings.IconScale.Value);
    }

    private void DrawLookLine(GameObject player)
    {
        var angle = GetCameraRotation();

        var playerPosition = Service.MapManager.GetObjectPosition(player);
        var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(playerPosition);

        var lineLength = Settings.ConeRadius.Value * MapRenderer.Viewport.Scale;
        
        var halfConeAngle = DegreesToRadians(Settings.ConeAngle.Value) / 2.0f;
        
        DrawAngledLineFromCenter(drawPosition, lineLength, angle - halfConeAngle);
        DrawAngledLineFromCenter(drawPosition, lineLength, angle + halfConeAngle);
        DrawLineArcFromCenter(drawPosition, lineLength, angle);
        
        DrawFilledSemiCircle(drawPosition, lineLength, angle);
    }

    private void DrawAngledLineFromCenter(Vector2 center, float lineLength, float angle)
    {
        var lineSegment = new Vector2(lineLength * MathF.Cos(angle), lineLength * MathF.Sin(angle));
        ImGui.GetWindowDrawList().AddLine(center, center + lineSegment, ImGui.GetColorU32(Settings.OutlineColor.Value), Settings.OutlineThickness.Value);
    }

    private void DrawLineArcFromCenter(Vector2 center, float distance, float rotation)
    {
        var halfConeAngle = DegreesToRadians(Settings.ConeAngle.Value) / 2.0f;
        
        var start = rotation - halfConeAngle;
        var stop = rotation + halfConeAngle;
        
        ImGui.GetWindowDrawList().PathArcTo(center, distance, start, stop);
        ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(Settings.OutlineColor.Value), ImDrawFlags.None, Settings.OutlineThickness.Value);
    }

    private void DrawFilledSemiCircle(Vector2 center, float distance, float rotation)
    {
        var halfConeAngle = DegreesToRadians(Settings.ConeAngle.Value) / 2.0f;
        
        var startAngle = rotation - halfConeAngle;
        var stopAngle = rotation + halfConeAngle;
        
        var startPosition = new Vector2(distance * MathF.Cos(rotation - halfConeAngle), distance * MathF.Sin(rotation - halfConeAngle));

        ImGui.GetWindowDrawList().PathArcTo(center, distance, startAngle, stopAngle);
        ImGui.GetWindowDrawList().PathLineTo(center);
        ImGui.GetWindowDrawList().PathLineTo(center + startPosition);
        ImGui.GetWindowDrawList().PathFillConvex(ImGui.GetColorU32(Settings.FillColor.Value));
    }
    
    private unsafe float GetCameraRotation()
    {
        var cameraManager = CameraManager.Instance()->CurrentCamera;

        var yaw = MathF.Atan2(-1 * cameraManager->Vector_4.X, -1 * cameraManager->Vector_2.X);

        return yaw + 0.5f * MathF.PI;
    }

    private float DegreesToRadians(float degrees)
    {
        return MathF.PI / 180 * degrees;
    }
}