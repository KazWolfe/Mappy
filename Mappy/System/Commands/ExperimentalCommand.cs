using System;
using System.Collections.Generic;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Memory;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.GeneratedSheets;
using Mappy.Interfaces;
using Mappy.Utilities;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace Mappy.System.Commands;

public unsafe class ExperimentalCommand : IPluginCommand, IDisposable
{
    public string CommandArgument => "re";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            Hidden = true,
            CommandKeyword = "lumina",
            ParameterAction = (strings =>
            {
                if (strings is not null)
                {
                    var sheetID = uint.Parse(strings[0]);
                    var sheet = Framework.Instance()->ExdModule->ExcelModule->GetSheetByIndex(sheetID);
                    var name = MemoryHelper.ReadSeString((IntPtr) sheet->SheetName, 64);
                    PluginLog.Information($"[{sheetID}]: {name}");
                }
            })
        }
    };

    private static ExperimentClass _instance = null!;

    public ExperimentalCommand()
    {
        _instance = new ExperimentClass();
        
        (SubCommands as List<ISubCommand>)?.Add(        
            new SubCommand
            {
                Hidden = true,
                CommandKeyword = null,
                CommandAction = _instance.RunExperiment,
            });
    }

    public void Dispose()
    {
        _instance.Dispose();
    }
}

public unsafe class ExperimentClass
{
    // private delegate byte IsQuestAcceptQualifiedDelegate(IntPtr a1, ushort questID, byte a3 = 0);
    // [Signature("48 89 5C 24 ?? 57 48 83 EC 20 0F B7 D2", DetourName = nameof(IsQuestAcceptQualified))]
    // private readonly Hook<IsQuestAcceptQualifiedDelegate>? isQuestAcceptQualifiedHook = null!;

    private delegate IntPtr IsQuestAcceptQualifiedDelegate(IntPtr a1, IntPtr a2);
    [Signature("E8 ?? ?? ?? ?? 0F B7 9E ?? ?? ?? ?? 4C 8B F0", DetourName = nameof(IsQuestAcceptQualified))]
    private readonly Hook<IsQuestAcceptQualifiedDelegate>? isQuestAcceptQualifiedHook = null!;
    
    private delegate IntPtr GetQuestDataDelegate(IntPtr a1, uint questID);
    [Signature("E8 ?? ?? ?? ?? 48 3B C6 75 35", ScanType = ScanType.Text)]
    private readonly GetQuestDataDelegate getQuestData = null!;
    
    [Signature("E8 ?? ?? ?? ?? 48 3B C6 75 35", ScanType = ScanType.Text, DetourName = nameof(GetQuestDataFunction))]
    private readonly Hook<GetQuestDataDelegate>? getQuestDataHook = null!;
    
    private delegate IntPtr GetQuestStatusDelegate(IntPtr a1, IntPtr a2);
    [Signature("E8 ?? ?? ?? ?? 0F B7 9E ?? ?? ?? ?? 4C 8B F0")]
    private readonly GetQuestStatusDelegate getQuestStatus = null!;
    
    public ExperimentClass()
    {
        SignatureHelper.Initialise(this);
        
        // isQuestAcceptQualifiedHook?.Enable();
        getQuestDataHook.Enable();
    }

    public void Dispose()
    {
        isQuestAcceptQualifiedHook?.Dispose();
        getQuestDataHook?.Dispose();
    }

    public void RunExperiment()
    {
        var eventFramework = new IntPtr(EventFramework.Instance());
        var player = Service.ClientState.LocalPlayer?.Address ?? IntPtr.Zero;

        foreach (var quest in Service.DataManager.GetExcelSheet<Quest>()!)
        {
            // if (IsQuestAcceptable(quest.RowId, 1))
            // {
            //     PluginLog.Debug($"{quest.Name.ToDalamudString().TextValue}");
            // }
                
            var questDataPointer = getQuestData(eventFramework, quest.RowId | 0x10000u);
                
            if (questDataPointer != IntPtr.Zero)
            {
                // PluginLog.Debug($"Result: {questDataPointer:X8}, ID: {quest.RowId}, Name: {quest.Name.ToDalamudString().TextValue}");
                    
                var status = getQuestStatus(questDataPointer, player).ToInt64();
                PluginLog.Debug($"{quest.RowId,6} Lv. {quest.ClassJobLevel0,2} {quest.Name.ToDalamudString().TextValue,-48} :: {status:X16} :: {questDataPointer:X8}"); //; : {(status & 0xFFFFFFFFBC7DFFFF):X9}");

                // PluginLog.Debug($"Status: {status:X8}, Result: {questDataPointer:X8}, ID: {quest.RowId}, Name: {quest.Name.ToDalamudString().TextValue}");
            }
        }
            
        // const int questID = 66316;
        //
        // var questDataPtr = getQuestData(frameworkInstance, questID | 0x10000u);
        // PluginLog.Debug($"Result: {questDataPtr:X8}");
            
        PluginLog.Debug($"QuestManager: {new IntPtr(FFXIVClientStructs.FFXIV.Client.Game.QuestManager.Instance()->Quest[0]):X8}");
    }
    
    private bool IsQuestAcceptable(uint questID, byte filter)
    {
        var frameworkInstance = new IntPtr(EventFramework.Instance());

        var questDataPointer = getQuestData(frameworkInstance, questID | 0x10000u);

        if (questDataPointer != IntPtr.Zero)
        {
            var player = Service.ClientState.LocalPlayer?.Address ?? IntPtr.Zero;

            var questStatus = getQuestStatus(questDataPointer, player);

            if (filter != 0)
            {
                if (questStatus != IntPtr.Zero)
                {
                    return (questStatus.ToInt64() & 0x103C0CD800) != 0;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return questStatus.ToInt64() == 0;
            }
        }

        return questDataPointer.ToInt64() != 0;
    }
    
    private HashSet<uint> indexes = new();
        
    private IntPtr GetQuestDataFunction(IntPtr a1, uint a2)
    {
        var result = getQuestDataHook!.Original(a1, a2);
            
        Safety.ExecuteSafe(() =>
        {
            // if (a2 == 69893)
            // {
            //     PluginLog.Debug($"GetQuestDataFunction: {a1:X8}, {a2}, {result:X8} ?? {new IntPtr(EventFramework.Instance()):X8}");
            // }
                
            // indexes.Add(a2);
            //
            // PluginLog.Debug(string.Join(", ", indexes));
                
        }, "Exception in GetQuestDataFunction");

        return result;
    }

    // private byte IsQuestAcceptQualified(IntPtr a1, ushort questID, byte a3)
    // {
    //     var result = isQuestAcceptQualifiedHook!.Original(a1, questID, a3);
        
    private IntPtr IsQuestAcceptQualified(IntPtr a1, IntPtr a2)
    {
        var result = isQuestAcceptQualifiedHook!.Original(a1, a2);
            
        Safety.ExecuteSafe(() =>
        {
            PluginLog.Debug($"IsQuestAcceptQualified: {a1:X8}, {a2:X8} :: {result:X8}");
        }, "Exception in IsQuestAcceptQualified");

        return result;
    }
}