using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.UserInterface.Windows;

public class ConfigurationWindow : Window
{
    private readonly SelectionFrame selectionFrame;
    private readonly ConfigurationFrame configurationFrame;

    private readonly List<IModuleSettings> selectables = new()
    {
        new GeneralOptions(),
    };

    public ConfigurationWindow() : base("Mappy Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(610, 350),
            MaximumSize = new Vector2(9999,9999)
        };

        selectables.AddRange(Service.ModuleManager.GetModuleSettings());
        
        selectionFrame = new SelectionFrame(selectables);
        configurationFrame = new ConfigurationFrame();
    }

    public override void Draw()
    {
        selectionFrame.Draw();

        configurationFrame.Draw(selectionFrame.Selected);

        if (ImGui.IsItemHovered())
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        
        AboutWindow.DrawInfoButton();
    }
}

public class GeneralOptions : IModuleSettings
{
    public ComponentName ComponentName => ComponentName.General;

    public void DrawSettings()
    {
        var lastIntegrationState = Service.Configuration.EnableIntegrations.Value;
        
        InfoBox.Instance
            .AddTitle(Strings.Configuration.GeneralSettings)
            .AddString(Strings.Configuration.RenderAboveGameUI)
            .AddDummy(1.0f)
            .AddString(Strings.Configuration.RenderAboveGameUIHelp, Colors.Orange)
            .AddDummy(1.0f)
            .AddString(Strings.Configuration.RenderAboveGameUIHelpExtended, Colors.Orange)
            .AddDummy(4.0f)
            .AddSeparator()
            .AddDummy(6.0f)
            .AddConfigCheckbox(Strings.Configuration.EnableIntegrations, Service.Configuration.EnableIntegrations, Strings.Configuration.IntegrationsHelp)
            .AddDummy(6.0f)
            .AddConfigCheckbox(Strings.Configuration.KeepOpen, Service.Configuration.KeepOpen)
            .AddConfigCheckbox(Strings.Configuration.HideBetweenAreas, Service.Configuration.HideBetweenAreas)
            .AddConfigCheckbox(Strings.Configuration.LockWindow, Service.Configuration.LockWindow)
            .AddConfigCheckbox(Strings.Configuration.HideWindowFrame, Service.Configuration.HideWindowFrame)
            .AddConfigCheckbox(Strings.Configuration.HideInDuties, Service.Configuration.HideInDuties)
            .AddConfigCheckbox(Strings.Configuration.HideDuringCombat, Service.Configuration.HideInCombat)
            .AddConfigCheckbox(Strings.Configuration.AlwaysShowToolbar, Service.Configuration.AlwaysShowToolbar)
            .AddConfigCheckbox(Strings.Configuration.FadeWhenUnfocused, Service.Configuration.FadeWhenUnfocused)
            .AddDragFloat(Strings.Configuration.FadePercent, Service.Configuration.FadePercent, 0.0f, 1.0f, InfoBox.Instance.InnerWidth / 2.0f)
            .Draw();
        
        // Handle settings that aren't compatible with each other
        if (Service.Configuration.HideWindowFrame.Value)
        {
            Service.Configuration.LockWindow.Value = true;
            Service.Configuration.Save();
        }

        if (lastIntegrationState != Service.Configuration.EnableIntegrations.Value)
        {
            if (Service.Configuration.EnableIntegrations.Value)
            {
                PluginLog.Debug("Enabling Game Integrations");
                Service.GameIntegration.Enable();
            }
            else
            {
                PluginLog.Debug("Disabling Game Integrations");
                Service.GameIntegration.Disable();
            }
        }
    }
}