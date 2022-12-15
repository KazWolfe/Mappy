using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Mappy.DataModels;

[Flags]
public enum QuestFlags : byte
{
    None = 0,
    Priority = 1,
    Hidden = 8,
}

[StructLayout(LayoutKind.Explicit, Size = 24)]
public struct QuestExtended
{
    [FieldOffset(8)] public ushort QuestID;
    [FieldOffset(10)] public readonly byte CurrentSequenceNumber;
    [FieldOffset(11)] public QuestFlags Flags;
    [FieldOffset(17)] private readonly byte TodoBits;

    public bool IsHidden => Flags.HasFlag(QuestFlags.Hidden);
    public BitArray TodoMask => new BitArray(new[]{TodoBits}).Reverse();
    
    public uint IconID => CurrentSequenceNumber == 0xFF ? 071025u : 071023u;
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