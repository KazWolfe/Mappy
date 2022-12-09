using System.Numerics;
using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows.ConfigurationComponents;

public class TemporaryMarkerOptions : IModuleSettings
{
    private static TemporaryMarkerSettings Settings => Service.Configuration.TemporaryMarkers;
    public ComponentName ComponentName => ComponentName.TemporaryMarker;
    
    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Info)
            .AddString(Strings.Map.TemporaryMarkers.About)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.ColorOptions)
            .AddConfigColor(Strings.Map.TemporaryMarkers.GatheringColor, Settings.GatheringColor, Colors.Blue with {W = 0.33f})
            .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.White)
            .Draw();
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.Adjustments)
            .AddDragFloat(Strings.Map.TemporaryMarkers.FlagScale, Settings.FlagScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddDragFloat(Strings.Map.TemporaryMarkers.GatheringAreaScale, Settings.GatheringScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .AddButton(Strings.Configuration.Reset, () =>
            {
                Settings.FlagScale.Value = 0.50f;
                Settings.GatheringScale.Value = 0.50f;
                Service.Configuration.Save();
            }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
            .Draw();
    }
}