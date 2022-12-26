using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Mappy.UI;

public class ConfigWindow : Window
{
    public ConfigWindow() : base("Mappy Configuration")
    {
        
    }

    public override void Draw()
    {
        ImGui.Text($"WindowPosition: {Service.AreaMapAddon.GetWindowPosition()}");
        ImGui.Text($"WindowPosition: {Service.AreaMapAddon.GetWindowSize()}");
    }
}