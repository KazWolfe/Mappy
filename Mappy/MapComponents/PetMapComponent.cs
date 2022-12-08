using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PetSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<float> IconScale = new(0.75f);
    public Setting<Vector4> TooltipColor = new(Colors.Purple);
}

public class PetMapComponent : IMapComponent
{
    private static PetSettings Settings => Service.Configuration.Pet;
    
    public void Update(uint mapID)
    {
        
    }

    public void Draw()
    {
        if (!Settings.Enable.Value) return;
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
            if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(60961, obj, Settings.IconScale.Value);
            if(Settings.ShowTooltip.Value) MapRenderer.DrawTooltip(obj.Name.TextValue, Settings.TooltipColor.Value);
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