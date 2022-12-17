using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Mappy.Localization;
using Mappy.Modules;
using Mappy.UserInterface.Windows;

namespace Mappy.System;

public enum ContextMenuType
{
    Inactive,
    General,
    Flag,
    GatheringArea,
    Quest,
}

public class MapContextMenu
{
    private Vector2 clickPosition;
    private ContextMenuType menuType;
    private object? additionalData;

    public void Show(ContextMenuType type, object? data = null)
    {
        menuType = type;
        additionalData = data;
        
        clickPosition = Service.MapManager.GetTexturePosition(ImGui.GetMousePos() - MapWindow.MapContentsStart);
    }

    public void Draw()
    {
        if (Service.WindowManager.GetWindowOfType<MapWindow>(out var mapWindow))
        {
            if (mapWindow.IsFocused == false)
            {
                menuType = ContextMenuType.Inactive;
            }
        }
        
        switch (menuType)
        {
            case ContextMenuType.General:
                DrawGeneralContext();
                break;
            
            case ContextMenuType.Flag:
                DrawFlagContext();
                break;
            
            case ContextMenuType.GatheringArea:
                DrawGatheringContext();
                break;
            
            case ContextMenuType.Quest:
                DrawQuestContext();
                break;
        }
    }

    private unsafe void DrawGeneralContext()
    {
        if (ImGui.BeginPopupContextWindow("###GeneralRightClickContext"))
        {
            if (ImGui.Selectable("Add Flag"))
            {
                if (Service.MapManager.Map is { } map)
                {
                    if(Service.MapManager.Map.TerritoryType.Value is { } territory)
                    {
                        var agent = AgentMap.Instance();
                        
                        Service.GameIntegration.SetFlagMarker(agent, territory.RowId, map.RowId, clickPosition.X, clickPosition.Y, 60561);
                        Service.GameIntegration.InsertFlagInChat();
                    }
                }
            }
            ImGui.EndPopup();
        }
    }

    private void DrawFlagContext()
    {
        if (ImGui.BeginPopupContextWindow("###FlagContext"))
        {
            if (ImGui.Selectable(Strings.Map.RemoveFlag))
            {
                FlagMarker.ClearFlag();
            }

            ImGui.EndPopup();
        }
    }

    private void DrawGatheringContext()
    {
        if (ImGui.BeginPopupContextWindow("###GatheringContext"))
        {
            if (ImGui.Selectable(Strings.Map.RemoveGatheringArea))
            {
                GatheringAreaMarker.ClearGatheringArea();
            }

            ImGui.EndPopup();
        }
    }

    private void DrawQuestContext()
    {
        if (additionalData is null) return;
        
        if (ImGui.BeginPopupContextWindow("###QuestContext"))
        {
            if (ImGui.Selectable(Strings.Map.HideQuest))
            {
                if (!Service.Configuration.QuestMarkers.HiddenQuests.Contains((uint) additionalData))
                {
                    Service.Configuration.QuestMarkers.HiddenQuests.Add((uint)additionalData);
                    Service.Configuration.Save();
                }
            }

            ImGui.EndPopup();
        }
    }
}
