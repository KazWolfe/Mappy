using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class PlayerOptions : ISelectable
{
    private static PlayerMapComponentSettings Settings => Service.Configuration.PlayerSettings;
    
    public ComponentName ComponentName => ComponentName.Player;
 
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Player.Enable, Settings.Enable)
            .AddDummy(8.0f)
            .AddConfigCheckbox(Strings.Map.Player.ShowIcon, Settings.ShowIcon)
            .AddConfigCheckbox(Strings.Map.Player.ShowCone, Settings.ShowCone)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.Player.OutlineColor, Settings.OutlineColor, Colors.Grey)
            .AddConfigColor(Strings.Map.Player.FillColor, Settings.FillColor, Colors.Blue with { W = 0.2f })
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.Player.IconScale, Settings.IconScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddDragFloat(Strings.Map.Player.ConeRadius, Settings.ConeRadius, 30.0f, 240f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddDragFloat(Strings.Map.Player.ConeAngle, Settings.ConeAngle, 0.0f, 180.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddDragFloat(Strings.Map.Player.ConeThickness, Settings.OutlineThickness, 0.5f, 10.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.IconScale.Value = 0.60f;
                Settings.ConeRadius.Value = 90.0f;
                Settings.ConeAngle.Value = 90.0f;
                Settings.OutlineThickness.Value = 2.0f;
                Service.Configuration.Save();
            }, ImGuiHelpers.ScaledVector2(InfoBox.Instance.InnerWidth, 23.0f))
            .Draw();
    }
}