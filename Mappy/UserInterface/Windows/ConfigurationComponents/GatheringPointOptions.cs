using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class GatheringPointOptions : ISelectable
{
    private static GatheringPointSettings Settings => Service.Configuration.GatheringPoints;
    
    public ComponentName ComponentName => ComponentName.GatheringPoint;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Gathering.ShowIcon, Settings.ShowIcon)
            .AddConfigCheckbox(Strings.Map.Gathering.ShowTooltip, Settings.ShowTooltip)
            .Draw();
    }
}