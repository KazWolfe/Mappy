using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.System;
using Mappy.Utilities;
using static Mappy.UserInterface.Components.InfoBox;

namespace Mappy.Modules;

public class TemporaryMarkerSettings
{
    public Setting<float> FlagScale = new(0.5f);
    public Setting<float> GatheringScale = new(0.5f);
    public Setting<Vector4> GatheringColor = new(Colors.Blue with {W = 0.33f});
    public Setting<Vector4> TooltipColor = new(Colors.White);
}

public class TemporaryMarkers : IModule
{
    private static TemporaryMarkerSettings Settings => Service.Configuration.TemporaryMarkers;
    public IMapComponent MapComponent { get; } = new TemporaryMarkersMapComponent();
    public IModuleSettings Options { get; } = new TemporaryMarkerOptions();
    public class TemporaryMarkersMapComponent : IMapComponent
    {
        private static readonly List<TemporaryMarker> TemporaryMarkers = new();
        private static readonly List<TemporaryMarker> StaleMarkers = new();
        public static TemporaryMarker? TempMarker;
        private static bool _dataStale;
        
        public void Draw()
        {
            foreach (var marker in TemporaryMarkers.TakeWhile(_ => !_dataStale))
            {
                if (Service.MapManager.LoadedMapId == marker.MapID)
                {
                    DrawIcon(marker);
                    ShowContextMenu(marker);
                    ShowTooltip(marker)?.Invoke(marker);
                }
            }

            if (StaleMarkers.Count > 0)
            {
                foreach (var staleMarker in StaleMarkers)
                {
                    TemporaryMarkers.Remove(staleMarker);
                }
            
                StaleMarkers.Clear();
                _dataStale = false;
            }
        }

        private Action<TemporaryMarker>? ShowTooltip(TemporaryMarker marker)
        {
            return marker.Type switch
            {
                MarkerType.Flag => null,
                MarkerType.Gathering => GatheringMarkerTooltip,
                _ => null
            };
        }

        private void GatheringMarkerTooltip(TemporaryMarker marker)
        {
            if (marker.TooltipText == string.Empty) return;
            if (!ImGui.IsItemHovered()) return;
        
            ImGui.BeginTooltip();
            ImGui.TextColored(Settings.TooltipColor.Value, marker.TooltipText);
            ImGui.EndTooltip();
        }
    
        private void DrawIcon(TemporaryMarker marker)
        {
            switch (marker.Type)
            {
                case MarkerType.Flag:
                    MapRenderer.DrawIcon(marker.IconID, marker.AdjustedPosition, Settings.FlagScale.Value);
                    break;
            
                case MarkerType.Gathering:
                    DrawRing(marker);
                    MapRenderer.DrawIcon(marker.IconID, marker.AdjustedPosition, Settings.GatheringScale.Value);
                    break;
            }
        }

        public static void AddMarker(TemporaryMarker marker)
        {
            TemporaryMarkers.RemoveAll(mapMarker => mapMarker.Type == marker.Type);
            TemporaryMarkers.Add(marker);
        
            PluginLog.Debug($"Adding Temporary Marker. Count: {TemporaryMarkers.Count}");
        }

        private void DrawRing(TemporaryMarker marker)
        {
            var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(marker.AdjustedPosition);

            var radius = marker.Radius * MapRenderer.Viewport.Scale;
            var color = ImGui.GetColorU32(Settings.GatheringColor.Value);
        
            ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, color);
            ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 0, 4);
        }

        private void ShowContextMenu(TemporaryMarker marker)
        {
            if (!ImGui.IsItemClicked(ImGuiMouseButton.Right)) return;

            switch (marker.Type)
            {
                case MarkerType.Flag:
                    Service.ContextMenu.Show(ContextMenuType.Flag);
                    break;
            
                case MarkerType.Gathering:
                    Service.ContextMenu.Show(ContextMenuType.GatheringArea);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void RemoveFlag()
        {
            var flag = TemporaryMarkers.FirstOrDefault(marker => marker.Type == MarkerType.Flag);

            if (flag is not null)
            {
                StaleMarkers.Add(flag);
                _dataStale = true;
            }
        }

        public static void RemoveGatheringArea()
        {
            var flag = TemporaryMarkers.FirstOrDefault(marker => marker.Type == MarkerType.Gathering);

            if (flag is not null)
            {
                StaleMarkers.Add(flag);
                _dataStale = true;
            }
        }
    }
    private class TemporaryMarkerOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.TemporaryMarker;
    
        public void DrawSettings()
        {
            Instance
                .AddTitle(Strings.Configuration.Info)
                .AddString(Strings.Map.TemporaryMarkers.About)
                .Draw();
        
            Instance
                .AddTitle(Strings.Configuration.ColorOptions)
                .AddConfigColor(Strings.Map.TemporaryMarkers.GatheringColor, Settings.GatheringColor, Colors.Blue with {W = 0.33f})
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.White)
                .Draw();
        
            Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.TemporaryMarkers.FlagScale, Settings.FlagScale, 0.1f, 5.0f, Instance.InnerWidth / 2.0f)
                .AddDragFloat(Strings.Map.TemporaryMarkers.GatheringAreaScale, Settings.GatheringScale, 0.1f, 5.0f, Instance.InnerWidth / 2.0f)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.FlagScale.Value = 0.50f;
                    Settings.GatheringScale.Value = 0.50f;
                    Service.Configuration.Save();
                }, new Vector2(Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        }
    }
}

