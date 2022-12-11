using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;
using csQuest = FFXIVClientStructs.FFXIV.Client.Game.QuestManager.QuestListArray.Quest;

namespace Mappy.MapComponents;

public class QuestSettings
{
    public List<uint> HiddenQuests = new();
    public Setting<bool> ShowTribal = new(false);
    public Setting<bool> ShowFestival = new(false);
    public Setting<bool> ShowGrandCompany = new(false);
    public Setting<bool> HideAccepted = new(false);
    public Setting<bool> HideUnaccepted = new(false);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<float> AcceptedScale = new(0.75f);
    public Setting<float> UnacceptedScale = new(0.75f);
    public Setting<Vector4> InProgressColor = new(Colors.Red with {W = 0.20f});
    public Setting<Vector4> AcceptedTooltipColor = new(Colors.SoftGreen);
    public Setting<Vector4> UnacceptedTooltipColor = new(Colors.White);
}

public unsafe class QuestMapComponent : IMapComponent
{
    private static QuestSettings Settings => Service.Configuration.QuestMarkers;
    
    private readonly List<QuestData> unclaimedQuests = new();
    
    private bool dataStale;
    private bool refreshInProgress;
    private uint newMap;
    
    public QuestMapComponent()
    {
        PluginLog.Debug($"QuestManager: {new IntPtr(QuestManager.Instance()->Quest[0]):X8}");
    }
    
    public void Update(uint mapID)
    {
        newMap = mapID;
        dataStale = true;
    }
    
    public void Draw()
    {
        if(!Settings.HideUnaccepted.Value) DrawUnclaimedQuests();
        if(!Settings.HideAccepted.Value) DrawClaimedQuests();

        if (dataStale && !refreshInProgress)
        {
            unclaimedQuests.Clear();
            Task.Run(LoadMarkers);
            refreshInProgress = true;
        }
    }

    private void DrawClaimedQuests()
    {
        foreach (var quest in GetAcceptedQuests())
        {
            var luminaData = Service.Cache.QuestCache.GetRow(quest.Base.QuestID + 65536u);

            var activeIndexes = GetActiveIndexes(luminaData, quest);

            foreach (var activeIndex in activeIndexes)
            {
                if (luminaData.ToDoMainLocation[activeIndex] is {Value: { } level })
                {
                    if (level.RowId != 0 && Service.MapManager.LoadedMapId == level.Map.Row)
                    {
                        DrawObjective(level, quest, luminaData);
                    }
                }
                
                foreach (var index in Enumerable.Range(0, 7))
                {
                    if (luminaData.ToDoChildLocation[activeIndex, index] is {Value: { } subLevel})
                    {
                        if (subLevel.RowId != 0 && Service.MapManager.LoadedMapId == subLevel.Map.Row)
                        {
                            DrawObjective(subLevel, quest, luminaData);
                        }
                    }
                }
            }
        }
    }

    private static List<int> GetActiveIndexes(CustomQuestSheet luminaData, QuestExtended quest)
    {
        var activeIndexes = new List<int>();

        foreach (var index in Enumerable.Range(0, 24))
        {
            if (luminaData.ToDoCompleteSeq[index] == quest.CurrentSequenceNumber)
            {
                activeIndexes.Add(index);
            }
        }

        return activeIndexes;
    }

    private void DrawObjective(Level level, QuestExtended quest, CustomQuestSheet questData)
    {
        DrawRing(level);
        DrawIcon(level, quest.CurrentSequenceNumber);
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextColored(Settings.AcceptedTooltipColor.Value, questData.Name.ToDalamudString().TextValue);
            ImGui.EndTooltip();
        }
    }
    
    private void DrawIcon(Level level, byte currentSequenceNumber)
    {
        var position = Service.MapManager.GetTextureOffsetPosition(new Vector2(level.X, level.Z));

        var icon = currentSequenceNumber == 0xFF ? 071025u : 071023u;
        
        MapRenderer.DrawIcon(icon, position, Settings.AcceptedScale.Value);
    }

    private void DrawRing(Level positionInfo)
    {
        var position = Service.MapManager.GetTextureOffsetPosition(new Vector2(positionInfo.X, positionInfo.Z));
        var drawPosition = MapRenderer.GetImGuiWindowDrawPosition(position);

        var radius = positionInfo.Radius * MapRenderer.Viewport.Scale / 10.0f;
        var color = ImGui.GetColorU32(Settings.InProgressColor.Value);
                
        ImGui.BeginGroup();
        ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, color);
        ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 35, 4);
        ImGui.EndGroup();
    }

    private void DrawUnclaimedQuests()
    {
        foreach (var quest in unclaimedQuests.TakeWhile(_ => !dataStale))
        {
            quest.Draw();
        }
    }
    
    private void LoadMarkers()
    {
        var acceptedQuests = GetAcceptedQuests().Select(accepted => accepted.Base.QuestID);

        var unclaimedLuminaQuests = Service.DataManager.GetExcelSheet<Quest>()!
            .Where(quest => quest.IssuerLocation.Value?.Map.Row == newMap)
            .Where(quest => !QuestManager.IsQuestComplete(quest.RowId))
            .Where(PreReqsComplete)
            .Where(quest => !acceptedQuests.Contains((ushort) (quest.RowId - 65536)));
        
        foreach (var quest in unclaimedLuminaQuests)
        {
            unclaimedQuests.Add(new QuestData(quest));
        }
        
        dataStale = false;
        refreshInProgress = false;
    }

    private IEnumerable<QuestExtended> GetAcceptedQuests()
    {
        List<QuestExtended> questList = new();
        
        var questArray = QuestManager.Instance()->Quest;

        foreach (var index in Enumerable.Range(0, 30))
        {
            if (*questArray[index] is {IsHidden: false, QuestID: > 0} )
            {
                var questPointer = questArray[index];
                
                questList.Add(*(QuestExtended*)questPointer);
            }
        }

        return questList;
    }

    private bool PreReqsComplete(Quest quest)
    {
        if (!IsPreQuestsComplete(quest)) return false;
        if (IsQuestLocked(quest)) return false;

        if (IsTribeQuest(quest) && !Settings.ShowTribal.Value) return false;
        if (IsFestivalQuest(quest) && !Settings.ShowFestival.Value) return false;
        if (IsGrandCompany(quest) && !Settings.ShowGrandCompany.Value) return false;

        return true;
    }

    private bool IsPreQuestsComplete(Quest quest)
    {
        switch (quest.PreviousQuestJoin)
        {
            // 2 = must have any one of the following completed
            case 2:
                switch (quest)
                {
                    case {PreviousQuest0.Row: 0, PreviousQuest1.Row: 0, PreviousQuest2.Row: 0}: 
                        return true;
            
                    case {PreviousQuest0.Row: > 0 } when QuestManager.IsQuestComplete(quest.PreviousQuest0.Row):
                    case {PreviousQuest1.Row: > 0 } when QuestManager.IsQuestComplete(quest.PreviousQuest1.Row):
                    case {PreviousQuest2.Row: > 0 } when QuestManager.IsQuestComplete(quest.PreviousQuest2.Row):
                        return true;
            
                    default:
                        return false;
                }

            // 1 = must have all of the following completed
            case 1:
                switch (quest)
                {
                    case {PreviousQuest0.Row: 0, PreviousQuest1.Row: 0, PreviousQuest2.Row: 0}: 
                        return true;
            
                    case {PreviousQuest0.Row: > 0 } when !QuestManager.IsQuestComplete(quest.PreviousQuest0.Row):
                    case {PreviousQuest1.Row: > 0 } when !QuestManager.IsQuestComplete(quest.PreviousQuest1.Row):
                    case {PreviousQuest2.Row: > 0 } when !QuestManager.IsQuestComplete(quest.PreviousQuest2.Row):
                        return false;
            
                    default:
                        return true;
                }
                
            default:
                return false;
        }
    }

    private bool IsQuestLocked(Quest quest)
    {
        // 2 = must not have any completed
        if (quest.QuestLockJoin == 2)
        {
            if (quest.QuestLock[0] is {Row: > 0} && QuestManager.IsQuestComplete(quest.QuestLock[0].Row)) return true;
            if (quest.QuestLock[1] is {Row: > 0} && QuestManager.IsQuestComplete(quest.QuestLock[1].Row)) return true;
        }
        // 1 = must not have all completed
        else if (quest.QuestLockJoin == 1)
        {
            if (quest.QuestLock[0] is {Row: > 0} && !QuestManager.IsQuestComplete(quest.QuestLock[0].Row)) return false;
            if (quest.QuestLock[1] is {Row: > 0} && !QuestManager.IsQuestComplete(quest.QuestLock[1].Row)) return false;
        }

        return false;
    }

    private bool IsTribeQuest(Quest quest) => quest.BeastTribe.Row > 0;
    private bool IsFestivalQuest(Quest quest) => quest.Festival.Row > 0;
    private bool IsGrandCompany(Quest quest) => quest.GrandCompany.Row > 0;
}