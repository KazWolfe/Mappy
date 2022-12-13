using System.Collections.Generic;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.System.Commands;

internal class LocalizationCommand : IPluginCommand
{
    public string CommandArgument => "loc";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "generate",
            CommandAction = () =>
            {
                Chat.Print("Command", "Generating Localization File"); 
                Service.Localization.ExportLocalization();
            },
            Hidden = true,
        }
    };
}