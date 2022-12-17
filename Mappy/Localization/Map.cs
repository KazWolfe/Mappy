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
    public string Info => Loc.Localize("Map_Info", "Icons with a green border are enabled.\n" +
                                                   "Icons with a red border are disabled.\n" +
                                                   "Click on an icon to enable/disable.");

    public string RemoveFlag => Loc.Localize("Map_RemoveFlag", "Remove Flag");
    public string RemoveGatheringArea => Loc.Localize("Map_RemoveGatheringArea", "Remove Gathering Area");
    public string HideQuest => Loc.Localize("Map_HideQuest", "Hide Quest");

    public Generic Generic = new();
    public FateStrings Fate = new();
    public GatheringStrings Gathering = new();
    public MarkerStrings Markers = new();
    public PartyMemberStrings PartyMembers = new();
    public PetStrings Pets = new();
    public AllianceMemberStrings AllianceMembers = new();
    public PlayerStrings Player = new();
    public WaymarkStrings Waymarks = new();
    public TemporaryMarkerStrings TemporaryMarkers = new();
    public QuestMarkerStrings Quests = new();
    public HousingStrings Housing = new();
    public TreasureStrings Treasure = new();
    public FlagStrings Flag = new();
    public GatheringAreaStrings GatheringArea = new();
}

public class Generic
{
    public string Enable => Loc.Localize("Generic_Enable", "Enable");
    public string ShowTooltip => Loc.Localize("Generic_ShowTooltip", "Show Tooltip");
    public string ShowIcon => Loc.Localize("Generic_ShowIcon", "Show Icon");
    public string TooltipColor => Loc.Localize("Generic_TooltipColor", "Tooltip Color");
    public string IconScale => Loc.Localize("Generic_IconScale", "Icon Scale");
}

public class FateStrings
{
    public string Label => Loc.Localize("Fate_Label", "FATEs");
    public string Level => Loc.Localize("Fate_Level", "Lv.");
    public string TimeRemaining => Loc.Localize("Fate_TimeRemaining", "Time Remaining");
    public string Progress => Loc.Localize("Fate_Progress", "Progress");
    public string ShowRing => Loc.Localize("Fate_ShowRing", "Show Ring");
    public string Color => Loc.Localize("Fate_Color", "Circle Color");
    public string ExpiringWarning => Loc.Localize("Fate_ExpiringWarning", "Expiring Warning");
    public string ExpirationWarningHelp => Loc.Localize("Fate_ExpiringWarningHelp", "Changes the color of the FATE circle\nWhen the FATE is about to expire");
    public string EarlyWarningTime => Loc.Localize("Fate_EarlyWarningTime", "Early Warning Time");
    public string ExpirationColor => Loc.Localize("Fate_ExpiringColor", "Expiring Color");
    public string InSeconds => Loc.Localize("Fate_InSeconds", "Time in Seconds");
}

public class GatheringStrings
{
    public string Label => Loc.Localize("Gathering_Label", "Gathering Points");
}

public class MarkerStrings
{
    public string Label => Loc.Localize("Marker_Label", "Map Markers");
    public string AetherytesOnTop => Loc.Localize("Marker_AetherytesOnTop", "Draw Aetherytes On Top");
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
}

public class PlayerStrings
{
    public string Label => Loc.Localize("Player_Label", "Player");
    public string OutlineColor => Loc.Localize("Player_OutlineColor", "Cone Outline Color");
    public string FillColor => Loc.Localize("Player_FillColor", "Cone Fill Color");
    public string ConeRadius => Loc.Localize("Player_ConeRadius", "Vision Cone Radius");
    public string ConeAngle => Loc.Localize("Player_ConeAngle", "Vision Cone Angle");
    public string ConeThickness => Loc.Localize("Player_ConeThickness", "Outline Thickness");
    public string ShowCone => Loc.Localize("Player_ShowCone", "Show Look Cone");
    public string AngleInDegrees => Loc.Localize("Player_AngleInDegrees", "Angle in Degrees");
}

public class WaymarkStrings
{
    public string Label => Loc.Localize("Waymark_Label", "Waymarks");
}

public class TemporaryMarkerStrings
{
    public string Label => Loc.Localize("TemporaryMarker_Label", "Temporary Markers");
    public string About => Loc.Localize("TemporaryMarker_About", "This module controls the flag map marker\nand the gathering area marker");
    public string GatheringColor => Loc.Localize("TemporaryMarker_GatheringRingColor", "Gathering Ring Color");
    public string FlagScale => Loc.Localize("TemporaryMarker_FlagScale", "Flag Scale");
    public string GatheringAreaScale => Loc.Localize("TemporaryMarker_GatheringAreaScale", "Gathering Area Scale");
}

public class QuestMarkerStrings
{
    public string Label => Loc.Localize("QuestMarker_Label", "Quest Markers");
    public string Tribal => Loc.Localize("QuestMarker_Tribal", "Show Tribal");
    public string Festival => Loc.Localize("QuestMarker_Festival", "Show Festival");
    public string GrandCompany => Loc.Localize("QuestMarker_GrandCompany", "Show Grand Company");
    public string HideAccepted => Loc.Localize("QuestMarker_HideAccepted", "Hide Accepted");
    public string HideUnaccepted => Loc.Localize("QuestMarker_HideUnaccepted", "Hide Unaccepted");
    public string AcceptedScale => Loc.Localize("QuestMarker_AcceptedScale", "Accepted Scale");
    public string UnacceptedScale => Loc.Localize("QuestMarker_UnacceptedScale", "Unaccepted Scale");
    public string ObjectiveColor => Loc.Localize("QuestMarker_ObjectiveColor", "Circle Color");
    public string AcceptedColor => Loc.Localize("QuestMarker_AcceptedColor", "Accepted Color");
    public string UnacceptedColor => Loc.Localize("QuestMarker_UnacceptedColor", "Unaccepted Color");
    public string ResetBlacklist => Loc.Localize("QuestMarker_ResetBlacklist", "Reset Blacklist");
    public string RemoveFromBlacklist => Loc.Localize("QuestMarker_RemoveFromBlacklist", "Click quest to remove from blacklist");
    public string EmptyBlacklist => Loc.Localize("QuestMarker_NothingBlacklisted", "There are no quests in the blacklist\nRight click a quest icon to add to blacklist");
    public string HideRepeatable => Loc.Localize("QuestMarker_HideRepeatable", "Hide Repeatable");

}

public class HousingStrings
{
    public string Label => Loc.Localize("Housing_Label", "Housing Markers");
}

public class TreasureStrings
{
    public string Label => Loc.Localize("Treasure_Label", "Treasure Markers");
}

public class FlagStrings
{
    public string Label => Loc.Localize("Flag_Label", "Flag Marker");
}

public class GatheringAreaStrings
{
    public string Label => Loc.Localize("GatheringArea_Labeel", "Gathering Area Marker");
}