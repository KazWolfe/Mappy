using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Mappy.Localization;
using Mappy.MapComponents;
using Mappy.UserInterface.Windows;

namespace Mappy.UserInterface.Components;

public enum ContextMenuType
{
    Inactive,
    General,
    Flag,
    GatheringArea,
}

public class MapContextMenu
{
    private Vector2 clickPosition;
    private ContextMenuType menuType;

    public void Show(ContextMenuType type)
    {
        menuType = type;
        
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
                TemporaryMarkersMapComponent.RemoveFlag();

                unsafe
                {
                    // Flag Set Byte, we want to clear this so that we can set a new flag
                    var byteAddress = (byte*) AgentMap.Instance() + 0x59B3;
                    *byteAddress = 0;
                }
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
                TemporaryMarkersMapComponent.RemoveGatheringArea();
            }

            ImGui.EndPopup();
        }
    }
}
