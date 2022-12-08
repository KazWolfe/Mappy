using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class GatheringPointOptions : ISelectable
{
    private static GatheringPointSettings Settings => Service.Configuration.GatheringPoints;
    
    public ComponentName ComponentName => ComponentName.GatheringPoint;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Gathering.Enable, Settings.Enable)
            .AddDummy(8.0f)
            .AddConfigCheckbox(Strings.Map.Gathering.ShowIcon, Settings.ShowIcon)
            .AddConfigCheckbox(Strings.Map.Gathering.ShowTooltip, Settings.ShowTooltip)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.Gathering.TooltipColor, Settings.TooltipColor, Colors.White)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.Gathering.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.IconScale.Value = 0.50f;
                Service.Configuration.Save();
            }, ImGuiHelpers.ScaledVector2(InfoBox.Instance.InnerWidth, 23.0f))
            .Draw();
    }
}