using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;
using FieldMarker = Lumina.Excel.GeneratedSheets.FieldMarker;

namespace Mappy.MapComponents;

public class WaymarkSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<float> IconScale = new(0.5f);
}

public class WaymarkMapComponent : IMapComponent
{
    private static WaymarkSettings Settings => Service.Configuration.Waymarks;

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