using System.Collections.Generic;
using Mappy.Interfaces;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System.Commands;

public class DebugWindowCommand : IPluginCommand
{
    public string CommandArgument => "debug";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction =  () => Chat.PrintError("The debug menu cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            Hidden = true
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( Service.WindowManager.GetWindowOfType<DebugWindow>(out var debugWindow) )
                {
                    debugWindow.IsOpen = !debugWindow.IsOpen;
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            Hidden = true
        },
    };
}