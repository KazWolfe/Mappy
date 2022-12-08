using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class FateOptions : ISelectable
{
    private static FateSettings Settings => Service.Configuration.FateSettings;
    
    public ComponentName ComponentName => ComponentName.Fate;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Fate.Enable, Settings.Enable)
            .AddDummy(8.0f)
            .AddConfigCheckbox(Strings.Map.Fate.ShowIcon, Settings.ShowIcon)
            .AddConfigCheckbox(Strings.Map.Fate.ShowTooltip, Settings.ShowTooltip)
            .AddConfigCheckbox(Strings.Map.Fate.ShowRing, Settings.ShowRing)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.Fate.Color, Settings.Color, Colors.FatePink)
            .AddConfigColor(Strings.Map.Fate.TooltipColor, Settings.TooltipColor, Colors.White)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.Fate.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.IconScale.Value = 0.50f;
                Service.Configuration.Save();
            }, ImGuiHelpers.ScaledVector2(InfoBox.Instance.InnerWidth, 23.0f))
            .Draw();
    }
}