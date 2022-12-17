using System;
using Mappy.Localization;

namespace Mappy.DataModels;

public enum ComponentName
{
    Fate,
    GatheringPoint,
    MapMarker,
    PartyMember,
    Pet,
    AllianceMember,
    Player,
    Waymark,
    General,
    TemporaryMarker,
    FlagMarker,
    GatheringArea,
    QuestMarker,
    Housing,
    Treasure,
}

public static class ComponentNameExtensions
{
    public static string GetTranslatedString(this ComponentName value)
    {
        return value switch
        {
            ComponentName.Fate => Strings.Map.Fate.Label,
            ComponentName.GatheringPoint => Strings.Map.Gathering.Label,
            ComponentName.MapMarker => Strings.Map.Markers.Label,
            ComponentName.PartyMember => Strings.Map.PartyMembers.Label,
            ComponentName.Pet => Strings.Map.Pets.Label,
            ComponentName.AllianceMember => Strings.Map.AllianceMembers.Label,
            ComponentName.Player => Strings.Map.Player.Label,
            ComponentName.Waymark => Strings.Map.Waymarks.Label,
            ComponentName.General => Strings.Configuration.Label,
            ComponentName.TemporaryMarker => Strings.Map.TemporaryMarkers.Label,
            ComponentName.QuestMarker => Strings.Map.Quests.Label,
            ComponentName.Housing => Strings.Map.Housing.Label,
            ComponentName.Treasure => Strings.Map.Treasure.Label,
            ComponentName.FlagMarker => Strings.Map.Flag.Label,
            ComponentName.GatheringArea => Strings.Map.GatheringArea.Label,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}