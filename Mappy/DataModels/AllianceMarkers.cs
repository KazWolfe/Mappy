using System;
using Mappy.Localization;

namespace Mappy.DataModels;

public enum AllianceMarkers
{
    Green = 60358,
    Red = 60359,
    Yellow = 60360,
    Blue = 60361
}

public static class AllianceMarkersExtensions
{
    public static string GetTranslatedString(this AllianceMarkers marker)
    {
        return marker switch
        {
            AllianceMarkers.Green => Strings.Map.AllianceMembers.GreenMarker,
            AllianceMarkers.Red => Strings.Map.AllianceMembers.RedMarker,
            AllianceMarkers.Yellow => Strings.Map.AllianceMembers.YellowMarker,
            AllianceMarkers.Blue => Strings.Map.AllianceMembers.BlueMarker,
            _ => throw new ArgumentOutOfRangeException(nameof(marker), marker, null)
        };
    }
}