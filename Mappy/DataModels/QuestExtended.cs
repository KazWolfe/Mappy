using System.Collections;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Mappy.DataModels;

[StructLayout(LayoutKind.Explicit, Size = 24)]
public struct QuestExtended
{
    [FieldOffset(0)] public QuestManager.QuestListArray.Quest Base;
    [FieldOffset(10)] public readonly byte CurrentSequenceNumber;
    [FieldOffset(17)] private readonly byte TodoBits;

    public BitArray TodoMask => new BitArray(new[]{TodoBits}).Reverse();

}

public static class BitArrayExtensions
{
    public static BitArray Reverse(this BitArray array)
    {
        var length = array.Length;
        var mid = length / 2;

        for (var i = 0; i < mid; i++)
        {
            (array[i], array[length - i - 1]) = (array[length - i - 1], array[i]);
        }

        return array;
    }
}