using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dalamud.Interface.Windowing;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System;

internal class WindowManager : IDisposable
{
    private readonly WindowSystem windowSystem = new("DailyDuty");

    private readonly List<Window> windows = new()
    {
        new ConfigurationWindow(),
        new MapWindow(),
        new DebugWindow(),
    };

    public WindowManager()
    {
        foreach (var window in windows)
        {
            windowSystem.AddWindow(window);
        }

        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    private void DrawUI() => windowSystem.Draw();

    private void DrawConfigUI()
    {
        if (Service.ClientState.IsPvP)
        {
            Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
        }

        if (GetWindowOfType<ConfigurationWindow>(out var window))
        {
            window.IsOpen = true;
        }
    }

    public bool GetWindowOfType<T>([NotNullWhen(true)] out T? window)
    {
        window = windows.OfType<T>().FirstOrDefault();

        return window != null;
    }

    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

        foreach (var window in windows.OfType<IDisposable>())
        {
            window.Dispose();
        }

        windowSystem.RemoveAllWindows();
    }
}