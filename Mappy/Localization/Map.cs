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
    public string FollowPlayer => Loc.Localize("Map_FollowPlayerMouseover", "Follow Player");
    public string CenterOnPlayer => Loc.Localize("Map_CenterOnPlayer", "Move Map To Player");
    public string SearchForMap => Loc.Localize("Map_SearchForMap", "Search For Map");
    public string Settings => Loc.Localize("Map_Settings", "Settings");
    
    public FateStrings Fate { get; set; } = new();
}

public class FateStrings
{
    public string Label => Loc.Localize("Fate_Label", "Fate");
    public string Level => Loc.Localize("Fate_Level", "Lv.");
    public string TimeRemaining => Loc.Localize("Fate_TimeRemaining", "Time Remaining");
    public string Progress => Loc.Localize("Fate_Progress", "Progress");
}