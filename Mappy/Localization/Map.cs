using CheapLoc;

namespace Mappy.Localization;

public static partial class Strings
{
    public static MapStrings Map { get; set; } = new();
}

public class MapStrings
{
    public string FollowPlayer => Loc.Localize("Map_FollowPlayerMouseover", "Follow Player");
    public string CenterOnPlayer => Loc.Localize("Map_CenterOnPlayer", "Move Map To Player");
    public string SearchForMap => Loc.Localize("Map_SearchForMap", "Search For Map");
    public string Settings => Loc.Localize("Map_Settings", "Settings");
    public string DefaultTooltipColor => Loc.Localize("Map_DefaultTooltipColor", "Default Tooltip Color");
    public string MapLinkTooltipColor => Loc.Localize("Map_MapLinkTooltipColor", "Map Link Tooltip Color");
    public string InstanceLinkTooltipColor => Loc.Localize("Map_InstanceLinkTooltipColor", "Instance Link Tooltip Color");
    public string AetheryteTooltipColor => Loc.Localize("Map_AetheryteTooltipColor", "Aetheryte Tooltip Color");
    public string AethernetTooltipColor => Loc.Localize("Map_AethernetTooltipColor", "Aethernet Tooltip Color"); 
    
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
    public string Enable => Loc.Localize("Fate_Enable", "Enable");
    public string Level => Loc.Localize("Fate_Level", "Lv.");
    public string TimeRemaining => Loc.Localize("Fate_TimeRemaining", "Time Remaining");
    public string Progress => Loc.Localize("Fate_Progress", "Progress");
    public string ShowRing => Loc.Localize("Fate_ShowRing", "Show Ring");
    public string ShowTooltip => Loc.Localize("Fate_ShowTooltip", "Show Tooltip");
    public string ShowIcon => Loc.Localize("Fate_ShowIcon", "Show Icon");
    public string Color => Loc.Localize("Fate_Color", "Circle Color");
    public string TooltipColor => Loc.Localize("Fate_TooltipColor", "Tooltip Color");
    public string IconScale => Loc.Localize("Fate_IconScale", "Icon Scale");
}

public class GatheringStrings
{
    public string Label => Loc.Localize("Gathering_Label", "Gathering Points");
    public string Enable => Loc.Localize("Gathering_Enable", "Enable");
    public string ShowTooltip => Loc.Localize("Gathering_ShowTooltip", "Show Tooltip");
    public string ShowIcon => Loc.Localize("Gathering_ShowIcon", "Show Icon");
    public string TooltipColor => Loc.Localize("Gathering_TooltipColor", "Tooltip Color");
    public string IconScale => Loc.Localize("Gathering_IconScale", "Icon Scale");
}

public class MarkerStrings
{
    public string Label => Loc.Localize("Marker_Label", "Map Markers");
    public string Enable => Loc.Localize("Marker_Enable", "Enable");
    public string IconScale => Loc.Localize("Marker_Scale", "Icon Scale");
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
    public string Enable => Loc.Localize("AllianceMember_Enable", "Enable");
    public string ShowTooltip => Loc.Localize("AllianceMember_ShowTooltip", "Show Tooltip");
    public string ShowIcon => Loc.Localize("AllianceMember_ShowIcon", "Show Icon");
    public string GreenMarker => Loc.Localize("AllianceMember_GreenMarker", "Green Marker");
    public string RedMarker => Loc.Localize("AllianceMember_RedMarker", "Red Marker");
    public string YellowMarker => Loc.Localize("AllianceMember_YellowMarker", "Yellow Marker");
    public string BlueMarker => Loc.Localize("AllianceMember_BlueMarker", "Blue Marker");
    public string TooltipColor => Loc.Localize("AllianceMember_TooltipColor", "Tooltip Text Color");
}

public class PlayerStrings
{
    public string Label => Loc.Localize("Player_Label", "Player");
    public string Enable => Loc.Localize("Player_Enable", "Enable");
    public string OutlineColor => Loc.Localize("Player_OutlineColor", "Cone Outline Color");
    public string FillColor => Loc.Localize("Player_FillColor", "Cone Fill Color");
    public string IconScale => Loc.Localize("Player_IconScale", "Icon Scale");
    public string ConeRadius => Loc.Localize("Player_ConeRadius", "Vision Cone Radius");
    public string ConeAngle => Loc.Localize("Player_ConeAngle", "Vision Cone Angle");
    public string ConeThickness => Loc.Localize("Player_ConeThickness", "Outline Thickness");
    public string ShowIcon => Loc.Localize("Player_ShowIcon", "Show Player Icon");
    public string ShowCone => Loc.Localize("Player_ShowCone", "Show Look Cone");
}

public class WaymarkStrings
{
    public string Label => Loc.Localize("Waymark_Label", "Waymarks");
}