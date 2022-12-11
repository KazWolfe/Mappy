using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.System;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class TemporaryMarkerSettings
{
    public Setting<float> FlagScale = new(0.5f);
    public Setting<float> GatheringScale = new(0.5f);
    public Setting<Vector4> GatheringColor = new(Colors.Blue with {W = 0.33f});
    public Setting<Vector4> TooltipColor = new(Colors.White);
}

public enum MarkerType
{
    Unknown,
    Flag,
    Gathering
}

public class TemporaryMarker
{
    public MarkerType Type { get; set; } = MarkerType.Unknown;
    public uint MapID { get; set; } = 0;
    public uint IconID { get; set; } = 0;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Radius { get; set; } = 0.0f;
    public string TooltipText { get; set; } = string.Empty;

    public Vector2 AdjustedPosition => Service.MapManager.GetTextureOffsetPosition(Position);
}

public class TemporaryMarkersMapComponent : IMapComponent
{
    private static TemporaryMarkerSettings Settings => Service.Configuration.TemporaryMarkers;
    
    private static readonly List<TemporaryMarker> TemporaryMarkers = new();
    private static readonly List<TemporaryMarker> StaleMarkers = new();
    public static TemporaryMarker? TempMarker;
    private static bool _dataStale;
    
    public void Update(uint mapID)
    {
        
    }

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
        ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 35, 4);
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