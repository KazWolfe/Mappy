using System.Numerics;
using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class QuestMarkerOptions : IModuleSettings
{
    private static QuestSettings Settings => Service.Configuration.QuestMarkers;
    public ComponentName ComponentName => ComponentName.QuestMarker;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
            .AddDummy(8.0f)
            .AddConfigCheckbox(Strings.Map.Quests.Tribal, Settings.ShowTribal)
            .AddConfigCheckbox(Strings.Map.Quests.Festival, Settings.ShowFestival)
            .AddConfigCheckbox(Strings.Map.Quests.GrandCompany, Settings.ShowGrandCompany)
            .AddConfigCheckbox(Strings.Map.Quests.HideAccepted, Settings.HideAccepted)
            .AddConfigCheckbox(Strings.Map.Quests.HideUnaccepted, Settings.HideUnaccepted)
            .AddConfigCheckbox(Strings.Map.Generic.ShowTooltip, Settings.ShowTooltip)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.Quests.ObjectiveColor, Settings.InProgressColor, Colors.Red with {W = 0.20f})
            .AddConfigColor(Strings.Map.Quests.AcceptedColor, Settings.AcceptedTooltipColor, Colors.SoftGreen)
            .AddConfigColor(Strings.Map.Quests.UnacceptedColor, Settings.UnacceptedTooltipColor, Colors.White)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.Quests.AcceptedScale, Settings.AcceptedScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddDragFloat(Strings.Map.Quests.UnacceptedScale, Settings.UnacceptedScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.AcceptedScale.Value = 0.75f;
                Settings.UnacceptedScale.Value = 0.75f;
                Service.Configuration.Save();
            }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Special)
            .AddButton(Strings.Map.Quests.ResetBlacklist, () =>
            {
                Service.Configuration.QuestMarkers.HiddenQuests.Clear();
                Service.Configuration.Save();
            }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
            .Draw();
    }
}