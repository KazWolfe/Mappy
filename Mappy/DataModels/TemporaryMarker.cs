using System.Numerics;

namespace Mappy.DataModels;

public enum MarkerType
{
    Unknown,
    Flag,
    Gathering
}

public class TempMarker
{
    public MarkerType Type { get; init; } = MarkerType.Unknown;
    public uint MapID { get; init; }
    public uint IconID { get; init; }
    public Vector2 Position { get; init; } = Vector2.Zero;
    public float Radius { get; init; }
    public string TooltipText { get; init; } = string.Empty;

    public Vector2 AdjustedPosition => Service.MapManager.GetTextureOffsetPosition(Position);
}