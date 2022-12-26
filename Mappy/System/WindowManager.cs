using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;
using Mappy.UI;
using Mappy.Util;

namespace Mappy.System;

public class WindowManager : IDisposable
{
    private readonly WindowSystem windowSystem = new("Mappy");

    private readonly List<Window> windows;

    public WindowManager()
    {
        windows = new List<Window>
        {
            new ConfigWindow(),
        };
        
        windows.ForEach(window => windowSystem.AddWindow(window));
        
        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }
    
    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

        windowSystem.RemoveAllWindows();
    }

    public T? GetWindowOfType<T>() => windows.OfType<T>().FirstOrDefault();

    private void DrawUI() => windowSystem.Draw();
    
    private void DrawConfigUI()
    {
        if (Service.ClientState.IsPvP)
        {
            Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
        }

        if (GetWindowOfType<ConfigWindow>() is { } window)
        {
            window.IsOpen = true;
        }
    }

}