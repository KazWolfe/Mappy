using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class MapMarkerOptions : IModuleSettings
{
    private static MapMarkersSettings Settings => Service.Configuration.MapMarkers;
    
    public ComponentName ComponentName => ComponentName.MapMarker;
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.FeatureToggles)
            .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.DefaultTooltipColor, Settings.StandardColor, Colors.White)
            .AddConfigColor(Strings.Map.MapLinkTooltipColor, Settings.MapLink, Colors.MapTextBrown)
            .AddConfigColor(Strings.Map.InstanceLinkTooltipColor, Settings.InstanceLink, Colors.Orange)
            .AddConfigColor(Strings.Map.AetheryteTooltipColor, Settings.Aetheryte, Colors.Blue)
            .AddConfigColor(Strings.Map.AethernetTooltipColor, Settings.Aethernet, Colors.BabyBlue)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.IconScale.Value = 0.50f;
                Service.Configuration.Save();
            }, ImGuiHelpers.ScaledVector2(InfoBox.Instance.InnerWidth, 23.0f))
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.IconSelect)
            .AddString(Strings.Map.Info)
            .AddDummy(8.0f)
            .BeginFlexGrid()
            .MultiSelect(Settings.IconSettings)
            .EndFlexGrid()
            .Draw();
    }
}