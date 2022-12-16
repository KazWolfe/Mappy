using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class FateSettings
{
    public Setting<bool> Enable = new(true);
    
    public Setting<bool> ShowRing = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<float> IconScale = new(0.5f);
    public Setting<bool> ExpiringWarning = new(false);
    public Setting<int> EarlyWarningTime = new(300);

    public Setting<Vector4> Color = new(Colors.FatePink);
    public Setting<Vector4> TooltipColor = new(Colors.White);
    public Setting<Vector4> ExpiringColor = new(Colors.SoftRed with {W = 0.33f});
}

public class Fates : IModule
{
    private static FateSettings Settings => Service.Configuration.FateSettings;
    public IMapComponent MapComponent { get; }
    public IModuleSettings Options { get; }

    public Fates()
    {
        MapComponent = new FateMapComponent();
        Options = new FateOptions();
    }
    
    private class FateMapComponent : IMapComponent
    {
        public unsafe void Draw()
        {
            if (!Service.MapManager.PlayerInCurrentMap) return;
            if (!Settings.Enable.Value) return;

            foreach (var fate in FateManager.Instance()->Fates.Span)
            {
                if (fate.Value is null) continue;
                DrawFate(*fate.Value);
            }
        }

        private void DrawFate(FateContext fate)
        {
            var position = Service.MapManager.GetObjectPosition(fate.Location);

            if (Settings.ShowRing.Value) DrawRing(fate);
            if (Settings.ShowIcon.Value) MapRenderer.DrawIcon(fate.IconId, position, Settings.IconScale.Value);
            if (Settings.ShowTooltip.Value) DrawTooltip(fate);
        }

        private void DrawRing(FateContext fate)
        {
            var timeRemaining = GetTimeRemaining(fate);
            var earlyWarningTime = TimeSpan.FromSeconds(Settings.EarlyWarningTime.Value);
            var color = ImGui.GetColorU32(Settings.Color.Value);

            if (Settings.ExpiringWarning.Value && timeRemaining <= earlyWarningTime)
            {
                color = ImGui.GetColorU32(Settings.ExpiringColor.Value);
            }

            switch (fate.State)
            {
                case 2:
                    var position = Service.MapManager.GetObjectPosition(fate.Location);
                    var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(position);

                    var radius = fate.Radius * MapRenderer.Viewport.Scale;

                    ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, color);
                    ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 0, 4);
                    break;
            }
        }

        private void DrawTooltip(FateContext fate)
        {
            if (!ImGui.IsItemHovered()) return;

            ImGui.BeginTooltip();

            switch (fate.State)
            {
                case 2:
                    var remainingTime = GetTimeFormatted(GetTimeRemaining(fate));

                    ImGui.TextColored(Settings.TooltipColor.Value,
                        $"{Strings.Map.Fate.Level} {fate.Level} {fate.Name}\n" +
                        $"{Strings.Map.Fate.TimeRemaining}: {remainingTime}\n" +
                        $"{Strings.Map.Fate.Progress}: {fate.Progress}%%");
                    break;

                case 7:
                    ImGui.TextColored(Settings.TooltipColor.Value,
                        $"{Strings.Map.Fate.Level} {fate.Level} {fate.Name}");
                    break;
            }

            ImGui.EndTooltip();
        }

        private TimeSpan GetTimeRemaining(FateContext fate)
        {
            var now = DateTime.UtcNow;
            var start = DateTimeOffset.FromUnixTimeSeconds(fate.StartTimeEpoch).UtcDateTime;
            var duration = TimeSpan.FromSeconds(fate.Duration);

            var delta = duration - (now - start);

            return delta;
        }

        private string GetTimeFormatted(TimeSpan span)
        {
            return $"{span.Minutes:D2}:{span.Seconds:D2}";
        }
    }
    
    private class FateOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.Fate;
    
        public void DrawSettings()
        {
            InfoBox.Instance
                .AddTitle(Strings.Configuration.FeatureToggles)
                .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
                .AddDummy(8.0f)
                .AddConfigCheckbox(Strings.Map.Generic.ShowIcon, Settings.ShowIcon)
                .AddConfigCheckbox(Strings.Map.Generic.ShowTooltip, Settings.ShowTooltip)
                .AddConfigCheckbox(Strings.Map.Fate.ShowRing, Settings.ShowRing)
                .AddDummy(8.0f)
                .AddConfigCheckbox(Strings.Map.Fate.ExpiringWarning, Settings.ExpiringWarning, Strings.Map.Fate.ExpirationWarningHelp)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.ColorOptions)
                .AddConfigColor(Strings.Map.Fate.Color, Settings.Color, Colors.FatePink)
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.White)
                .AddConfigColor(Strings.Map.Fate.ExpirationColor, Settings.ExpiringColor, Colors.SoftRed with { W = 0.33f })
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddInputInt(Strings.Map.Fate.EarlyWarningTime, Settings.EarlyWarningTime, 10, 3600, 0, 0, InfoBox.Instance.InnerWidth / 2.0f)
                .AddHelpMarker(Strings.Map.Fate.InSeconds)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.IconScale.Value = 0.50f;
                    Settings.EarlyWarningTime.Value = 300;
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        }
    }
}


