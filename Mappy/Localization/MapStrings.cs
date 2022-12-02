using CheapLoc;

namespace Mappy.Localization;

public static partial class Strings
{
    public static MapStrings Map { get; set; } = new();
}

public class MapStrings
{
    public string NoLayers => Loc.Localize("Map_NoLayers", "Map has no layers");
}