using System;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.Interfaces;
using Mappy.Utilities;
using ClientStructGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace Mappy.MapComponents;

public class GatheringPointMapComponent : IMapComponent
{
    private readonly GatheringPointName mineralDeposit;
    private readonly GatheringPointName rockyOutcrop;
    private readonly GatheringPointName matureTree;
    private readonly GatheringPointName lushVegetation;
    
    public GatheringPointMapComponent()
    {
        mineralDeposit = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(1)!;
        rockyOutcrop = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(2)!;
        matureTree = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(3)!;
        lushVegetation = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(4)!;
    }

    
    public void Update(uint mapID)
    {
        
    }

    public void Draw()
    {
        foreach (var obj in Service.ObjectTable)
        {
            if(obj.ObjectKind != ObjectKind.GatheringPoint) continue;

            if(!IsTargetable(obj)) continue;
            
            var iconId = GetIconIdForGatheringNode(obj);
            var icon = Service.Cache.IconCache.GetIconTexture(iconId);
            var position = Service.MapManager.GetObjectPosition(obj);
            
            MapRenderer.DrawIcon(icon, position);
            DrawTooltip(obj);
        }
    }

    private void DrawTooltip(GameObject gameObject)
    {
        if (!ImGui.IsItemHovered()) return;

        var displayString = gameObject.Name.TextValue;
        
        if (displayString != string.Empty)
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(displayString);
            ImGui.EndTooltip();
        }
    }

    private unsafe bool IsTargetable(GameObject gameObject)
    {
        if (gameObject.Address == IntPtr.Zero) return false;
        
        var csObject = (ClientStructGameObject*)gameObject.Address;
        return csObject->GetIsTargetable();
    }
    
    private uint GetIconIdForGatheringNode(GameObject gatheringNode)
    {
        var iconId = 0u;
        var nodeName = gatheringNode.Name.TextValue.ToLower();
        
        if (nodeName == mineralDeposit.Singular)
        {
            iconId = 60438;
        }
        else if (nodeName == rockyOutcrop.Singular)
        {
            iconId = 60437;
        }
        else if (nodeName == matureTree.Singular)
        {
            iconId = 60433;
        }
        else if (nodeName == lushVegetation.Singular)
        {
            iconId = 60432;
        }

        return iconId;
    }
}