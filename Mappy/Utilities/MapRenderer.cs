using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using ImGuiScene;
using Mappy.DataModels;

namespace Mappy.Utilities;

public static class MapRenderer
{
    public static MapViewport Viewport { get; } = new();

    public static void DrawMap(bool faded)
    {
        if (Service.MapManager.MapTexture is { } mapTexture)
        {
            Viewport.Size = ImGui.GetContentRegionAvail();
            var fadePercent = faded ? 1.0f - Service.Configuration.FadePercent.Value : 1.0f;
            var scaledImageSize = new Vector2(mapTexture.Width, mapTexture.Height) * Viewport.Scale;
            
            SetImGuiDrawPosition();
            ImGui.Image(mapTexture.ImGuiHandle, scaledImageSize,Vector2.Zero, Vector2.One, Vector4.One with { W = fadePercent });
        }
    }

    public static void DrawIcon(TextureWrap? iconTexture, Vector2 position, float scale = 0.5f)
    {
        if (iconTexture is not null)
        {
            var iconSize = new Vector2(iconTexture.Width, iconTexture.Height) * scale;
            SetImGuiDrawPosition(position * Viewport.Scale - iconSize / 2.0f);
            ImGui.Image(iconTexture.ImGuiHandle, iconSize);
        }
    }

    public static void DrawIcon(TextureWrap? iconTexture, GameObject gameObject, float scale = 0.50f)
    {
        DrawIcon(iconTexture, Service.MapManager.GetObjectPosition(gameObject), scale);
    }

    public static void DrawIcon(uint iconId, Vector2 position, float scale = 0.50f)
    {
        DrawIcon(Service.Cache.IconCache.GetIconTexture(iconId), position, scale);
    }

    public static void DrawIcon(uint iconId, GameObject gameObject, float scale = 0.50f)
    {
        var icon = Service.Cache.IconCache.GetIconTexture(iconId);
        var position = Service.MapManager.GetObjectPosition(gameObject);
        
        DrawIcon(icon, position, scale);
    }
    
    public static void DrawImageRotated(TextureWrap? texture, GameObject gameObject, float iconScale = 0.5f)
    {
        if (texture is not null)
        {
            var objectPosition = Service.MapManager.GetObjectPosition(gameObject.Position);
            var center = GetImGuiWindowDrawPosition(objectPosition);
            var angle = GetObjectRotation(gameObject);
        
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
    
    public static void DrawTooltip(string text, Vector4 color)
    {
        if (!ImGui.IsItemHovered()) return;
        
        Draw.DrawTooltip(text, color);
    }

    private static float GetObjectRotation(GameObject gameObject)
    {
        return -gameObject.Rotation + 0.5f * MathF.PI;
    }
    
    private static Vector2 ImRotate(Vector2 v, float cosA, float sinA) 
    { 
        return new Vector2(v.X * cosA - v.Y * sinA, v.X * sinA + v.Y * cosA);
    }
    
    public static void MoveViewportCenter(Vector2 offset) => Viewport.Center += offset / Viewport.Scale;
    public static void SetViewportCenter(Vector2 position) => Viewport.Center = position;
    public static void ZoomIn(float zoomAmount) => Viewport.Scale += zoomAmount;
    public static void ZoomOut(float zoomAmount) => Viewport.Scale -= zoomAmount;
    public static void SetViewportZoom(float scale) => Viewport.Scale = scale;
    private static void SetImGuiDrawPosition() => ImGui.SetCursorPos(-Viewport.ScaledTopLeft);
    private static void SetImGuiDrawPosition(Vector2 position) => ImGui.SetCursorPos(-Viewport.ScaledTopLeft + position);
    public static Vector2 GetImGuiWindowDrawPosition(Vector2 position) => -Viewport.ScaledTopLeft + position * Viewport.Scale + ImGui.GetWindowPos();
}