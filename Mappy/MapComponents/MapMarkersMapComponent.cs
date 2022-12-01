using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;

namespace Mappy.MapComponents;

public class MapMarkersMapComponent
{
    private static MapData MapData => Service.MapManager.MapData;

    private readonly List<MapMarker> mapMarkers = new();
    private bool reloadMarkers;

    public void Draw()
    {
        if (!MapData.DataAvailable) return;
        
        foreach (var marker in mapMarkers)
        {
            DrawMapMarker(marker);
        }

        if (reloadMarkers)
        {
            LoadMarkers();
        }
    }
    
    private void DrawMapMarker(MapMarker marker)
    {
        var icon = Service.IconManager.GetIconTexture(marker.Icon);

        if (icon is not null)
        {
            var iconSize = new Vector2(icon.Width, icon.Height);

            var markerPosition = MapData.GetScaledPosition(new Vector2(marker.X, marker.Y)) - iconSize / 2.0f;
            
            MapData.SetDrawPosition(markerPosition);
            ImGui.Image(icon.ImGuiHandle, iconSize);

            DrawTooltip(marker);
            CheckClick(marker);
        }
    }

    private void CheckClick(MapMarker marker)
    {
        if (!ImGui.IsItemClicked()) return;
        
        switch (marker.DataType)
        {
            case 1:
                ChangeMapView(marker);
                break;
            
            case 3:
                DoTeleport(marker);
                break;
        }
    }

    private void DoTeleport(MapMarker marker)
    {
        var targetAetherite = Service.DataManager.GetExcelSheet<Aetheryte>()!.GetRow(marker.DataKey);
        if (targetAetherite is null) return;
        
        Service.Teleporter.Teleport(targetAetherite);
    }

    private void ChangeMapView(MapMarker marker)
    {
        var targetMap = Service.DataManager.GetExcelSheet<Map>()!.GetRow(marker.DataKey);
        if (targetMap is null) return;
        
        var mapKey = targetMap.Id.RawString;
        
        MapData.LoadMapWithKey(mapKey);
        reloadMarkers = true;
    }

    private void DrawTooltip(MapMarker marker)
    {
        switch (marker.DataType)
        {
            case 0:
            case 1:
                DrawRegularTooltip(marker);
                break;
            
            case 3:
                DrawAetheriteTooltip(marker);
                break;
        }
    }

    private void DrawAetheriteTooltip(MapMarker marker)
    {
        if (!ImGui.IsItemHovered()) return;
        
        var markerPlaceName = Service.DataManager.GetExcelSheet<Aetheryte>()!.GetRow(marker.DataKey)!.PlaceName.Value;

        if (markerPlaceName is not null && markerPlaceName.RowId != 0)
        {
            ImGui.BeginTooltip();
            ImGui.Text(markerPlaceName.Name);
            ImGui.EndTooltip();
        }
    }

    private void DrawRegularTooltip(MapMarker marker)
    {
        if (!ImGui.IsItemHovered()) return;
        
        var markerPlaceName = marker.PlaceNameSubtext.Value;

        if (markerPlaceName is not null && markerPlaceName.RowId != 0)
        {
            ImGui.BeginTooltip();
            ImGui.Text(markerPlaceName.Name);
            ImGui.EndTooltip();
        }
    }

    public void LoadMarkers()
    {
        mapMarkers.Clear();

        if (!MapData.DataAvailable) return;
        
        foreach (var row in Service.DataManager.GetExcelSheet<MapMarker>()!)
        {
            if (row.RowId == MapData.Map.MapMarkerRange)
            {
                mapMarkers.Add(row);
            }
        }
    }
}