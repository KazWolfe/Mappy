﻿using System;
using System.Linq;
using System.Numerics;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Mappy.DataModels;
using Mappy.Modules;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System;

public unsafe class GameIntegration : IDisposable
{
    private delegate void OpenMapByIdDelegate(AgentMap* agent, uint mapID);
    private readonly Hook<OpenMapByIdDelegate>? openMapByIdHook;

    private delegate void OpenMapDelegate(AgentMap* agent, OpenMapInfo* data);
    private readonly Hook<OpenMapDelegate>? openMapHook;

    private delegate void SetFlagMarkerDelegate(AgentMap* agent, uint territoryId, uint mapId, float mapX, float mapY, uint iconId);
    private readonly Hook<SetFlagMarkerDelegate>? setFlagMarkerHook;

    private delegate void SetGatheringMarkerDelegate(AgentMap* agent, uint styleFlags, int mapX, int mapY, uint iconID, int radius, Utf8String* tooltip);
    private readonly Hook<SetGatheringMarkerDelegate>? setGatheringMarkerHook;
    
    private delegate void ShowMapDelegate();
    [Signature("E8 ?? ?? ?? ?? 40 B6 01 C7 44 24 ?? ?? ?? ?? ?? BA ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 84 C0 74 15", DetourName = nameof(OnShowHook))]
    private readonly Hook<ShowMapDelegate>? showHook = null;

    private delegate byte InsertTextCommand(AgentInterface* agent, uint paramID, byte a3 = 0);
    [Signature("E8 ?? ?? ?? ?? 40 88 6E 08 EB 04")]
    private readonly InsertTextCommand? insertFlagTextCommand = null;

    private AgentInterface* ChatAgent => Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.ChatLog);
    private AgentInterface* GatheringNoteAgent => Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.GatheringNote);
    
    public GameIntegration()
    {
        SignatureHelper.Initialise(this);

        openMapByIdHook ??= Hook<OpenMapByIdDelegate>.FromAddress(new IntPtr(AgentMap.fpOpenMapByMapId), OpenMapById);
        openMapHook ??= Hook<OpenMapDelegate>.FromAddress(new IntPtr(AgentMap.fpOpenMap), OpenMap);
        setFlagMarkerHook ??= Hook<SetFlagMarkerDelegate>.FromAddress(new IntPtr(AgentMap.fpSetFlagMapMarker), SetFlagMarker);
        setGatheringMarkerHook ??= Hook<SetGatheringMarkerDelegate>.FromAddress(new IntPtr(AgentMap.fpAddGatheringTempMarker), SetGatheringMarker);

        if (Service.Configuration.EnableIntegrations.Value)
        {
            openMapByIdHook?.Enable();
            openMapHook?.Enable();
            setFlagMarkerHook?.Enable();
            setGatheringMarkerHook?.Enable();
            showHook?.Enable();
        }

        Service.ClientState.EnterPvP += Disable;
        Service.ClientState.LeavePvP += TryEnable;
    }
    
    public void Dispose()
    {
        openMapByIdHook?.Dispose();
        openMapHook?.Dispose();
        setFlagMarkerHook?.Dispose();
        setGatheringMarkerHook?.Dispose();
        showHook?.Dispose();
        
        Service.ClientState.EnterPvP -= Disable;
        Service.ClientState.LeavePvP -= TryEnable;
    }

    public void Enable()
    {
        openMapByIdHook?.Enable();
        openMapHook?.Enable();
        setFlagMarkerHook?.Enable();
        setGatheringMarkerHook?.Enable();
        showHook?.Enable();
    }

    public void Disable()
    {
        openMapByIdHook?.Disable();
        openMapHook?.Disable();
        setFlagMarkerHook?.Disable();
        setGatheringMarkerHook?.Disable();
        showHook?.Disable();
    }

    private void TryEnable()
    {
        if (Service.Configuration.EnableIntegrations.Value)
        {
            Enable();
        }
    }
    
    private void OpenMapById(AgentMap* agent, uint mapId)
    {
        Safety.ExecuteSafe(() =>
        {
            Service.MapManager.LoadMap(mapId);
        }, "Exception during OpenMapByMapId");
    }
    
    private void OpenMap(AgentMap* agent, OpenMapInfo* mapInfo)
    {
        Safety.ExecuteSafe(() =>
        {
            PluginLog.Debug("OpenMap");
            
            MapWindow.FocusWindow();

            switch (mapInfo->Type)
            {
                case MapType.FlagMarker when FlagMarker.GetFlag() is {} flag:
                    Service.MapManager.LoadMap(mapInfo->MapId, flag.Position);
                    break;
                
                case MapType.QuestLog when GetQuestLocation(mapInfo) is {} questLocation:
                    Service.MapManager.LoadMap(mapInfo->MapId, questLocation);
                    break;
                
                case MapType.GatheringLog when GatheringAreaMarker.GetGatheringArea() is {} area:
                    Service.MapManager.LoadMap(mapInfo->MapId, area.Position);
                    break;
                
                default:
                    Service.MapManager.LoadMap(mapInfo->MapId);
                    break;
            }

        }, "Exception during OpenMap");
    }
    
    private Vector2? GetQuestLocation(OpenMapInfo* mapInfo)
    {
        var targetLevels = Service.QuestManager.GetActiveLevelsForQuest(mapInfo->TitleString.ToString(), mapInfo->MapId);
        var focusLevel = targetLevels?.Where(level => level.Map.Row == mapInfo->MapId && level.Map.Row != 0).FirstOrDefault();

        if (focusLevel is not null)
        {
            return new Vector2(focusLevel.X, focusLevel.Z);
        }

        return null;
    }
    
    private static void OnShowHook()
    {
        Safety.ExecuteSafe(() =>
        {
            MapWindow.FocusWindow(true);
        }, "Exception during OnShowHook");
    }

    public void SetFlagMarker(AgentMap* agent, uint territoryId, uint mapId, float mapX, float mapY, uint iconId)
    {
        Safety.ExecuteSafe(() =>
        {
            PluginLog.Debug($"SetFlagMarker");
            
            FlagMarker.SetFlag(new TempMarker
            {
                Type = MarkerType.Flag,
                MapID = mapId,
                IconID = iconId,
                Position = new Vector2(mapX, mapY)
            });

            setFlagMarkerHook!.Original(agent, territoryId, mapId, mapX, mapY, iconId);
        }, "Exception during SetFlagMarker");
    }

    private void SetGatheringMarker(AgentMap* agent, uint styleFlags, int mapX, int mapY, uint iconID, int radius, Utf8String* tooltip)
    {
        Safety.ExecuteSafe(() =>
        {
            PluginLog.Debug("GatheringTrigger");

            GatheringAreaMarker.SetGatheringArea(new TempMarker
            {
                Type = MarkerType.Gathering,
                MapID = GetGatheringAreaMapInfo()->MapId,
                IconID = iconID,
                Radius = radius,
                Position = new Vector2(mapX, mapY),
                TooltipText = tooltip->ToString(),
            });
            
        }, "Exception during SetGatheringMarker");
    }

    public void InsertFlagInChat() => insertFlagTextCommand?.Invoke(ChatAgent, 1048u);

    private OpenMapInfo* GetGatheringAreaMapInfo()
    {
        // GatheringNoteAgent+184 is a pointer to where the OpenMapInfo block is roughly located
        var agentPointer = new IntPtr(GatheringNoteAgent);
        var agentOffsetPointer = agentPointer + 184;

        // OpenMapInfo is allocated 16bytes from this address
        var dataBlockPointer = new IntPtr(*(long*) agentOffsetPointer);
        var dataBlock = dataBlockPointer + 16;
        
        return (OpenMapInfo*) dataBlock;
    }
}