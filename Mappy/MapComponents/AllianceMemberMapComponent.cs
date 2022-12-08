using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;



public class AllianceMemberSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<uint> SelectedIcon = new((uint) AllianceMarkers.Green);
    public Setting<float> IconScale = new(0.50f);
    public Setting<Vector4> TooltipColor = new(Colors.ForestGreen);
}

public class AllianceMemberMapComponent : IMapComponent
{
    private static AllianceMemberSettings Settings => Service.Configuration.AllianceSettings;
    
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
        if (!Settings.Enable.Value) return;
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
                if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(Settings.SelectedIcon.Value, player, Settings.IconScale.Value);
                if(Settings.ShowTooltip.Value) MapRenderer.DrawTooltip(player.Name.TextValue, Settings.TooltipColor.Value);
            }
        }
    }
}