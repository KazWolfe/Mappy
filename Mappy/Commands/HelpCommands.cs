using System.Collections.Generic;
using System.Linq;
using Mappy.Interfaces;
using Mappy.Util;

namespace Mappy.Commands;

public class HelpCommands: IPluginCommand
{
    public string CommandArgument => "help";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                foreach (var command in Service.CommandManager.Commands)
                {
                    PrintSubCommands(command);
                }
            },
            GetHelpText = () => "Show this message"
        }
    };

    private static void PrintSubCommands(IPluginCommand command)
    {
        foreach (var subCommand in command.SubCommands.GroupBy(subCommand => subCommand.GetCommand()))
        {
            var selectedSubCommand = subCommand.First();

            if (!selectedSubCommand.Hidden)
            {
                PrintHelpText(command, selectedSubCommand);
            }
        }
    }

    private static void PrintHelpText(IPluginCommand mainCommand, ISubCommand subCommand)
    {
        var commandString = "/mappy ";

        if (mainCommand.CommandArgument is not null)
        {
            commandString += mainCommand.CommandArgument + " ";
        }

        if (subCommand.GetCommand() is not null)
        {
            commandString += subCommand.GetCommand() + " ";
        }

        Chat.PrintHelp(commandString, subCommand.GetHelpText());
    }
}