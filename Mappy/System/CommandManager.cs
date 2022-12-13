using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Mappy.Interfaces;
using Mappy.System.Commands;

namespace Mappy.System;

internal class CommandManager : IDisposable
{
    private const string SettingsCommand = "/mappy";

    private const string HelpCommand = "/mappy help";

    public readonly List<IPluginCommand> Commands = new()
    {
        new ConfigurationWindowCommand(),
        new PrintHelpTextCommand(),
        new DebugWindowCommand(),
        new MapManagerCommand(),
        new LocalizationCommand(),
        new GotoCommand(),
    };

    public CommandManager()
    {
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
    }

    private void OnCommand(string command, string arguments)
    {
        PluginLog.Debug($"Received Command `{command}` `{arguments}`");

        var subCommand = GetPrimaryCommand(arguments);
        var subCommandArguments = GetSecondaryCommand(arguments);

        switch (subCommand)
        {
            case null:
                GetCommand<ConfigurationWindowCommand>()?.Execute(subCommandArguments);
                break;

            case "help":
                GetCommand<PrintHelpTextCommand>()?.Execute(subCommandArguments);
                break;

            default:
                ProcessCommand(subCommand, subCommandArguments);
                break;
        }
    }

    private IPluginCommand? GetCommand<T>()
    {
        return Commands.OfType<T>().FirstOrDefault() as IPluginCommand;
    }
    
    private void ProcessCommand(string subCommand, string? subCommandArguments)
    {
        var matchingCommands = Commands.Where(command => command.CommandArgument == subCommand).ToList();

        if (matchingCommands.Any())
        {
            foreach (var cmd in matchingCommands)
            {
                cmd.Execute(subCommandArguments);
            }
        }
        else
        {
            IPluginCommand.PrintCommandError(subCommand, subCommandArguments);

        }
    }

    private static string? GetSecondaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray.Length == 1)
        {
            return null;
        }

        return string.Join(" ", stringArray[1..]);
    }

    private static string? GetPrimaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray[0] == string.Empty)
        {
            return null;
        }

        return stringArray[0];
    }
}