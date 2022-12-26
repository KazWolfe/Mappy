using System.Collections.Generic;
using Mappy.Interfaces;
using Mappy.UI;
using Mappy.Util;

namespace Mappy.Commands;

internal class ConfigurationWindowCommands : IPluginCommand
{
    public string? CommandArgument => null;

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The configuration menu cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window"
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( Service.WindowManager.GetWindowOfType<ConfigWindow>() is {} mainWindow )
                {
                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window"
        },
    };
}