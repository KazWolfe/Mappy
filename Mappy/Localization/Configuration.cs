using CheapLoc;

namespace Mappy.Localization;

public static partial class Strings
{
    public static ConfigurationStrings Configuration { get; } = new();
}

public class ConfigurationStrings
{
    public string Label => Loc.Localize("Configuration_Label", "Configuration");
    public string GeneralSettings => Loc.Localize("Configuration_GeneralSettings", "General Settings");
    public string FollowPlayer => Loc.Localize("Configuration_FollowPlayer", "Follow Player");
    public string LockWindow => Loc.Localize("Configuration_LockWindow", "Lock Window Position");
    public string HideWindowFrame => Loc.Localize("Configuration_HideWindowFrame", "Hide Window Frame");
    public string HideInDuties => Loc.Localize("Configuration_HideInDuties", "Hide In Duties");
    public string FadeWhenUnfocused => Loc.Localize("Configuration_FadeWhenUnfocused", "Fade When Unfocused");
    public string FadePercent => Loc.Localize("Configuration_FadePercent", "Fade Percent");
    public string KeepOpen => Loc.Localize("Configuration_KeepOpen", "Keep Open");
    public string AlwaysShowToolbar => Loc.Localize("Configuration_AlwaysShowToolbar", "Always Show Toolbar");
}