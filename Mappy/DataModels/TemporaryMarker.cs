using System.Numerics;

namespace Mappy.DataModels;

public enum MarkerType
{
    Unknown,
    Flag,
    Gathering
}

public class TemporaryMarker
{
    public MarkerType Type { get; set; } = MarkerType.Unknown;
    public uint MapID { get; set; } = 0;
    public uint IconID { get; set; } = 0;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Radius { get; set; } = 0.0f;
    public string TooltipText { get; set; } = string.Empty;

    public Vector2 AdjustedPosition => Service.MapManager.GetTextureOffsetPosition(Position);
}