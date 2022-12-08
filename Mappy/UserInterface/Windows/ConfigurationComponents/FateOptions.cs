using System.Numerics;
using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class FateOptions : IModuleSettings
{
    private static FateSettings Settings => Service.Configuration.FateSettings;
    
    public ComponentName ComponentName => ComponentName.Fate;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
            .AddDummy(8.0f)
            .AddConfigCheckbox(Strings.Map.Generic.ShowIcon, Settings.ShowIcon)
            .AddConfigCheckbox(Strings.Map.Generic.ShowTooltip, Settings.ShowTooltip)
            .AddConfigCheckbox(Strings.Map.Fate.ShowRing, Settings.ShowRing)
            .AddDummy(8.0f)
            .AddConfigCheckbox(Strings.Map.Fate.ExpiringWarning, Settings.ExpiringWarning, Strings.Map.Fate.ExpirationWarningHelp)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.Fate.Color, Settings.Color, Colors.FatePink)
            .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.White)
            .AddConfigColor(Strings.Map.Fate.ExpirationColor, Settings.ExpiringColor, Colors.SoftRed with { W = 0.33f })
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddInputInt(Strings.Map.Fate.EarlyWarningTime, Settings.EarlyWarningTime, 10, 3600, 0, 0, InfoBox.Instance.InnerWidth / 2.0f)
            .AddHelpMarker(Strings.Map.Fate.InSeconds)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.IconScale.Value = 0.50f;
                Settings.EarlyWarningTime.Value = 300;
                Service.Configuration.Save();
            }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
            .Draw();
    }
}