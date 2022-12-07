using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class WaymarkMapComponent : IMapComponent
{
    public void Update(uint mapID)
    {
        
    }

    public unsafe void Draw()
    {
        if (!Service.MapManager.PlayerInCurrentMap) return;

        var fieldMarkers = MarkingController.Instance()->FieldMarkerSpan;

        foreach (var index in Enumerable.Range(0, 8))
        {
            if (fieldMarkers[index] is { Active: true } marker)
            {
                var position = Service.MapManager.GetObjectPosition(marker.Position);
                var icon = Service.Cache.IconCache.GetIconTexture(GetIconForMarkerIndex(index));
                    
                MapRenderer.DrawIcon(icon, position);
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
        };
    }
}