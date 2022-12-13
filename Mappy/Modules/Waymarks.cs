using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;
using FieldMarker = Lumina.Excel.GeneratedSheets.FieldMarker;

namespace Mappy.Modules;

public class WaymarkSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<float> IconScale = new(0.5f);
}

public class Waymarks : IModule
{
    private static WaymarkSettings Settings => Service.Configuration.Waymarks;
    public IMapComponent MapComponent { get; } = new WaymarkMapComponent();
    public IModuleSettings Options { get; } = new WaymarkOptions();
    private class WaymarkMapComponent : IMapComponent
    {

        private readonly List<FieldMarker> fieldMarkers;
    
        public WaymarkMapComponent()
        {
            fieldMarkers = Service.DataManager.GetExcelSheet<FieldMarker>()!.Where(row => row.RowId is >= 1u and <= 8u).ToList();
        }

        public void Update(uint mapID)
        {
        
        }

        public unsafe void Draw()
        {
            if (!Settings.Enable.Value) return;
            if (!Service.MapManager.PlayerInCurrentMap) return;

            var markerSpan = MarkingController.Instance()->FieldMarkerSpan;

            foreach (var index in Enumerable.Range(0, 8))
            {
                if (markerSpan[index] is { Active: true } marker)
                {
                    var position = Service.MapManager.GetObjectPosition(marker.Position);
                    
                    MapRenderer.DrawIcon(GetIconForMarkerIndex(index), position, Settings.IconScale.Value);
                }
            }
        }

        private uint GetIconForMarkerIndex(int index) => fieldMarkers[index].MapIcon;
    }

    private class WaymarkOptions : IModuleSettings
    {
    
        public ComponentName ComponentName => ComponentName.Waymark;
        public void DrawSettings()
        {
            InfoBox.Instance
                .AddTitle(Strings.Configuration.FeatureToggles)
                .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.IconScale.Value = 0.5f;
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        }
    }
}

