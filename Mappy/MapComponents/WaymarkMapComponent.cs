using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class WaymarkSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<float> IconScale = new(0.5f);
}

public class WaymarkMapComponent : IMapComponent
{
    private static WaymarkSettings Settings => Service.Configuration.Waymarks;
    
    public void Update(uint mapID)
    {
        
    }

    public unsafe void Draw()
    {
        if (!Settings.Enable.Value) return;
        if (!Service.MapManager.PlayerInCurrentMap) return;

        var fieldMarkers = MarkingController.Instance()->FieldMarkerSpan;

        foreach (var index in Enumerable.Range(0, 8))
        {
            if (fieldMarkers[index] is { Active: true } marker)
            {
                var position = Service.MapManager.GetObjectPosition(marker.Position);
                    
                MapRenderer.DrawIcon(GetIconForMarkerIndex(index), position, Settings.IconScale.Value);
            }
        }
    }

    private uint GetIconForMarkerIndex(int index)
    {
        return index switch
        {
            0 => 60474,
            1 => 60475,
            2 => 60476,
            3 => 60936,
            4 => 60931,
            5 => 60932,
            6 => 60933,
            7 => 63904,
            _ => throw new IndexOutOfRangeException()
        };
    }
}