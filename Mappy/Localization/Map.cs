using CheapLoc;

namespace Mappy.Localization;

public static partial class Strings
{
    public static MapStrings Map { get; set; } = new();
}

public class MapStrings
{
    public string NoLayers => Loc.Localize("Map_NoLayers", "Map has no layers");
    public string Aetheryte => Loc.Localize("Map_Aetheryte", "Aetheryte");

    public FateStrings Fate { get; set; } = new();
}

public class FateStrings
{
    public string Label => Loc.Localize("Fate_Label", "Fate");
    public string Level => Loc.Localize("Fate_Level", "Lv.");
    public string TimeRemaining => Loc.Localize("Fate_TimeRemaining", "Time Remaining");
    public string Progress => Loc.Localize("Fate_Progress", "Progress");
}