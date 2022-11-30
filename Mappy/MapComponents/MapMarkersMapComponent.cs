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

    public void Draw()
    {
        if (!MapData.DataAvailable) return;
        
        foreach (var marker in mapMarkers)
        {
            DrawMapMarker(marker);
        }
    }
    
    private void DrawMapMarker(MapMarker marker)
    {
        var icon = Service.IconManager.GetIconTexture(marker.Icon);

        if (icon is not null)
        {
            var iconSize = new Vector2(icon.Width, icon.Height);

            var markerPosition = MapData.GetScaledPosition(new Vector2(marker.X, marker.Y)) - iconSize / 2.0f;

            // if (marker.PlaceNameSubtext.Value is { } placeName && viewport.Scale > 0.5f)
            // {
            //     var stringSize = new Vector2(ImGui.CalcTextSize(placeName.Name).X, 0.0f) * viewport.Scale;
            //     var textOffset = new Vector2(0.0f, -20.0f);
            //     
            //     ImGui.SetCursorPos(-viewportPosition + markerCenter - stringSize);
            //     ImGui.TextColored(Colors.Black, placeName.Name.ToDalamudString().TextValue);
            // }
            
            MapData.SetDrawPosition(markerPosition);
            ImGui.Image(icon.ImGuiHandle, iconSize);
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