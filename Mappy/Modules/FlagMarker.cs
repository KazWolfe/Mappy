using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.System;
using Mappy.UserInterface.Components;
using Mappy.Utilities;


namespace Mappy.Modules;

public class FlagMarkerSettings
{
    public Setting<float> IconScale = new(0.5f);
    public Setting<Vector4> TooltipColor = new(Colors.White);
}

public class FlagMarker : IModule
{
    private static FlagMarkerSettings Settings => Service.Configuration.Flag;
    public IMapComponent MapComponent { get; } = new FlagMarkerMapComponent();
    public IModuleSettings Options { get; } = new FlagMarkerOptions();

    public static void SetFlag(TempMarker marker) => FlagMarkerMapComponent.SetFlagInternal(marker);
    public static void ClearFlag() => FlagMarkerMapComponent.ClearFlagInternal();
    public static TempMarker? GetFlag() => FlagMarkerMapComponent.GetFlagInternal();
    
    private unsafe class FlagMarkerMapComponent : IMapComponent
    {
        private static TempMarker? _flagMarker;
        private static byte* FlagSetByte => (byte*) AgentMap.Instance() + 0x59B3;
        private static bool IsFlagSet => *FlagSetByte > 0;
        private static void ClearFlagByte() => *FlagSetByte = 0;
        
        public void Draw()
        {
            if (_flagMarker is null) return;
            if (_flagMarker.MapID != Service.MapManager.LoadedMapId) return;
            
            MapRenderer.DrawIcon(_flagMarker.IconID, _flagMarker.AdjustedPosition, Settings.IconScale.Value);
            ShowContextMenu();
        }

        public static TempMarker? GetFlagInternal() => _flagMarker;
        
        public static void SetFlagInternal(TempMarker marker)
        {
            if (IsFlagSet)
            {
                ClearFlagByte();
            }
            
            _flagMarker = marker;
        }

        public static void ClearFlagInternal()
        {
            ClearFlagByte();
            _flagMarker = null;
        }

        private void ShowContextMenu()
        {
            if (!ImGui.IsItemClicked(ImGuiMouseButton.Right)) return;
            Service.ContextMenu.Show(ContextMenuType.Flag);
        }
    }
    
    private class FlagMarkerOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.FlagMarker;
        
        public void DrawSettings()
        {
            InfoBox.Instance
                .AddTitle(Strings.Configuration.ColorOptions)
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.White)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.TemporaryMarkers.FlagScale, Settings.IconScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.IconScale.Value = 0.50f;
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        }
    }
}