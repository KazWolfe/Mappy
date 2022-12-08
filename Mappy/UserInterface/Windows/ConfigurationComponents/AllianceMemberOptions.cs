using System;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class AllianceMemberOptions : ISelectable
{
    private static AllianceMemberSettings Settings => Service.Configuration.AllianceSettings;
    
    public ComponentName ComponentName => ComponentName.AllianceMember;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.AllianceMembers.ShowIcon, Settings.ShowIcon)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Configuration.IconSelect)
            .AddString(Strings.Configuration.SelectedIcon)
            .AddIconImage((uint) Settings.SelectedIcon.Value)
            .AddConfigCombo(Enum.GetValues<AllianceMarkers>(), Settings.SelectedIcon, AllianceMarkersExtensions.GetTranslatedString, width: InfoBox.Instance.InnerWidth)
            .Draw();
    }
}