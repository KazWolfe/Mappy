using CheapLoc;

namespace Mappy.Localization;

public static partial class Strings
{
    public static CommandStrings Command { get; } = new();
}

public class CommandStrings
{
    public string InvalidCommand => Loc.Localize("Command_InvalidCommand", "Invalid Command");
    public string CenterMapError => Loc.Localize("Command_CenterMapError", "Cannot center map when 'Follow Player' is enabled");
}