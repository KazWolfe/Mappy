using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.Utilities;
using ClientStructGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace Mappy.MapComponents;

public class GatheringPointSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<Vector4> TooltipColor = new(Colors.White);
    public Setting<float> IconScale = new(0.5f);
}

public class GatheringPointMapComponent : IMapComponent
{
    private static GatheringPointSettings Settings => Service.Configuration.GatheringPoints;
    
    public void Update(uint mapID)
    {
        
    }

    public void Draw()
    {
        if (!Settings.Enable.Value) return;
        
        foreach (var obj in Service.ObjectTable)
        {
            if(obj.ObjectKind != ObjectKind.GatheringPoint) continue;

            if(!IsTargetable(obj)) continue;
            
            var iconId = GetIconIdForGatheringNode(obj);
            
            if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(iconId, obj, Settings.IconScale.Value);
            if(Settings.ShowTooltip.Value) DrawTooltip(obj);
        }
    }

    private void DrawTooltip(GameObject gameObject)
    {
        if (!ImGui.IsItemHovered()) return;

        var gatheringPoint = Service.Cache.GatheringPointCache.GetRow(gameObject.DataId);
        var gatheringPointBase = Service.Cache.GatheringPointBaseCache.GetRow(gatheringPoint.GatheringPointBase.Row);
        
        var displayString = $"{Strings.Map.Fate.Level} {gatheringPointBase.GatheringLevel} {gameObject.Name.TextValue}";
        
        if (displayString != string.Empty)
        {
            ImGui.BeginTooltip();
            ImGui.TextColored(Settings.TooltipColor.Value, displayString);
            ImGui.EndTooltip();
        }
    }

    private unsafe bool IsTargetable(GameObject gameObject)
    {
        if (gameObject.Address == IntPtr.Zero) return false;

        var csObject = (ClientStructGameObject*)gameObject.Address;
        return csObject->GetIsTargetable();
    }
    
    private uint GetIconIdForGatheringNode(GameObject gameObject)
    {
        var gatheringPoint = Service.Cache.GatheringPointCache.GetRow(gameObject.DataId);
        var gatheringPointBase = Service.Cache.GatheringPointBaseCache.GetRow(gatheringPoint.GatheringPointBase.Row);

        return gatheringPointBase.GatheringType.Row switch
        {
            0 => 60438,
            1 => 60437,
            2 => 60433,
            3 => 60432,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}