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
            .AddConfigCheckbox(Strings.Map.Fate.ShowIcon, Settings.ShowIcon)
            .AddConfigCheckbox(Strings.Map.Fate.ShowRing, Settings.ShowRing)
            .AddConfigCheckbox(Strings.Map.Fate.ShowTooltip, Settings.ShowTooltip)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.Fate.Color, Settings.Color, Colors.FatePink)
            .Draw();
    }
}