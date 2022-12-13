using System.Collections.Generic;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System.Commands;

internal class ConfigurationWindowCommand : IPluginCommand
{
    public string? CommandArgument => null;

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The configuration menu cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => Strings.Command.OpenConfigWindow
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( Service.WindowManager.GetWindowOfType<ConfigurationWindow>(out var mainWindow) )
                {
                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => Strings.Command.OpenConfigWindow
        },
    };
}