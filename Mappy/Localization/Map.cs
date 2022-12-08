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
    
    public FateStrings Fate { get; } = new();
    public GatheringStrings Gathering { get; } = new();
    public MarkerStrings Markers { get; } = new();
    public PartyMemberStrings PartyMembers { get; } = new();
    public PetStrings Pets { get; } = new();
    public AllianceMemberStrings AllianceMembers { get; } = new();
    public PlayerStrings Player { get; } = new();
    public WaymarkStrings Waymarks { get; } = new();
}

public class FateStrings
{
    public string Label => Loc.Localize("Fate_Label", "Fate");
    public string Level => Loc.Localize("Fate_Level", "Lv.");
    public string TimeRemaining => Loc.Localize("Fate_TimeRemaining", "Time Remaining");
    public string Progress => Loc.Localize("Fate_Progress", "Progress");
    public string ShowRing => Loc.Localize("Fate_ShowRing", "Show Ring");
    public string ShowTooltip => Loc.Localize("Fate_ShowTooltip", "Show Tooltip");
    public string ShowIcon => Loc.Localize("Fate_ShowIcon", "Show Icon");
    public string Color => Loc.Localize("Fate_Color", "Circle Color");
}

public class GatheringStrings
{
    public string Label => Loc.Localize("Gathering_Label", "Gathering Points");
    public string ShowTooltip => Loc.Localize("Gathering_ShowTooltip", "Show Tooltip");
    public string ShowIcon => Loc.Localize("Gathering_ShowIcon", "Show Icon");
}

public class MarkerStrings
{
    public string Label => Loc.Localize("Marker_Label", "Map Markers");
}

public class PartyMemberStrings
{
    public string Label => Loc.Localize("PartyMember_Label", "Party Members");
}

public class PetStrings
{
    public string Label => Loc.Localize("Pet_Label", "Pets");
}

public class AllianceMemberStrings
{
    public string Label => Loc.Localize("AllianceMember_Label", "Alliance Members");
    public string ShowIcon => Loc.Localize("AllianceMember_ShowIcon", "Show Icon");
    public string GreenMarker => Loc.Localize("AllianceMember_GreenMarker", "Green Marker");
    public string RedMarker => Loc.Localize("AllianceMember_RedMarker", "Red Marker");
    public string YellowMarker => Loc.Localize("AllianceMember_YellowMarker", "Yellow Marker");
    public string BlueMarker => Loc.Localize("AllianceMember_BlueMarker", "Blue Marker");
}

public class PlayerStrings
{
    public string Label => Loc.Localize("Player_Label", "Player");
    public string OutlineColor => Loc.Localize("Player_OutlineColor", "Cone Outline Color");
    public string FillColor => Loc.Localize("Player_FillColor", "Cone Fill Color");
    public string IconScale => Loc.Localize("Player_IconScale", "Icon Scale");
    public string ConeRadius => Loc.Localize("Player_ConeRadius", "Cone Radius");
    public string ConeAngle => Loc.Localize("Player_ConeAngle", "Cone Angle");
    public string ConeAngleHelp => Loc.Localize("Player_ConeAngleHelp", "Angle in Degrees");
    public string ConeThickness => Loc.Localize("Player_ConeThickness", "Outline Thickness");
    public string ShowIcon => Loc.Localize("Player_ShowIcon", "Show Player Icon");
    public string ShowCone => Loc.Localize("Player_ShowCone", "Show Look Cone");
}

public class WaymarkStrings
{
    public string Label => Loc.Localize("Waymark_Label", "Waymarks");
}