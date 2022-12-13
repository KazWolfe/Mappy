using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.Localization;
using Mappy.Modules;
using Mappy.System;
using Mappy.Utilities;

namespace Mappy.DataModels;

public enum QuestType
{
    Unknown,
    Regular,
    Repeatable,
    FeatureUnlock,
    MainStory
}

public class QuestData
{
    private static QuestSettings Settings => Service.Configuration.QuestMarkers;
    
    private readonly Quest internalQuestData;

    public QuestType Type { get; }
    public uint AdjustedQuestID => internalQuestData.RowId - 65536;
    public uint IconID => GetIconForType();
    
    public QuestData(Quest quest)
    {
        internalQuestData = quest;
        
        var journalEntry = Service.DataManager.GetExcelSheet<CompleteJournal>()!.FirstOrDefault(entry =>
            entry.Unknown0 == AdjustedQuestID && entry.Name.RawString == quest.Name.RawString);

        Type = journalEntry?.Icon switch
        {
            61411 => QuestType.Regular,
            >= 062301 and <= 062420 => QuestType.Regular,
            61419 => QuestType.FeatureUnlock,
            61413 => QuestType.Repeatable,
            >= 61401 and <= 61403 => QuestType.FeatureUnlock,
            61412 => QuestType.MainStory,
            _ => QuestType.Unknown
        };

        if (Type == QuestType.Unknown)
        {
            PluginLog.Warning($"Unknown QuestIcon found: {journalEntry?.Icon}");
        }
    }

    public void Draw()
    {
        if (Settings.HiddenQuests.Contains(AdjustedQuestID)) return;
        if (Settings.HideRepeatable.Value && Type == QuestType.Repeatable) return;
        
        DrawIcon();
        DrawTooltip();
        DrawContextMenu();
    }

    private void DrawIcon()
    {
        if (internalQuestData.IssuerLocation is {Value: { } levelInfo})
        {
            var position = Service.MapManager.GetTextureOffsetPosition(new Vector2(levelInfo.X, levelInfo.Z));
            
            MapRenderer.DrawIcon(IconID, position, Settings.UnacceptedScale.Value);
        }
    }

    private void DrawTooltip()
    {
        if (!Settings.ShowTooltip.Value) return;
        if (!ImGui.IsItemHovered()) return;
        
        ImGui.BeginTooltip();
        ImGui.TextColored(Settings.UnacceptedTooltipColor.Value,
            $"{Strings.Map.Fate.Level} {internalQuestData.ClassJobLevel0} {internalQuestData.Name.ToDalamudString().TextValue}");
        
        ImGui.EndTooltip();
    }

    private void DrawContextMenu()
    {
        if (!ImGui.IsItemClicked(ImGuiMouseButton.Right)) return;
        
        Service.ContextMenu.Show(ContextMenuType.Quest, AdjustedQuestID);
    }

    private uint GetIconForType()
    {
        return Type switch
        {
            QuestType.Regular => 61411,
            QuestType.FeatureUnlock => 61419,
            QuestType.MainStory => 61412,
            QuestType.Repeatable => 61413,
            _ => 61421
        };
    }
}