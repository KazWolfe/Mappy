using System;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class FateMapComponent : IMapComponent
{
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
         var icon = Service.Cache.IconCache.GetIconTexture(fate.IconId);
         var position = Service.MapManager.GetObjectPosition(fate.Location);   
             
         DrawRing(fate);
         MapRenderer.DrawIcon(icon, position);
         DrawTooltip(fate);
    }

    private void DrawRing(FateContext fate)
    {
        switch (fate.State)
        {
            case 2:
                var position = Service.MapManager.GetObjectPosition(fate.Location);
                var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(position);

                var radius = fate.Radius * MapRenderer.Viewport.Scale;
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