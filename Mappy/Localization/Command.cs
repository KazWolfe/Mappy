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
    public string OpenConfigWindow => Loc.Localize("Command_OpenConfigWindow", "Open configuration window");
    public string Help => Loc.Localize("Command_Help", "Show this message");
    public string Follow => Loc.Localize("Command_Follow", "Toggles follow player");
    public string Center => Loc.Localize("Command_Center", "Moves view to center of the map");
    public string Goto => Loc.Localize("Command_Goto", "[/mappy goto ## ##] Centers view on specified map coordinates");
}