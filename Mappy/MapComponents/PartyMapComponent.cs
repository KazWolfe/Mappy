using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PartyMapComponent : IMapComponent
{
    private readonly HashSet<uint> allianceRaidTerritories;

    private bool enableAllianceChecking;

    public PartyMapComponent()
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
        
        DrawPlayers();
        DrawPets();
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
                var playerPosition = Service.MapManager.GetObjectPosition(player.Position);
                var icon = Service.Cache.IconCache.GetIconTexture(60358);

                MapRenderer.DrawIcon(icon, playerPosition);
                DrawTooltip(player.Name.TextValue, Colors.ForestGreen);
            }
        }
    }

    private void DrawPlayers()
    {
        foreach (var player in Service.PartyList)
        {
            var playerPosition = Service.MapManager.GetObjectPosition(player.Position);
            var icon = Service.Cache.IconCache.GetIconTexture(60421);

            MapRenderer.DrawIcon(icon, playerPosition);
            DrawTooltip(player.Name.TextValue, Colors.Blue);
        }
    }
    
    private void DrawPets()
    {
        if (Service.PartyList.Length == 0)
        {
            if (Service.ClientState.LocalPlayer is { } localPlayer)
            {
                DrawPet(localPlayer.ObjectId);
            }
        }
        else
        {
            foreach (var partyMember in Service.PartyList)
            {
                DrawPet(partyMember.ObjectId);
            }
        }
    }

    private void DrawPet(uint ownerID)
    {
        foreach (var obj in OwnedPets(ownerID))
        {
            var petPosition = Service.MapManager.GetObjectPosition(obj.Position);
            var icon = Service.Cache.IconCache.GetIconTexture(60961);
                    
            MapRenderer.DrawIcon(icon, petPosition);
            DrawTooltip(obj.Name.TextValue, Colors.Purple);
        }
    }

    private void DrawTooltip(string playerName, Vector4 color)
    {
        if (!ImGui.IsItemHovered()) return;
        
        Utilities.Draw.DrawTooltip(playerName, color);
    }

    private IEnumerable<GameObject> OwnedPets(uint objectID)
    {
        var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == objectID);
        
        return ownedObjects.Where(obj => obj.ObjectKind == ObjectKind.BattleNpc && IsPetOrChocobo(obj));
    }

    private static bool IsPetOrChocobo(GameObject gameObject)
    {
        var battleNpc = gameObject as BattleNpc;

        return  (BattleNpcSubKind?)battleNpc?.SubKind switch
        {
            BattleNpcSubKind.Chocobo => true,
            BattleNpcSubKind.Enemy => false,
            BattleNpcSubKind.None => false,
            BattleNpcSubKind.Pet => true,
            _ => false
        };
    }
}