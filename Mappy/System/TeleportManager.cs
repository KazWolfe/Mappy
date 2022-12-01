using System;
using System.Linq;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.System;

internal class TeleportManager : IDisposable
{
    private readonly ICallGateSubscriber<uint, byte, bool> teleportIpc;
    private readonly ICallGateSubscriber<bool> showChatMessageIpc;

    public TeleportManager()
    {
        teleportIpc = Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>("Teleport");
        showChatMessageIpc = Service.PluginInterface.GetIpcSubscriber<bool>("Teleport.ChatMessage");
    }

    public void Dispose()
    {
    }

    public void Teleport(Aetheryte targetAetherite)
    {
        if (AetheryteUnlocked(targetAetherite, out var targetAetheriteEntry))
        {
            Teleport(targetAetheriteEntry!);
        }
        else
        {
            PluginLog.Error("User attempted to teleport to an aetheryte that is not unlocked");
            UserError(Strings.Teleport.NotUnlocked);
        }
    }
    
    private void Teleport(AetheryteEntry aetheryte)
    {
        try
        {
            var didTeleport = teleportIpc.InvokeFunc(aetheryte.AetheryteId, aetheryte.SubIndex);
            var showMessage = showChatMessageIpc.InvokeFunc();

            if (!didTeleport)
            {
                UserError(Strings.Teleport.Error);
            }
            else if (showMessage)
            {
                Chat.Print(Strings.Teleport.Label, Strings.Teleport.Teleporting.Format(GetAetheryteName(aetheryte)));
            }
        }
        catch (IpcNotReadyError)
        {
            PluginLog.Error("Teleport IPC not found");
            UserError(Strings.Teleport.CommunicationError);
        }
    }

    private void UserError(string error)
    {
        Service.Chat.PrintError(error);
        Service.Toast.ShowError(error);
    }

    private string GetAetheryteName(AetheryteEntry aetheryte)
    {
        var gameData = aetheryte.AetheryteData.GameData;
        var placeName = gameData?.PlaceName.Value;

        return placeName == null ? "[Name Lookup Failed]" : placeName.Name;
    }
    
    private bool AetheryteUnlocked(Aetheryte aetheryte, out AetheryteEntry? entry)
    {
        if (Service.AetheryteList.Any(entry => entry.AetheryteId == aetheryte.RowId))
        {
            entry = Service.AetheryteList.Where(entry => entry.AetheryteId == aetheryte.RowId).First();
            return true;
        }
        else
        {
            entry = null;
            return false;
        }
    }
}
