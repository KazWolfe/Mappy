using System.Numerics;
using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class AllianceMemberOptions : IModuleSettings
{
    private static AllianceMemberSettings Settings => Service.Configuration.AllianceSettings;
    
    public ComponentName ComponentName => ComponentName.AllianceMember;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
            .AddDummy(8.0f)
            .AddConfigCheckbox(Strings.Map.Generic.ShowIcon, Settings.ShowIcon)
            .AddConfigCheckbox(Strings.Map.Generic.ShowTooltip, Settings.ShowTooltip)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.ForestGreen)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.IconScale.Value = 0.50f;
                Service.Configuration.Save();
            }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Configuration.IconSelect)
            .BeginFlexGrid()
            .SingleSelect(Settings.SelectedIcon, 60358, 60359, 60360, 60361)
            .EndFlexGrid()
            .Draw();
    }
}