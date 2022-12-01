using System.ComponentModel;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Mappy.Localization;
using Mappy.UserInterface.Components;

namespace Mappy.UserInterface.Windows;

public class ConfigurationWindow : Window
{
    public ConfigurationWindow() : base("Mappy Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350,350),
            MaximumSize = new Vector2(350,350)
        };
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.GeneralSettings)
            .AddConfigCheckbox(Strings.Configuration.KeepOpen, Service.Configuration.KeepOpen)
            .AddConfigCheckbox(Strings.Configuration.FollowPlayer, Service.Configuration.FollowPlayer)
            .AddConfigCheckbox(Strings.Configuration.LockWindow, Service.Configuration.LockWindow)
            .AddConfigCheckbox(Strings.Configuration.HideWindowFrame, Service.Configuration.HideWindowFrame)
            .AddConfigCheckbox(Strings.Configuration.HideInDuties, Service.Configuration.HideInDuties)
            .AddConfigCheckbox(Strings.Configuration.FadeWhenUnfocused, Service.Configuration.FadeWhenUnfocused)
            .AddDragFloat(Strings.Configuration.FadePercent, Service.Configuration.FadePercent, 0.0f, 1.0f, 150.0f * ImGuiHelpers.GlobalScale)
            .Draw();
    }
}