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

public class TreasureSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<Vector4> TooltipColor = new(Colors.White);
    public Setting<float> IconScale = new(0.5f);
    public Setting<uint> SelectedIcon = new(60003);
}

public class Treasures : IModule
{
    private static TreasureSettings Settings => Service.Configuration.Treasure;

    public IMapComponent MapComponent { get; } = new TreasureMapComponent();
    public IModuleSettings Options { get; } = new TreasureOptions();
    private class TreasureMapComponent : IMapComponent
    {
        public void Update(uint mapID)
        {
        
        }

        public void Draw()
        {
            if (!Settings.Enable.Value) return;
        
            foreach (var obj in Service.ObjectTable)
            {
                if(obj.ObjectKind != ObjectKind.Treasure) continue;

                if(!IsTargetable(obj)) continue;
            
                if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(Settings.SelectedIcon.Value, obj, Settings.IconScale.Value);
                if(Settings.ShowTooltip.Value) DrawTooltip(obj);
            }
        }

        private void DrawTooltip(GameObject gameObject)
        {
            if (!ImGui.IsItemHovered()) return;
            var displayString = $"{gameObject.Name.TextValue}";
        
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


            if (Service.ClientState.LocalPlayer is { } player)
            {
                if (gameObject.Position.Y <= player.Position.Y + 20.0f &&
                    gameObject.Position.Y >= player.Position.Y - 20.0f)
                {
                                    
                    var csObject = (ClientStructGameObject*)gameObject.Address;
                    return csObject->GetIsTargetable();
                }
            }

            return false;
        }
    }
    private class TreasureOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.Treasure;
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

