using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Mappy.DataModels;

[StructLayout(LayoutKind.Explicit, Size = 24)]
public struct QuestExtended
{
    [FieldOffset(0)] public QuestManager.QuestListArray.Quest Base;
    [FieldOffset(10)] public readonly byte CurrentSequenceNumber;
}