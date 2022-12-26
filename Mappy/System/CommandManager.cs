using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.Logging;
using KamiLib;
using KamiLib.Interfaces;
using Mappy.Commands;

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
    }

    private void OnCommand(string command, string arguments)
    {
        PluginLog.Debug($"Received Command `{command}` `{arguments}`");

        var commandData = Command.GetCommandData(command, arguments);
        switch (commandData)
        {
            case {Command: null}:
                GetCommand<ConfigurationWindowCommands>()?.Execute(commandData);
                break;
            
            case {Command: "help"}:
                GetCommand<HelpCommands>()?.Execute(commandData);
                break;
            
            default:
                Command.ProcessCommand(commandData, Commands);
                break;
        }
    }
    
    private IPluginCommand? GetCommand<T>() => Commands.OfType<T>().FirstOrDefault() as IPluginCommand;
}
