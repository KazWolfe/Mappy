using Mappy.Interfaces;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System.Commands;

public class DebugWindowCommand : IPluginCommand
{
    public string? CommandArgument => "debug";
    public void Execute(string? additionalArguments)
    {
        if ( Service.WindowManager.GetWindowOfType<DebugWindow>(out var debugWindow) )
        {
            if (Service.ClientState.IsPvP)
            {
                Chat.PrintError("The map cannot be opened while in a PvP area");
            }
            else
            {
                debugWindow.IsOpen = !debugWindow.IsOpen;
            }
        }
    }

}