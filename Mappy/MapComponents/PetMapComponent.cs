using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PetMapComponent : IMapComponent
{
    public void Update(uint mapID)
    {
        
    }

    public void Draw()
    {
        if (!Service.MapManager.PlayerInCurrentMap) return;

        DrawPets();
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
            MapRenderer.DrawIcon(60961, obj);
            MapRenderer.DrawTooltip(obj.Name.TextValue, Colors.Purple);
        }
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