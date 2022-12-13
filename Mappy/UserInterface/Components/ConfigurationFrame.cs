using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.UserInterface.Components;

public class ConfigurationFrame
{
    public void Draw(IModuleSettings? selected)
    {
        DrawVerticalLine();
        ImGuiHelpers.ScaledDummy(0.0f);
        ImGui.SameLine();

        var regionAvailable = ImGui.GetContentRegionAvail();

        if (ImGui.BeginChild("###ConfigurationFrame", regionAvailable with {Y = 0}, false, ImGuiWindowFlags.AlwaysVerticalScrollbar))
        {
            if (selected != null)
            {
                selected.DrawSettings();
            }
            else
            {
                var available = ImGui.GetContentRegionAvail() / 2.0f;
                var textSize = ImGui.CalcTextSize(Strings.Configuration.ModuleNotSelected) / 2.0f;
                var center = new Vector2(available.X - textSize.X, available.Y - textSize.Y);

                ImGui.SetCursorPos(center);
                ImGui.TextWrapped(Strings.Configuration.ModuleNotSelected);
            }
        }

        ImGui.EndChild();
    }

    private void DrawVerticalLine()
    {
        var contentArea = ImGui.GetContentRegionAvail();
        var cursor = ImGui.GetCursorScreenPos();
        var drawList = ImGui.GetWindowDrawList();
        var color = ImGui.GetColorU32(Colors.White);

        drawList.AddLine(cursor, cursor with {Y = cursor.Y + contentArea.Y}, color, 1.0f);
    }
}