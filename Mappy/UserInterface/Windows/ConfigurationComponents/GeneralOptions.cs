using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class GeneralOptions : IModuleSettings
{
    public ComponentName ComponentName => ComponentName.General;

    private readonly Setting<bool> dummySetting = new(true);

    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.GeneralSettings)
            .AddString(Strings.Configuration.RenderAboveGameUI)
            .AddDummy(1.0f)
            .AddString(Strings.Configuration.RenderAboveGameUIHelp, Colors.Orange)
            .AddDummy(1.0f)
            .AddString(Strings.Configuration.RenderAboveGameUIHelpExtended, Colors.Orange)
            .AddDummy(4.0f)
            .AddSeparator()
            .AddDummy(6.0f)
            .AddConfigCheckbox(Strings.Configuration.KeepOpen, Service.Configuration.KeepOpen)
            .AddConfigCheckbox(Strings.Configuration.LockWindow, Service.Configuration.LockWindow)
            .AddConfigCheckbox(Strings.Configuration.HideWindowFrame, Service.Configuration.HideWindowFrame)
            .AddConfigCheckbox(Strings.Configuration.HideInDuties, Service.Configuration.HideInDuties)
            .AddConfigCheckbox(Strings.Configuration.AlwaysShowToolbar, Service.Configuration.AlwaysShowToolbar)
            .AddConfigCheckbox(Strings.Configuration.FadeWhenUnfocused, Service.Configuration.FadeWhenUnfocused)
            .AddDragFloat(Strings.Configuration.FadePercent, Service.Configuration.FadePercent, 0.0f, 1.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .Draw();
        
        // Handle settings that aren't compatible with each other
        if (Service.Configuration.HideWindowFrame.Value)
        {
            Service.Configuration.LockWindow.Value = true;
            Service.Configuration.Save();
        }
    }
}