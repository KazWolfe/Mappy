using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using ClientStructGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace Mappy.MapComponents;

public class GatheringPointMapComponent
{
    private static MapData MapData => Service.MapManager.MapData;
    
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
    
    public void Draw()
    {
        DrawGatheringNodes();
    }
    
    private void DrawGatheringNodes()
    {
        foreach (var obj in Service.ObjectTable)
        {
            if(obj.ObjectKind != ObjectKind.GatheringPoint) continue;

            if(!IsTargetable(obj)) continue;
            
            var iconId = GetIconIdForGatheringNode(obj);
            
            if (Service.IconManager.GetIconTexture(iconId) is {} icon)
            {
                DrawGatheringMarker(icon, obj.Position);
            }
        }
    }

    private unsafe bool IsTargetable(GameObject gameObject)
    {
        if (gameObject.Address == IntPtr.Zero) return false;
        
        var csObject = (ClientStructGameObject*)gameObject.Address;
        return csObject->GetIsTargetable();
    }
    
    private void DrawGatheringMarker(TextureWrap icon, Vector3 position)
    {
        if (!MapData.DataAvailable) return;
        
        var iconSize = new Vector2(icon.Width, icon.Height);
        var iconPosition = MapData.GetScaledGameObjectPosition(position) - iconSize / 2.0f;

        MapData.SetDrawPosition(iconPosition);
        ImGui.Image(icon.ImGuiHandle, iconSize);
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