﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class MapMarkersSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> AetherytesOnTop = new(true);
    public Dictionary<uint,Setting<IconSelection>> IconSettings = new();
    public Setting<float> IconScale = new(0.5f);
    public Setting<Vector4> StandardColor = new(Colors.White);
    public Setting<Vector4> MapLink = new(Colors.MapTextBrown);
    public Setting<Vector4> InstanceLink = new(Colors.Orange);
    public Setting<Vector4> Aetheryte = new(Colors.Blue);
    public Setting<Vector4> Aethernet = new(Colors.BabyBlue);
}

public class MapMarkersMapComponent : IMapComponent
{
    private static MapMarkersSettings Settings => Service.Configuration.MapMarkers;
    
    private readonly List<MapMarkerData> mapMarkers = new();

    private bool dataStale;
    private uint newMapId;

    public MapMarkersMapComponent()
    {
        var expectedCount = Service.DataManager.GetExcelSheet<MapSymbol>()!.Count() - 1;
        
        // If we have an empty icon settings object
        if (Settings.IconSettings.Count == 0)
        {
            foreach (var mapIcon in Service.DataManager.GetExcelSheet<MapSymbol>()!)
            {
                if(mapIcon.Icon == 0) continue;
                
                Settings.IconSettings.Add((uint)mapIcon.Icon, new Setting<IconSelection>(new IconSelection((uint)mapIcon.Icon, true)));
                Service.Configuration.Save();
            }
        }
        
        // If the datasheet contains more elements than we have
        else if (Settings.IconSettings.Count != expectedCount)
        {
            PluginLog.Warning("Mismatched number of MapMarkers, attempting to load new markers.");
            
            var startPoint = Settings.IconSettings.Count;
            var difference = expectedCount - startPoint;
            foreach (var index in Enumerable.Range(startPoint, difference))
            {
                PluginLog.Warning($"Attempting to add: [{index}]");
                
                var newEntry = Service.DataManager.GetExcelSheet<MapSymbol>()!.GetRow((uint)index);
                if (newEntry is not null)
                {
                    PluginLog.Warning($"Adding [{newEntry.PlaceName.Value?.Name ?? "Unknown Name"}] [IconID: {newEntry.Icon}");
                    Settings.IconSettings.Add((uint)newEntry.Icon, new Setting<IconSelection>(new IconSelection((uint)newEntry.Icon, true)));
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

        if (dataStale)
        {
            LoadMarkers();
        }
    }

    private void DrawMarkers(IEnumerable<MapMarkerData> markers)
    {
        foreach (var marker in markers.TakeWhile(_ => !dataStale && Settings.Enable.Value))
        {
            if (Settings.IconSettings.TryGetValue(marker.IconId, out var settings) && settings.Value.Enabled)
            {
                marker.Draw();
            }
            else
            {
                marker.Draw();
            }
        }
    }

    private void LoadMarkers()
    {
        mapMarkers.Clear();

        var map = Service.Cache.MapCache.GetRow(newMapId);
        
        foreach (var row in Service.DataManager.GetExcelSheet<MapMarker>()!)
        {
            if (row.RowId == map.MapMarkerRange)
            {
                mapMarkers.Add(new MapMarkerData(row));
            }
        }

        dataStale = false;
    }
}