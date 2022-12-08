using CheapLoc;

namespace Mappy.Localization;

public static partial class Strings
{
    public static ConfigurationStrings Configuration { get; } = new();
}

public class ConfigurationStrings
{
    public string Label => Loc.Localize("Configuration_Label", "General Configuration");
    public string GeneralSettings => Loc.Localize("Configuration_GeneralSettings", "General Settings");
    public string LockWindow => Loc.Localize("Configuration_LockWindow", "Lock Window Position");
    public string HideWindowFrame => Loc.Localize("Configuration_HideWindowFrame", "Hide Window Frame");
    public string HideInDuties => Loc.Localize("Configuration_HideInDuties", "Hide In Duties");
    public string FadeWhenUnfocused => Loc.Localize("Configuration_FadeWhenUnfocused", "Fade When Unfocused");
    public string FadePercent => Loc.Localize("Configuration_FadePercent", "Fade Percent");
    public string KeepOpen => Loc.Localize("Configuration_KeepOpen", "Keep Open");
    public string AlwaysShowToolbar => Loc.Localize("Configuration_AlwaysShowToolbar", "Always Show Toolbar");
    public string ModuleNotSelected => Loc.Localize("Configuration_ModuleNotSelected", "Select an item to configure in the left pane");
    public string FeatureToggles => Loc.Localize("Configuration_FeatureToggles", "Feature Toggles");
    public string ColorOptions => Loc.Localize("Configuration_ColorOptions", "Color Options");
    public string Adjustments => Loc.Localize("Configuration_Adjustments", "Adjustments");
    public string IconSelect => Loc.Localize("Configuration_IconSelect", "Icon Selection");
    public string Reset => Loc.Localize("Configuration_Reset", "Reset to Default");
    public string Default => Loc.Localize("Configuration_Default", "Default");
    public string SelectedIcon => Loc.Localize("Configuration_SelectedIcon", "Selected Icon");
    public string RenderAboveGameUI => Loc.Localize("Generic_RenderAboveGameUI", "Displaying Above Game UI");
    public string RenderAboveGameUIHelp => Loc.Localize("Generic_RenderAboveGameUIHelp", "It is not possible to display Mappy's visual components below the game's user interface.");
    public string RenderAboveGameUIHelpExtended => Loc.Localize("Generic_RenderAboveGameUIHelpExtended", "This is a technical limitation of the tools we use to develop plugin user interfaces.");
}