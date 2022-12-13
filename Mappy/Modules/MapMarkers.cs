using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Housing;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class MapMarkersSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> AetherytesOnTop = new(true);
    public Dictionary<uint,Setting<IconSelection>> IconSettingList = new();
    public Setting<float> IconScale = new(0.5f);
    public Setting<Vector4> StandardColor = new(Colors.White);
    public Setting<Vector4> MapLink = new(Colors.MapTextBrown);
    public Setting<Vector4> InstanceLink = new(Colors.Orange);
    public Setting<Vector4> Aetheryte = new(Colors.Blue);
    public Setting<Vector4> Aethernet = new(Colors.BabyBlue);
}

public class MapMarkers : IModule
{
    private static MapMarkersSettings Settings => Service.Configuration.MapMarkers;
    public IMapComponent MapComponent { get; } = new MapMarkersMapComponent();
    public IModuleSettings Options { get; } = new MapMarkerOptions();

    private class MapMarkersMapComponent : IMapComponent
    {
        private readonly List<MapMarkerData> mapMarkers = new();

        private bool dataStale;
        private bool refreshInProgress;
        private uint newMapId;

        public MapMarkersMapComponent()
        {
            var expectedCount = Service.DataManager.GetExcelSheet<MapSymbol>()!.Count() - 1;
        
            // If we have an empty icon settings object
            if (Settings.IconSettingList.Count == 0)
            {
                foreach (var mapIcon in Service.DataManager.GetExcelSheet<MapSymbol>()!)
                {
                    if(mapIcon.Icon == 0) continue;
                
                    Settings.IconSettingList.Add((uint)mapIcon.Icon, new Setting<IconSelection>(new IconSelection((uint)mapIcon.Icon, true)));
                    Service.Configuration.Save();
                }
            }
        
            // If the datasheet contains more elements than we have
            else if (Settings.IconSettingList.Count != expectedCount)
            {
                PluginLog.Warning("Mismatched number of MapMarkers, attempting to load new markers.");
            
                var startPoint = Settings.IconSettingList.Count;
                var difference = expectedCount - startPoint;
                foreach (var index in Enumerable.Range(startPoint, difference))
                {
                    PluginLog.Warning($"Attempting to add: [{index}]");
                
                    var newEntry = Service.DataManager.GetExcelSheet<MapSymbol>()!.GetRow((uint)index);
                    if (newEntry is not null)
                    {
                        PluginLog.Warning($"Adding [{newEntry.PlaceName.Value?.Name ?? "Unknown Name"}] [IconID: {newEntry.Icon}");
                        Settings.IconSettingList.Add((uint)newEntry.Icon, new Setting<IconSelection>(new IconSelection((uint)newEntry.Icon, true)));
                        Service.Configuration.Save();
                    }
                }
            }
        }
    
        public void Update(uint mapID)
        {
            newMapId = mapID;
            dataStale = true;
        }

        public void Draw()
        {
            if (Settings.AetherytesOnTop.Value)
            {
                DrawMarkers(mapMarkers.Where(marker => marker.IconId != 60453));
                DrawMarkers(mapMarkers.Where(marker => marker.IconId == 60453));
            }
            else
            {
                DrawMarkers(mapMarkers);
            }

            if (dataStale && !refreshInProgress)
            {
                mapMarkers.Clear();
                Task.Run(LoadMarkers);
                refreshInProgress = true;
            }
        }

        private void DrawMarkers(IEnumerable<MapMarkerData> markers)
        {
            foreach (var marker in markers.TakeWhile(_ => !dataStale && Settings.Enable.Value))
            {
                if (Settings.IconSettingList.TryGetValue(marker.IconId, out var settings))
                {
                    if (settings.Value.Enabled)
                    {
                        marker.Draw();
                    }
                }
                else
                {
                    marker.Draw();
                }
            }
        }

        private void LoadMarkers()
        {
            var map = Service.Cache.MapCache.GetRow(newMapId);
        
            foreach (var row in Service.DataManager.GetExcelSheet<MapMarker>()!)
            {
                if (row.RowId == map.MapMarkerRange)
                {
                    mapMarkers.Add(new MapMarkerData(row));
                }
            }
        
            dataStale = false;
            refreshInProgress = false;
        }
    }
    
    private class MapMarkerOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.MapMarker;
        public void DrawSettings()
        {
            InfoBox.Instance
                .AddTitle(Strings.Configuration.FeatureToggles)
                .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
                .AddConfigCheckbox(Strings.Map.Markers.AetherytesOnTop, Settings.AetherytesOnTop)
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
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.IconSelect)
                .AddString(Strings.Map.Info)
                .AddDummy(8.0f)
                .BeginFlexGrid()
                .MultiSelect(Settings.IconSettingList.Values)
                .EndFlexGrid()
                .Draw();
        }
    }
}

