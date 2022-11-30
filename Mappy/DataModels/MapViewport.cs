using System;
using System.Numerics;

namespace Mappy.DataModels;

public class MapViewport
{
    public Vector2 Center { get; set; } = new(1024.0f, 1024.0f);

    private float scale = 1.0f;
    public float Scale
    {
        get => scale;
        set => scale = Math.Clamp(value, 0.4f, 4.0f);
    }

    public Vector2 Size { get; set; } = Vector2.Zero;
    public Vector2 ScaledCenter => Center * Scale;
    public Vector2 ScaledTopLeft => Center * Scale - HalfSize;
    public Vector2 ScaledBottomRight => Center * Scale + HalfSize;
    public Vector4 ScaledBounds => new(ScaledTopLeft.X, ScaledTopLeft.Y, ScaledBottomRight.X, ScaledBottomRight.Y);

    public Vector2 TopLeft
    {
        get => Center - HalfSize;
        set => Center = value - HalfSize;
    }

    public Vector2 BottomRight
    {
        get => Center + HalfSize;
        set => Center = value + HalfSize;
    }
    
    private Vector2 HalfSize => Size / 2.0f;
}