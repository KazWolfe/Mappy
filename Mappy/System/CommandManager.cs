using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Mappy.Commands;
using Mappy.Interfaces;
using Mappy.Util;

namespace Mappy.System;

public class CommandManager : IDisposable
{
    private const string SettingsCommand = "/mappy";
    private const string HelpCommand = "/mappy help";

    public readonly List<IPluginCommand> Commands;
    
    public CommandManager()
    {
        Commands = new List<IPluginCommand>
        {
            new ConfigurationWindowCommands(),
            new HelpCommands()
        };
        
        Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "open configuration window"
        });

        Service.Commands.AddHandler(HelpCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "display a list of all available sub-commands"
        });
    }
    
    public void Dispose()
    {
        Service.Commands.RemoveHandler(SettingsCommand);
        Service.Commands.RemoveHandler(HelpCommand);
        
        // ReSharper disable once SuspiciousTypeConversion.Global
        foreach (var disposableCommand in Commands.OfType<IDisposable>())
        {
            disposableCommand.Dispose();
        }
    }

    private void OnCommand(string command, string arguments)
    {
        PluginLog.Debug($"Received Command `{command}` `{arguments}`");

        var commandData = GetCommandData(arguments);
        switch (commandData)
        {
            case {Command: null}:
                GetCommand<ConfigurationWindowCommands>()?.Execute(commandData);
                break;
            
            case {Command: "help"}:
                GetCommand<HelpCommands>()?.Execute(commandData);
                break;
            
            default:
                ProcessCommand(commandData);
                break;
        }
    }
    
    private void ProcessCommand(CommandData data)
    {
        var matchingCommands = Commands.Where(command => command.CommandArgument == data.Command).ToList();

        if (matchingCommands.Any())
        {
            foreach (var cmd in matchingCommands)
            {
                cmd.Execute(data);
            }
        }
        else
        {
            Chat.PrintError($"The command '/mappy {data.Command}' does not exist.");
        }
    }
    
    private IPluginCommand? GetCommand<T>() => Commands.OfType<T>().FirstOrDefault() as IPluginCommand;

    private CommandData GetCommandData(string arguments) => new(arguments);
}

public class CommandData
{
    public string? Command;
    public string? SubCommand;
    public string?[]? Arguments;
    
    public CommandData(string arguments)
    {
        if (arguments != string.Empty)
        {
            var splits = arguments.Split(' ');

            if (splits.Length >= 1)
            {
                Command = splits[0];
            }

            if (splits.Length >= 2)
            {
                SubCommand = splits[1];
            }

            if (splits.Length >= 3)
            {
                Arguments = splits[2..];
            }
        }
    }

    public override string ToString() => $"{Command ?? "Empty Command"}, " +
                                         $"{SubCommand ?? "Empty SubCommand"}, " +
                                         $"{(Arguments is null ? "Empty Args" : string.Join(", ", Arguments))}";
}