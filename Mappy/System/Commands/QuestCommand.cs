using System.Collections.Generic;
using Mappy.Interfaces;

namespace Mappy.System.Commands;

public class QuestCommand : IPluginCommand
{
    public string CommandArgument => "quest";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "debug",
            CommandAction = () =>
            {
                var debugMode = Service.Configuration.QuestMarkers.DebugMode;
                debugMode.Value = !debugMode.Value;
            },
            Hidden = true,
        }
    };
}