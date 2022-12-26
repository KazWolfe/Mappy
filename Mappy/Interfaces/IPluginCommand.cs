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
        var targetSubCommand = GetSubCommand(additionalArguments);
        var matchingSubCommands = SubCommands.Where(subCommand => subCommand.GetCommand() == targetSubCommand).ToList();

        if (matchingSubCommands.Count != 0)
        {
            foreach (var subCommand in matchingSubCommands)
            {
                if (subCommand.Execute(GetSubSubCommand(additionalArguments)))
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

    private string? GetSubCommand(string? additionalArguments)
    {
        if (additionalArguments is null) return null;

        return additionalArguments.Split(" ")[0];
    }
    
    private string[]? GetSubSubCommand(string? additionalArguments)
    {
        if (additionalArguments is null) return null;
        
        var strings = additionalArguments.Split(" ");
        return strings[1..];
    }
}