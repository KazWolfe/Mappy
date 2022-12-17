using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.System;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class GatheringAreaSettings
{
    public Setting<float> IconScale = new(0.5f);
    public Setting<Vector4> RingColor = new(Colors.Blue with {W = 0.33f});
    public Setting<Vector4> TooltipColor = new(Colors.White);
}

public class GatheringAreaMarker : IModule
{
    private static GatheringAreaSettings Settings => Service.Configuration.GatheringArea;

    public IMapComponent MapComponent { get; } = new GatheringAreaMapComponent();
    public IModuleSettings Options { get; } = new GatheringAreaOptions();

    public static void SetGatheringArea(TempMarker marker) => GatheringAreaMapComponent.SetGatheringAreaInternal(marker);
    public static void ClearGatheringArea() => GatheringAreaMapComponent.ClearGatheringAreaInternal();
    public static TempMarker? GetGatheringArea() => GatheringAreaMapComponent.GetGatheringAreaInternal();
    
    private class GatheringAreaMapComponent : IMapComponent
    {
        private static TempMarker? _gatheringArea;
        
        public void Draw()
        {
            if (_gatheringArea is null) return;
            if (_gatheringArea.MapID != Service.MapManager.LoadedMapId) return;
            
            DrawRing();
            MapRenderer.DrawIcon(_gatheringArea.IconID, _gatheringArea.AdjustedPosition, Settings.IconScale.Value);
            DrawTooltip();
            ShowContextMenu();
        }
        
        private void ShowContextMenu()
        {
            if (!ImGui.IsItemClicked(ImGuiMouseButton.Right)) return;
            Service.ContextMenu.Show(ContextMenuType.GatheringArea);
        }
        
        private void DrawRing()
        {
            if (_gatheringArea is null) return;
            
            var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(_gatheringArea.AdjustedPosition);

            var radius = _gatheringArea.Radius * MapRenderer.Viewport.Scale;
            var color = ImGui.GetColorU32(Settings.RingColor.Value);
        
            ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, color);
            ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 0, 4);
        }
        
        private void DrawTooltip()
        {
            if (_gatheringArea is null) return;
            if (_gatheringArea.TooltipText == string.Empty) return;
            if (!ImGui.IsItemHovered()) return;
        
            ImGui.BeginTooltip();
            ImGui.TextColored(Settings.TooltipColor.Value, _gatheringArea.TooltipText);
            ImGui.EndTooltip();
        }
        
        public static TempMarker? GetGatheringAreaInternal() => _gatheringArea;
        public static void SetGatheringAreaInternal(TempMarker marker) => _gatheringArea = marker;
        public static void ClearGatheringAreaInternal() => _gatheringArea = null;
    }
    
    private class GatheringAreaOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.GatheringArea;
        public void DrawSettings()
        {
            InfoBox.Instance
                .AddTitle(Strings.Configuration.ColorOptions)
                .AddConfigColor(Strings.Map.TemporaryMarkers.GatheringColor, Settings.RingColor, Colors.Blue with {W = 0.33f})
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