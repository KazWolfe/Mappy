using CheapLoc;

namespace Mappy.Localization;

public static partial class Strings
{
    public static TeleportStrings Teleport { get; } = new();

}

public class TeleportStrings
{
    public string Label => Loc.Localize("Teleport_Label", "Teleport");
    public string Error => Loc.Localize("Teleport_Error", "Cannot teleport in this situation");
    public string Teleporting => Loc.Localize("Teleport_Teleporting", "Teleporting to '{0}'");
    public string CommunicationError => Loc.Localize("Teleport_CommunicationError", "To use the teleport function, you must install the \"Teleporter\" plugin");
    public string NotUnlocked => Loc.Localize("Teleport_NotUnlocked", "Destination Aetheryte is not unlocked, teleport cancelled");
}