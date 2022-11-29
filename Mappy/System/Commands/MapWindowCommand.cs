using Mappy.Interfaces;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System.Commands;

public class MapWindowCommand : IPluginCommand
{
    public string? CommandArgument => "map";
    public void Execute(string? additionalArguments)
    {
        if ( Service.WindowManager.GetWindowOfType<MapWindow>(out var mapWindow) )
        {
            if (Service.ClientState.IsPvP)
            {
                Chat.PrintError("The map cannot be opened while in a PvP area");
            }
            else
            {
                mapWindow.IsOpen = !mapWindow.IsOpen;
            }
        }
    }
}