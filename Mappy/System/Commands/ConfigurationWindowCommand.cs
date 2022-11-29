using Mappy.Interfaces;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System.Commands;

internal class ConfigurationWindowCommand : IPluginCommand
{
    public string? CommandArgument => null;

    public void Execute(string? additionalArguments)
    {
        if ( Service.WindowManager.GetWindowOfType<ConfigurationWindow>(out var mainWindow) )
        {
            if (Service.ClientState.IsPvP)
            {
                Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
            }
            else
            {
                mainWindow.IsOpen = !mainWindow.IsOpen;
            }
        }
    }
}