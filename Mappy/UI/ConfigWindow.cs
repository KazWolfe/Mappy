using Dalamud.Interface.Windowing;
using ImGuiNET;
using Mappy.Commands;

namespace Mappy.UI;

public class ConfigWindow : Window
{
    public ConfigWindow() : base("Mappy Configuration")
    { 
        KamiLib.KamiLib.CommandManager.AddCommand(new ConfigurationWindowCommands());
    }

    public override void Draw()
    {
        ImGui.Text($"WindowPosition: {Service.AreaMapAddon.GetWindowPosition()}");
        ImGui.Text($"WindowPosition: {Service.AreaMapAddon.GetWindowSize()}");
    }
}