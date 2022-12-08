using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class GeneralOptions : ISelectable
{
    public ComponentName ComponentName => ComponentName.General;

    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.GeneralSettings)
            .AddConfigCheckbox(Strings.Configuration.KeepOpen, Service.Configuration.KeepOpen)
            .AddConfigCheckbox(Strings.Configuration.LockWindow, Service.Configuration.LockWindow)
            .AddConfigCheckbox(Strings.Configuration.HideWindowFrame, Service.Configuration.HideWindowFrame)
            .AddConfigCheckbox(Strings.Configuration.HideInDuties, Service.Configuration.HideInDuties)
            .AddConfigCheckbox(Strings.Configuration.AlwaysShowToolbar, Service.Configuration.AlwaysShowToolbar)
            .AddConfigCheckbox(Strings.Configuration.FadeWhenUnfocused, Service.Configuration.FadeWhenUnfocused)
            .AddDragFloat(Strings.Configuration.FadePercent, Service.Configuration.FadePercent, 0.0f, 1.0f, 150.0f * ImGuiHelpers.GlobalScale)
            .Draw();
        
        // Handle settings that aren't compatible with each other
        if (Service.Configuration.HideWindowFrame.Value)
        {
            Service.Configuration.LockWindow.Value = true;
            Service.Configuration.Save();
        }
    }
}