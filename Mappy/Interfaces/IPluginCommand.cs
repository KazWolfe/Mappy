using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.Interfaces;

internal interface IPluginCommand
{
    string? CommandArgument { get; }

    void Execute(string? additionalArguments);

    static void PrintCommandError(string command, string? arguments)
    {
        Chat.PrintError(arguments != null
            ? $"{Strings.Command.InvalidCommand} `/mappy {command} {arguments}`"
            : $"{Strings.Command.InvalidCommand} `/mappy {command}`");
    }
}