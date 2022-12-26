using System.Collections.Generic;
using System.Linq;
using Mappy.System;
using Mappy.Util;

namespace Mappy.Interfaces;

public interface IPluginCommand
{
    string? CommandArgument { get; }
    
    IEnumerable<ISubCommand> SubCommands { get; }
    
    public void Execute(CommandData data)
    {
        var matchingSubCommands = SubCommands.Where(subCommand => subCommand.GetCommand() == data.SubCommand).ToList();

        if (matchingSubCommands.Any())
        {
            foreach (var _ in matchingSubCommands.Where(subCommand => subCommand.Execute(data)))
            {
                Chat.Print("Command", "Command Successful");
            }
        }
        else
        {
            Chat.PrintError($"The command '/mappy {CommandArgument} {data.SubCommand}' does not exist.");
        }
    }
}