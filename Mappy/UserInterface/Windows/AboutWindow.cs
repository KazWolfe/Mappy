using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows;

public class AboutWindow : Window
{
    public AboutWindow() : base("Mappy About")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(575, 660),
            MaximumSize = new Vector2(575, 660)
        };

        Flags |= ImGuiWindowFlags.NoResize;
    }

    public override void Draw()
    {
        ImGuiHelpers.ScaledDummy(10.0f);
        
        InfoBox.Instance
            .AddTitle("About")
            .AddString("Mappy is not intended to replace the minimap.", Colors.Orange)
            .AddDummy(5.0f)
            .AddString("Mappy is a replacement to the games normal Main Map. With many additional features, tons of customization options, complete integration with the game, and features that allow integration with other plugins.")
            .AddString("The primary goal with this project is to allow other plugin developers to enhance the player experience by seamlessly integrating with the game map.")
            .AddString("Over the last year there have been many requests for plugins to add markers or indicators to the main map, and often they are met with 'Interacting with the game map is a huge pain', with Mappy, all your dreams can come true!")
            .Draw();
        
        InfoBox.Instance
            .AddTitle("Thank You")
            .AddString("Mappy is a project that I have been wanting to start for a few months, at the time I was inspired to start this project, I didn't know nearly enough about how Dalamud and FFXIV work to make something even slightly functional.")
            .AddString("After having completed DailyDuty however, I realized I had the sufficient skills to make Mappy a reality. I am very happy to be able to offer a better in game map to all my fellow plugin enjoyers.")
            .Draw();
        
        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFFC);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
        
        InfoBox.Instance
            .AddTitle("Support")
            .AddString("If you would like to help me out, \nI have setup a Ko-Fi to support my projects")
            .AddString("Even if you choose not to donate, know that I genuinely appreciate that you are enjoying using what I have put my heart into creating for you. <3")
            .AddDummy(10.0f)
            .AddButton("Support me on Ko-Fi", () => Process.Start(new ProcessStartInfo {FileName = "https://ko-fi.com/midorikami", UseShellExecute = true}), new Vector2(InfoBox.Instance.InnerWidth, 27.0f * ImGuiHelpers.GlobalScale))
            .Draw();
        
        ImGui.PopStyleColor(3);
    }

    public static void DrawInfoButton()
    {
        var windowSize = ImGui.GetWindowSize();

        var position = windowSize - ImGuiHelpers.ScaledVector2(60.0f, 40.0f);

        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.SetCursorPos(position);
        ImGui.PushStyleColor(ImGuiCol.Button, 0x000000FF);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xFFA06020);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        
        if(ImGui.BeginChild("###InfoButtonChild", ImGuiHelpers.ScaledVector2(30.0f), false, ImGuiWindowFlags.NoScrollbar))
        {
            if (ImGui.Button(FontAwesomeIcon.InfoCircle.ToIconString(), ImGuiHelpers.ScaledVector2(30.0f)))
            {
                if (Service.WindowManager.GetWindowOfType<AboutWindow>(out var window))
                {
                    window.IsOpen = true;
                }
            }
        }        
        ImGui.EndChild();
        
        ImGui.PopStyleVar();
        
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopFont();
    }
}