using System.Collections.Generic;
using KamiLib;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using Mappy.UI;

namespace Mappy.Commands;

internal class ConfigurationWindowCommands : IPluginCommand
{
    public string? CommandArgument => null;

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The configuration window cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window"
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( KamiLib.KamiLib.WindowManager.GetWindowOfType<ConfigWindow>() is {} mainWindow )
                {
                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window"
        },
    };
}