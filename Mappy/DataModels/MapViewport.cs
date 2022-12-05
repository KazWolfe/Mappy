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
        set => scale = Math.Clamp(value, 0.2f, 5.0f);
    }
    public Vector2 Size { get; set; } = Vector2.Zero;
    public Vector2 ScaledTopLeft => Center * Scale - HalfSize;
    private Vector2 HalfSize => Size / 2.0f;
}