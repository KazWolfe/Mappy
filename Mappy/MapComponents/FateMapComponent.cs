using System;
using System.Numerics;
using Dalamud.Game.ClientState.Fates;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public unsafe class FateMapComponent : IMapComponent
{
    public MapData MapData => Service.MapManager.MapData;

    public void Draw()
    {
        var fateManager = FateManager.Instance()->Fates;

        foreach (var fate in fateManager.Span)
        {
            if (fate.Value is null) continue;
            DrawFate(*fate.Value);
        }
    }

    public void Refresh()
    {
        
    }
    
    private void DrawFate(FateContext fate)
    {
        if (!MapData.PlayerInCurrentMap()) return;
        
        DrawRing(fate);
        MapData.DrawIcon(fate.IconId, fate.Location);
        DrawTooltip(fate);
    }

    private void DrawRing(FateContext fate)
    {
        DebugWindow.AddString(fate.Location.ToString());
        
        switch (fate.State)
        {
            case 2:
                var position = MapData.GetScaledGameObjectPosition(fate.Location);
                var drawPosition = MapData.GetWindowDrawPosition(position);

                var radius = fate.Radius * MapData.Viewport.Scale;
                var fatePink = ImGui.GetColorU32(Colors.FatePink);
                
                ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, fatePink);
                ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, fatePink, 35, 4);
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

                ImGui.Text($"{Strings.Map.Fate.Level} {fate.Level} {fate.Name}\n" +
                           $"{Strings.Map.Fate.TimeRemaining}: {remainingTime}\n" +
                           $"{Strings.Map.Fate.Progress}: {fate.Progress}%%");
                break;
            
            case 7:
                ImGui.Text($"{Strings.Map.Fate.Level} {fate.Level} {fate.Name}");
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