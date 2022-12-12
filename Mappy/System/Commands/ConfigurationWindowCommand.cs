using System.Collections.Generic;
using Mappy.Interfaces;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System.Commands;

internal class ConfigurationWindowCommand : IPluginCommand
{
    public string? CommandArgument => null;

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand(null, () => Chat.PrintError("The configuration menu cannot be opened while in a PvP area"), () => Service.ClientState.IsPvP),
        new SubCommand(null, OpenConfigurationWindow, () => !Service.ClientState.IsPvP),
    };

    private static void OpenConfigurationWindow()
    {
        if ( Service.WindowManager.GetWindowOfType<ConfigurationWindow>(out var mainWindow) )
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        }
    }
}