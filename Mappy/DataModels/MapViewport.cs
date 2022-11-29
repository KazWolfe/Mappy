using System.Numerics;

namespace Mappy.DataModels;

public class MapViewport
{
    public Vector2 Center = new(1024.0f, 1024.0f);
    public Vector2 Size;
    public float Scale = 1.0f;
}