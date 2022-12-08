using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;



public class AllianceMemberSettings
{
    public Setting<bool> ShowIcon = new(true);
    public Setting<uint> SelectedIcon = new((uint) AllianceMarkers.Green);
}

public class AllianceMemberMapComponent : IMapComponent
{
    private readonly HashSet<uint> allianceRaidTerritories;
    private bool enableAllianceChecking;
    
    public AllianceMemberMapComponent()
    {
        allianceRaidTerritories = Service.DataManager.GetExcelSheet<TerritoryType>()
            !.Where(r => r.TerritoryIntendedUse is 8)
            .Select(r => r.RowId)
            .ToHashSet();
    }
    
    public void Update(uint mapID)
    {        
        var map = Service.Cache.MapCache.GetRow(mapID);

        enableAllianceChecking = allianceRaidTerritories.Contains(map.TerritoryType.Row);
    }

    public void Draw()
    {
        if (!Service.MapManager.PlayerInCurrentMap) return;
        if (!enableAllianceChecking) return;
        
        DrawAllianceMembers();
    }
    
    private void DrawAllianceMembers()
    {
        if (!enableAllianceChecking) return;
        
        foreach (var index in Enumerable.Range(0, 16))
        {
            var player = HudAgent.GetAllianceMember(index);

            if (player is not null)
            {
                MapRenderer.DrawIcon(60358, player);
                MapRenderer.DrawTooltip(player.Name.TextValue, Colors.ForestGreen);
            }
        }
    }
}