using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.MapComponents;

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
    public float Scale { get; set; } = 0.5f;
    public string TooltipText { get; set; } = string.Empty;

    public Vector2 AdjustedPosition => Service.MapManager.GetTextureOffsetPosition(Position);
}

public class TemporaryMarkersMapComponent : IMapComponent
{
    private static readonly List<TemporaryMarker> TemporaryMarkers = new();
    private static readonly List<TemporaryMarker> StaleMarkers = new();
    public static TemporaryMarker? TempMarker;
    private bool dataStale;
    
    public void Update(uint mapID)
    {
        
    }

    public void Draw()
    {
        foreach (var marker in TemporaryMarkers.TakeWhile(_ => !dataStale))
        {
            if (Service.MapManager.LoadedMapId == marker.MapID)
            {
                DrawIcon(marker);
                ShowContextMenu(marker)?.Invoke(marker);
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
            dataStale = false;
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
        ImGui.TextColored(Colors.White, marker.TooltipText);
        ImGui.EndTooltip();
    }
    
    private void DrawIcon(TemporaryMarker marker)
    {
        switch (marker.Type)
        {
            case MarkerType.Flag:
                MapRenderer.DrawIcon(marker.IconID, marker.AdjustedPosition, marker.Scale);
                break;
            
            case MarkerType.Gathering:
                DrawRing(marker);
                MapRenderer.DrawIcon(marker.IconID, marker.AdjustedPosition, marker.Scale);
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
        var color = ImGui.GetColorU32(Colors.Blue with { W = 0.33f });
        
        ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, color);
        ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 35, 4);
    }

    private Action<TemporaryMarker>? ShowContextMenu(TemporaryMarker marker)
    {
        return marker.Type switch
        {
            MarkerType.Flag => FlagContextMenu,
            MarkerType.Gathering => GatheringContextMenu,
            _ => null
        };
    }
    
    private void FlagContextMenu(TemporaryMarker marker)
    {
        if (!ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            if(ImGui.BeginPopupContextItem($"##{marker.Type}{marker.Position}"))
            {
                if (ImGui.Selectable(Strings.Map.RemoveFlag))
                {
                    StaleMarkers.Add(marker);
                    dataStale = true;
                }
                ImGui.EndPopup();
            }
        }
    }

    private void GatheringContextMenu(TemporaryMarker marker)
    {
        if (!ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            if(ImGui.BeginPopupContextItem($"##{marker.Type}{marker.Position}"))
            {
                if (ImGui.Selectable(Strings.Map.RemoveGatheringArea))
                {
                    StaleMarkers.Add(marker);
                    dataStale = true;
                }
                ImGui.EndPopup();
            }
        }
    }
}