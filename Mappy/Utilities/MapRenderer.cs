using System.Numerics;
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

    public static void DrawIcon(TextureWrap? iconTexture, Vector2 position)
    {
        if (iconTexture is not null)
        {
            var iconSize = new Vector2(iconTexture.Width, iconTexture.Height);
            SetImGuiDrawPosition(position * Viewport.Scale - iconSize / 2.0f);
            ImGui.Image(iconTexture.ImGuiHandle, iconSize);
        }
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