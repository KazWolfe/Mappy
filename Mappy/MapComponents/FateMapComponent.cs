﻿using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class FateSettings
{
    public Setting<bool> ShowRing = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<bool> ShowIcon = new(true);

    public Setting<Vector4> Color = new(Colors.FatePink);
}

public class FateMapComponent : IMapComponent
{
    private static FateSettings Settings => Service.Configuration.FateSettings;
    
    public void Update(uint mapID)
    {
        
    }

    public unsafe void Draw()
    {
        if (!Service.MapManager.PlayerInCurrentMap) return;
        
        var fateManager = FateManager.Instance()->Fates;

        foreach (var fate in fateManager.Span)
        {
            if (fate.Value is null) continue;
            DrawFate(*fate.Value);
        }
    }
    
     private void DrawFate(FateContext fate)
     { 
         var position = Service.MapManager.GetObjectPosition(fate.Location);   
             
         if(Settings.ShowRing.Value) DrawRing(fate);
         if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(fate.IconId, position);
         if(Settings.ShowTooltip.Value) DrawTooltip(fate);
    }

    private void DrawRing(FateContext fate)
    {
        switch (fate.State)
        {
            case 2:
                var position = Service.MapManager.GetObjectPosition(fate.Location);
                var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(position);

                var radius = fate.Radius * MapRenderer.Viewport.Scale;
                var color = ImGui.GetColorU32(Settings.Color.Value);
                
                ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, color);
                ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 35, 4);
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