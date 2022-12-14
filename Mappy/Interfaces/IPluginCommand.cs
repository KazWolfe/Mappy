using System.Collections.Generic;
using System.Linq;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.Interfaces;

internal interface IPluginCommand
{
    string? CommandArgument { get; }

    public void Execute(string? additionalArguments)
    {
        var matchingSubCommands = SubCommands.Where(subCommand => subCommand.GetCommand() == additionalArguments).ToList();

        if (matchingSubCommands.Count != 0)
        {
            foreach (var subCommand in matchingSubCommands)
            {
                if (subCommand.Execute())
                {
                    Chat.Print("Command", "Command Successful");
                }
            }
        }
        else
        {
            PrintCommandError(CommandArgument, additionalArguments);
        }
    }

    IEnumerable<ISubCommand> SubCommands { get; }
    
    static void PrintCommandError(string? command, string? arguments)
    {
        Chat.PrintCommandError(arguments != null
            ? $"{Strings.Command.InvalidCommand} `/mappy {command ?? "[blank]"} {arguments}`"
            : $"{Strings.Command.InvalidCommand} `/mappy {command ?? "[blank]"}`");
    }
}