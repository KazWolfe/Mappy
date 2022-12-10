using System;
using System.Numerics;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Mappy.MapComponents;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System;

public unsafe class GameIntegration : IDisposable
{
    private delegate void OpenMapByIdDelegate(AgentInterface* agent, uint mapID);
    private readonly Hook<OpenMapByIdDelegate>? openMapByIdHook;

    private delegate void OpenMapDelegate(AgentInterface* agent, OpenMapInfo* data);
    private readonly Hook<OpenMapDelegate>? openMapHook;

    private delegate void SetFlagMarkerDelegate(AgentInterface* agent, uint territoryId, uint mapId, float mapX, float mapY, uint iconId);
    private readonly Hook<SetFlagMarkerDelegate>? setFlagMarkerHook;

    private delegate void SetGatheringMarkerDelegate(AgentInterface* agent, uint styleFlags, int mapX, int mapY, uint iconID, int radius, Utf8String* tooltip);
    private readonly Hook<SetGatheringMarkerDelegate>? setGatheringMarkerHook;
    
    private delegate void ShowMapDelegate();
    [Signature("E8 ?? ?? ?? ?? 40 B6 01 C7 44 24 ?? ?? ?? ?? ?? BA ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 84 C0 74 15", DetourName = nameof(OnShowHook))]
    private readonly Hook<ShowMapDelegate>? showHook = null;

    private bool markerCalled;
    
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

    public void TryEnable()
    {
        if (Service.Configuration.EnableIntegrations.Value)
        {
            Enable();
        }
    }
    
    private void OpenMapById(AgentInterface* agent, uint mapId)
    {
        try
        {
            PluginLog.Debug("OpenMapById");
            Service.MapManager.LoadMap(mapId);
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Exception during OpenMapByMapId");
        }
    }
    
    private void OpenMap(AgentInterface* agent, OpenMapInfo* mapInfo)
    {
        try
        {
            PluginLog.Debug("OpenMap");
            
            Service.MapManager.LoadMap(mapInfo->MapId);

            if (markerCalled)
            {
                if (TemporaryMarkersMapComponent.TempMarker is { } stagedMarker)
                {
                    stagedMarker.MapID = mapInfo->MapId;
                    
                    MapRenderer.SetViewportCenter(stagedMarker.AdjustedPosition);
                    MapRenderer.SetViewportZoom(0.8f);
                
                    TemporaryMarkersMapComponent.AddMarker(stagedMarker);
                }

                if (Service.WindowManager.GetWindowOfType<MapWindow>(out var mapWindow))
                {
                    mapWindow.IsOpen = true;
                    ImGui.SetWindowFocus("Mappy Map Window");
                }
                
                markerCalled = false;
            }
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Exception during OpenMapByMapId");
        }
    }
    
    private void OnShowHook()
    {
        try
        {
            PluginLog.Debug("MapShowHook");
            
            if (Service.WindowManager.GetWindowOfType<MapWindow>(out var mapWindow))
            {
                mapWindow.IsOpen = !mapWindow.IsOpen;
            }
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Exception During Map Show");
        }
    }

    private void SetFlagMarker(AgentInterface* agent, uint territoryId, uint mapId, float mapX, float mapY, uint iconId)
    {
        try
        {
            PluginLog.Debug("FlagTrigger");
            markerCalled = true;

            TemporaryMarkersMapComponent.TempMarker = new TemporaryMarker
            {
                Type = MarkerType.Flag,
                MapID = mapId,
                IconID = iconId,
                Position = new Vector2(mapX, mapY),
            };

            if (Service.MapManager.LoadedMapId != mapId)
            {
                MapRenderer.SetViewportCenter(stagedMarker.AdjustedPosition);
                MapRenderer.SetViewportZoom(0.8f);
            }
            
            var flagSetByte = (byte*) AgentMap.Instance() + 0x59B3;

            if (*flagSetByte > 0)
            {
                *flagSetByte = 0;
            }
                
            TemporaryMarkersMapComponent.AddMarker(stagedMarker);
            setFlagMarkerHook!.Original(agent, territoryId, mapId, mapX, mapY, iconId);
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Exception Set Flag Marker");
        }
    }

    private void SetGatheringMarker(AgentInterface* agent, uint styleFlags, int mapX, int mapY, uint iconID, int radius, Utf8String* tooltip)
    {
        try
        {
            PluginLog.Debug("GatheringTrigger");
            markerCalled = true;
            
            TemporaryMarkersMapComponent.TempMarker = new TemporaryMarker
            {
                Type = MarkerType.Gathering,
                IconID = iconID,
                Radius = radius,
                Position = new Vector2(mapX, mapY),
                TooltipText = tooltip->ToString(),
            };
            
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Exception During Set Gathering Marker");
        }
    }
}