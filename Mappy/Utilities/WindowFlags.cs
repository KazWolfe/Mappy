using ImGuiNET;

namespace Mappy.Utilities;

public static class WindowFlags
{
    public const ImGuiWindowFlags AlwaysActiveFlags =
        ImGuiWindowFlags.NoFocusOnAppearing |
        ImGuiWindowFlags.NoNav |
        ImGuiWindowFlags.NoBringToFrontOnFocus |
        ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse;

    public const ImGuiWindowFlags NoDecorationFlags =
        ImGuiWindowFlags.NoDecoration |
        ImGuiWindowFlags.NoBackground;

    public const ImGuiWindowFlags NoMoveResizeFlags =
        ImGuiWindowFlags.NoMove |
        ImGuiWindowFlags.NoResize;
}