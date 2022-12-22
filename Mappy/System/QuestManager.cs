using System.Collections.Generic;
using System.Linq;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;


namespace Mappy.System;

public class QuestManager
{
    private readonly QuestList questList = new();
    
    public IEnumerable<Level>? GetActiveLevelsForQuest(string questName, uint mapID)
    {
        return (
            from quest in GetAcceptedQuests() 
            let luminaData = Service.Cache.QuestCache.GetRow(quest.QuestID + 65536u) 
            where luminaData.Name.ToDalamudString().TextValue == questName 
            select GetActiveLevelsForQuest(quest, mapID)
            ).FirstOrDefault();
    }

    public IEnumerable<QuestExtended> GetAcceptedQuests() => questList.Where(quest => quest is {IsHidden: false, QuestID: > 0});

    public IEnumerable<Level> GetActiveLevelsForQuest(QuestExtended quest, uint? mapID = null)
    {
        var luminaData = Service.Cache.QuestCache.GetRow(quest.QuestID + 65536u);

        var activeIndexes = GetActiveLevelIndexes(quest);

        var targetMap = mapID ?? Service.MapManager.LoadedMapId;
        
        return (
            from activeIndex 
                in activeIndexes 
            from index 
                in Enumerable.Range(0, 8) 
            where !quest.TodoMask[index] 
                select luminaData.ToDoLocation[activeIndex, index].Row 
                into targetRow 
                where targetRow != 0 
                    select Service.Cache.LevelCache.GetRow(targetRow) 
                    into level 
                    where level.RowId != 0 && targetMap == level.Map.Row 
                        select level
            ).ToHashSet();
    }
    
    private IEnumerable<int> GetActiveLevelIndexes(QuestExtended quest)
    {
        var luminaData = Service.Cache.QuestCache.GetRow(quest.QuestID + 65536u);
        return Enumerable.Range(0, 24).Where(index => luminaData.ToDoCompleteSeq[index] == quest.CurrentSequenceNumber).ToList();
    }
}

