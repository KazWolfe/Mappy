using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Mappy.UI;

public class MapOverlay : Window
{
    public MapOverlay() : base("Mappy Map Overlay")
    {
        Flags |= ImGuiWindowFlags.NoDecoration;
        Flags |= ImGuiWindowFlags.NoInputs;
        Flags |= ImGuiWindowFlags.NoBackground;
    }

    public override void PreOpenCheck() => IsOpen = Service.AreaMapAddon.AddonActive;

    public override void PreDraw()
    {
        Position = Service.AreaMapAddon.GetWindowPosition();
        Size = Service.AreaMapAddon.GetWindowSize();
    }

    public override void Draw()
    {
        // var windowStart = Service.AreaMapAddon.GetMapPosition();
        // var windowSize = Service.AreaMapAddon.GetMapSize();
        //
        // ImGui.GetWindowDrawList().AddRectFilled(windowStart, windowStart + windowSize, (Colors.Red with {W = 0.10f}).U32());
        //
        // var mapCorner = Service.AreaMapAddon.GetMapTopLeft();
        //
        // ImGui.GetForegroundDrawList().AddCircleFilled(windowStart + mapCorner, 5.0f, Colors.Purple.U32());
    }
}