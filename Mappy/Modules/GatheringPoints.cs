using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;
using ClientStructGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace Mappy.Modules;

public class GatheringPointSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<Vector4> TooltipColor = new(Colors.White);
    public Setting<float> IconScale = new(0.5f);
}

public class GatheringPoints : IModule
{
    private static GatheringPointSettings Settings => Service.Configuration.GatheringPoints;
    public IMapComponent MapComponent { get; } = new GatheringPointMapComponent();
    public IModuleSettings Options { get; } = new GatheringPointOptions();

    private class GatheringPointMapComponent : IMapComponent
    {
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
    
    private class GatheringPointOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.GatheringPoint;
    
        public void DrawSettings()
        {
            InfoBox.Instance
                .AddTitle(Strings.Configuration.FeatureToggles)
                .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
                .AddDummy(8.0f)
                .AddConfigCheckbox(Strings.Map.Generic.ShowIcon, Settings.ShowIcon)
                .AddConfigCheckbox(Strings.Map.Generic.ShowTooltip, Settings.ShowTooltip)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.ColorOptions)
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.White)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.IconScale.Value = 0.50f;
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        }
    }
}

