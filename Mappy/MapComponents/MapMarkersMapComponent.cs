using System.Collections.Generic;
using System.Linq;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;

namespace Mappy.MapComponents;

public class MapMarkersMapComponent : IMapComponent
{
    private readonly List<MapMarkerData> mapMarkers = new();

    private bool dataStale;
    private uint newMapId;
    
    public void Update(uint mapID)
    {
        newMapId = mapID;
        dataStale = true;
    }

    public void Draw()
    {
        foreach (var marker in mapMarkers.TakeWhile(_ => !dataStale))
        {
            marker.Draw();
        }

        if (dataStale)
        {
            LoadMarkers();
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

        PluginLog.Debug($"Loaded Markers for MapID: {newMapId}\n" +
                        $"Marker Range: {map.MapMarkerRange}\n" +
                        $"Marker Count: {mapMarkers.Count}");

        dataStale = false;
    }
}