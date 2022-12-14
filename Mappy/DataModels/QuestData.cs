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

public class QuestData
{
    private static QuestSettings Settings => Service.Configuration.QuestMarkers;
    
    private readonly Quest internalQuestData;

    private uint AdjustedQuestID => internalQuestData.RowId - 65536;
    
    private readonly bool isRepeatable;
    private readonly uint iconID;

    private const uint FeatureUnlockIcon = 61419;
    
    public QuestData(Quest quest)
    {
        internalQuestData = quest;

        var journalEntry = Service.DataManager.GetExcelSheet<CompleteJournal>()!.FirstOrDefault(entry =>
            entry.Unknown0 == AdjustedQuestID && entry.Name.RawString == quest.Name.RawString);

        iconID = journalEntry?.Icon switch
        {
            >= 062301 and <= 062420 => 61411, // Special Job Icons
            >= 61401 and <= 61403 => 61411, // Grand Company Icons
            0 => FeatureUnlockIcon, // Special Journal Entries, treat them as Feature Unlocks, since we can only get here from Quest datasheet
            62522 => FeatureUnlockIcon,
            _ => (uint)(journalEntry?.Icon ?? 0),
        };

        isRepeatable = journalEntry?.Icon == 61413;

        if (iconID == 0)
        {
            PluginLog.Warning($"JournalEntry Null: QuestID:{AdjustedQuestID}, QuestName:{quest.Name.ToDalamudString().TextValue}");
        }
    }

    public void Draw()
    {
        if (iconID == 0) return;
        if (Settings.HiddenQuests.Contains(AdjustedQuestID)) return;
        if (Settings.HideRepeatable.Value && isRepeatable) return;
        
        DrawIcon();
        DrawTooltip();
        DrawContextMenu();
    }

    private void DrawIcon()
    {
        if (internalQuestData.IssuerLocation is {Value: { } levelInfo})
        {
            var position = Service.MapManager.GetTextureOffsetPosition(new Vector2(levelInfo.X, levelInfo.Z));
            
            MapRenderer.DrawIcon(iconID, position, Settings.UnacceptedScale.Value);
        }
    }

    private void DrawTooltip()
    {
        if (!Settings.ShowTooltip.Value) return;
        if (!ImGui.IsItemHovered()) return;
        
        ImGui.BeginTooltip();
        ImGui.TextColored(Settings.UnacceptedTooltipColor.Value, $"{Strings.Map.Fate.Level} {internalQuestData.ClassJobLevel0} {internalQuestData.Name.ToDalamudString().TextValue}");
        if (Settings.DebugMode.Value)
        {
            ImGui.Text($"QuestID: {AdjustedQuestID}, IconID: {iconID}");
        }
        
        ImGui.EndTooltip();
    }

    private void DrawContextMenu()
    {
        if (!ImGui.IsItemClicked(ImGuiMouseButton.Right)) return;
        
        Service.ContextMenu.Show(ContextMenuType.Quest, AdjustedQuestID);
    }
}