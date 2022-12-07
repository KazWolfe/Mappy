﻿using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PartyMapComponent : IMapComponent
{
    public void Update(uint mapID)
    {
        
    }

    public void Draw()
    {
        DrawPlayers();
        DrawPets();
    }

    private void DrawPets()
    {
        if (Service.PartyList.Length == 0)
        {
            if (Service.ClientState.LocalPlayer is { } localPlayer)
            {
                foreach (var obj in OwnedPets(localPlayer.ObjectId))
                {
                    var petPosition = Service.MapManager.GetObjectPosition(obj.Position);
                    var icon = Service.Cache.IconCache.GetIconTexture(60961);
                    
                    MapRenderer.DrawIcon(icon, petPosition, 1.5f);
                    DrawTooltip(obj.Name.TextValue);
                }
            }
        }
        else
        {
            foreach (var partyMember in Service.PartyList)
            {
                foreach (var obj in OwnedPets(partyMember.ObjectId))
                {
                    var petPosition = Service.MapManager.GetObjectPosition(obj.Position);
                    var icon = Service.Cache.IconCache.GetIconTexture(60961);
                    
                    MapRenderer.DrawIcon(icon, petPosition, 1.5f);
                    DrawTooltip(obj.Name.TextValue);
                }
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
            DrawTooltip(player.Name.TextValue);
        }
    }
    
    private void DrawTooltip(string playerName)
    {
        if (!ImGui.IsItemHovered()) return;
        
        Utilities.Draw.DrawTooltip(playerName);
    }

    private IEnumerable<GameObject> OwnedPets(uint objectID)
    {
        var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == objectID);
        
        return ownedObjects.Where(obj => obj.ObjectKind == ObjectKind.BattleNpc && IsPetOrChocobo(obj));
    }

    private static bool IsPetOrChocobo(GameObject gameObject)
    {
        var battleNpc = gameObject as BattleNpc;

        return battleNpc?.SubKind switch
        {
            (byte) BattleNpcSubKind.Chocobo => true,
            (byte) BattleNpcSubKind.Enemy => false,
            (byte) BattleNpcSubKind.None => false,
            (byte) BattleNpcSubKind.Pet => true,
            _ => false
        };
    }
}