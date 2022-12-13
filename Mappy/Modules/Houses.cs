using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class HousingSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<float> IconScale = new(0.5f);
    public Setting<Vector4> TooltipColor = new(Colors.White);
}

public class Houses : IModule
{
    private static HousingSettings Settings => Service.Configuration.Housing;
    public IMapComponent MapComponent { get; } = new HousingMarkersMapComponent();
    public IModuleSettings Options { get; } = new HousingOptions();

    private class HousingMarkersMapComponent : IMapComponent
    {
        private List<HousingMapMarkerInfo> housingMarkers = new();
        private HousingLandSet? housingSizeInfo;

        private bool isHousingDistrict;
        
        public void Update(uint mapID)
        {
            isHousingDistrict = GetHousingDistrictID() != uint.MaxValue;

            if (isHousingDistrict)
            {
                housingMarkers = Service.DataManager.GetExcelSheet<HousingMapMarkerInfo>()!
                    .Where(marker => marker.Map.Row == mapID)
                    .ToList();

                housingSizeInfo = Service.DataManager.GetExcelSheet<HousingLandSet>()!
                    .GetRow(GetHousingDistrictID());
            }
        }

        public void Draw()
        {
            if (!Settings.Enable.Value) return;
            if (!isHousingDistrict) return;
            
            foreach (var marker in housingMarkers)
            {
                if(Settings.ShowIcon.Value) DrawHousingMapMarker(marker);
                if(Settings.ShowTooltip.Value) DrawTooltip(marker);
            }
        }

        private void DrawHousingMapMarker(HousingMapMarkerInfo marker)
        {
            if (housingSizeInfo is null) return;
            
            var iconId = marker.SubRowId is 60 or 61 ? 60789 : GetIconID(housingSizeInfo.PlotSize[marker.SubRowId]);
            var position = Service.MapManager.GetObjectPosition(new Vector2(marker.X, marker.Z));

            MapRenderer.DrawIcon(iconId, position, Settings.IconScale.Value);
        }

        private void DrawTooltip(HousingMapMarkerInfo marker)
        {
            if (!ImGui.IsItemHovered()) return;
            if (marker.SubRowId is 60 or 61) return;
            
            ImGui.BeginTooltip();
            ImGui.TextColored(Settings.TooltipColor.Value,$"{marker.SubRowId + 1}");
            ImGui.EndTooltip();
        }

        private uint GetIconID(byte housingSize)
        {
            return housingSize switch
            {
                0 => 60754, // Small House
                1 => 60755, // Medium House
                2 => 60756, // Large House
                _ => 60750  // Housing Placeholder Marker
            };
        }

        private uint GetHousingDistrictID()
        {
            return Service.MapManager.LoadedMapId switch
            {
                72 or 192 => 0,
                82 or 193 => 1,
                83 or 194 => 2,
                364 or 365 => 3,
                679 or 680 => 4,
                _ => uint.MaxValue
            };
        }
    }

    private class HousingOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.Housing;
        public void DrawSettings()
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
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.White)
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
        }
    }
}

