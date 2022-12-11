using System.Collections.Generic;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Mappy.UserInterface.Windows;

public class DebugWindow : Window
{
    private static readonly List<string> DebugStrings = new();

    public static void AddString(string message) => DebugStrings.Add(message);
    
    public DebugWindow() : base("Mappy Debug Window", ImGuiWindowFlags.AlwaysAutoResize)
    {
#if DEBUG
        IsOpen = true;
#endif
    }

    public override void Draw()
    {
        foreach (var message in DebugStrings)
        {
            ImGui.TextUnformatted(message);
        }
        DebugStrings.Clear();
    }
}