using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class QuestSettings
{
    public List<uint> HiddenQuests = new();
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowTribal = new(false);
    public Setting<bool> ShowFestival = new(false);
    public Setting<bool> ShowGrandCompany = new(false);
    public Setting<bool> HideRepeatable = new(false);
    public Setting<bool> HideAccepted = new(false);
    public Setting<bool> HideUnaccepted = new(false);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<float> AcceptedScale = new(0.75f);
    public Setting<float> UnacceptedScale = new(0.75f);
    public Setting<Vector4> InProgressColor = new(Colors.Red with {W = 0.20f});
    public Setting<Vector4> AcceptedTooltipColor = new(Colors.SoftGreen);
    public Setting<Vector4> UnacceptedTooltipColor = new(Colors.White);

    public Setting<bool> DebugMode = new(false);
}

public class Quests : IModule
{
    private static QuestSettings Settings => Service.Configuration.QuestMarkers;
    public IMapComponent MapComponent { get; } = new QuestMapComponent();
    public IModuleSettings Options { get; } = new QuestMarkerOptions();
    private unsafe class QuestMapComponent : IMapComponent
    {
        private readonly List<QuestData> unclaimedQuests = new();
    
        private static bool _dataStale;
        private bool refreshInProgress;
        private uint newMap;
    
        public QuestMapComponent()
        {
            PluginLog.Debug($"QuestManager: {new IntPtr(QuestManager.Instance()->Quest[0]):X8}");
        }
    
        public void Update(uint mapID)
        {
            newMap = mapID;
            _dataStale = true;
        }
    
        public void Draw()
        {
            if (!Settings.Enable.Value) return;
        
            if(!Settings.HideUnaccepted.Value) DrawUnclaimedQuests();
            if(!Settings.HideAccepted.Value) DrawClaimedQuests();

            if (_dataStale && !refreshInProgress)
            {
                unclaimedQuests.Clear();
                Task.Run(LoadMarkers);
                refreshInProgress = true;
            }
        }

        public static void RefreshMarkers() => _dataStale = true;

        private void DrawClaimedQuests()
        {
            foreach (var quest in GetAcceptedQuests())
            {
                var luminaData = Service.Cache.QuestCache.GetRow(quest.Base.QuestID + 65536u);

                var activeIndexes = GetActiveIndexes(luminaData, quest);
            
                foreach (var activeIndex in activeIndexes)
                {
                    foreach (var index in Enumerable.Range(0, 8))
                    {
                        if (!quest.TodoMask[index])
                        {
                            var targetRow = luminaData.ToDoLocation[activeIndex, index].Row;
                            if (targetRow != 0)
                            {
                                var level = Service.Cache.LevelCache.GetRow(targetRow);
                                if (level.RowId != 0 && Service.MapManager.LoadedMapId == level.Map.Row)
                                {
                                    DrawObjective(level, quest, luminaData);
                                }
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

            var radius = positionInfo.Radius * MapRenderer.Viewport.Scale / 7.0f;
            var color = ImGui.GetColorU32(Settings.InProgressColor.Value);
                
            ImGui.BeginGroup();
            ImGui.GetWindowDrawList().AddCircleFilled(drawPosition, radius, color);
            ImGui.GetWindowDrawList().AddCircle(drawPosition, radius, color, 0, 4);
            ImGui.EndGroup();
        }

        private void DrawUnclaimedQuests()
        {
            foreach (var quest in unclaimedQuests.TakeWhile(_ => !_dataStale))
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
        
            _dataStale = false;
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
    private class QuestMarkerOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.QuestMarker;
    
        public void DrawSettings()
        {
            var lastTribal = Settings.ShowTribal.Value;
            var lastFestival = Settings.ShowFestival.Value;
            var lastGrandCompany = Settings.ShowGrandCompany.Value;
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.FeatureToggles)
                .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
                .AddDummy(8.0f)
                .AddConfigCheckbox(Strings.Map.Quests.Tribal, Settings.ShowTribal)
                .AddConfigCheckbox(Strings.Map.Quests.Festival, Settings.ShowFestival)
                .AddConfigCheckbox(Strings.Map.Quests.GrandCompany, Settings.ShowGrandCompany)
                .AddConfigCheckbox(Strings.Map.Quests.HideRepeatable, Settings.HideRepeatable)
                .AddConfigCheckbox(Strings.Map.Quests.HideAccepted, Settings.HideAccepted)
                .AddConfigCheckbox(Strings.Map.Quests.HideUnaccepted, Settings.HideUnaccepted)
                .AddConfigCheckbox(Strings.Map.Generic.ShowTooltip, Settings.ShowTooltip)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.ColorOptions)
                .AddConfigColor(Strings.Map.Quests.ObjectiveColor, Settings.InProgressColor, Colors.Red with {W = 0.20f})
                .AddConfigColor(Strings.Map.Quests.AcceptedColor, Settings.AcceptedTooltipColor, Colors.SoftGreen)
                .AddConfigColor(Strings.Map.Quests.UnacceptedColor, Settings.UnacceptedTooltipColor, Colors.White)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.Quests.AcceptedScale, Settings.AcceptedScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddDragFloat(Strings.Map.Quests.UnacceptedScale, Settings.UnacceptedScale, 0.1f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.AcceptedScale.Value = 0.75f;
                    Settings.UnacceptedScale.Value = 0.75f;
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.Special)
                .AddButton(Strings.Map.Quests.ResetBlacklist, () =>
                {
                    Service.Configuration.QuestMarkers.HiddenQuests.Clear();
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .AddDummy(4.0f)
                .AddString(Strings.Map.Quests.RemoveFromBlacklist)
                .AddDummy(8.0f)
                .AddSeparator()
                .AddDummy(8.0f)
                .AddAction(DrawBlacklist)
                .Draw();

            if (Settings.ShowTribal.Value != lastTribal 
                || Settings.ShowFestival.Value != lastFestival 
                || Settings.ShowGrandCompany.Value != lastGrandCompany)
            {
                QuestMapComponent.RefreshMarkers();
            }
        }

        private void DrawBlacklist()
        {
            var size = new Vector2(InfoBox.Instance.InnerWidth, 150.0f);
        
            if(ImGui.BeginChild("###BlacklistFrame", size, true))
            {
                if (Service.Configuration.QuestMarkers.HiddenQuests.Count == 0)
                {
                    ImGui.TextColored(Colors.Orange, Strings.Map.Quests.EmptyBlacklist);
                }
                else
                {
                    foreach (var id in Service.Configuration.QuestMarkers.HiddenQuests)
                    {
                        var questName = Service.Cache.QuestCache.GetRow(id + 65536);
        
                        ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
                        if (ImGui.Selectable($"{id:D5}: {questName.Name.ToDalamudString().TextValue}"))
                        {
                            Service.Configuration.QuestMarkers.HiddenQuests.Remove(id);
                            Service.Configuration.Save();
                            break;
                        }
                    }
                }
            }
            ImGui.EndChild();
        }
    }

}

