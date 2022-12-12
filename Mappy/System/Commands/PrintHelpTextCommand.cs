using System.Collections.Generic;
using System.Linq;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.System.Commands;

internal class PrintHelpTextCommand : IPluginCommand
{
    public string CommandArgument => "help";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand(null, PrintCommands)
    };

    private static void PrintCommands()
    {
        foreach (var command in Service.CommandManager.Commands)
        {
            PrintSubCommands(command);
        }
    }

    private static void PrintSubCommands(IPluginCommand command)
    {
        foreach (var subCommand in command.SubCommands.GroupBy(subCommand => subCommand.GetCommand()))
        {
            Chat.Print("Help", $"/mappy {command.CommandArgument} {subCommand.Key}");
        }
    }
}