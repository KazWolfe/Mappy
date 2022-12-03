using System;
using System.Numerics;
using ImGuiNET;

namespace Mappy.Utilities;

public static class Draw
{
    public static void TextOutlined(Vector2 startingPosition, string text, float scale)
    {
        startingPosition = startingPosition.Ceil();

        var outlineThickness = (int)MathF.Ceiling(2);
        
        for (var x = -outlineThickness; x <= outlineThickness; ++x)
        {
            for (var y = -outlineThickness; y <= outlineThickness; ++y)
            {
                if (x == 0 && y == 0) continue;

                DrawText(startingPosition + new Vector2(x, y), text, scale, Colors.White);
            }
        }

        DrawText(startingPosition, text, scale, Colors.MapTextBrown);
    }
    
    private static void DrawText(Vector2 drawPosition, string text, float scale, Vector4 color)
    {
        var drawList = ImGui.GetWindowDrawList();

        var font = ImGui.GetFont();
        var fontSize = font.FontSize * scale;
        
        drawList.AddText(font, fontSize, drawPosition, ImGui.GetColorU32(color), text);
    }
}

public static class VectorExtensions
{
    public static Vector2 Ceil(this Vector2 data)
    {
        return new Vector2(MathF.Ceiling(data.X), MathF.Ceiling(data.Y));
    }
}