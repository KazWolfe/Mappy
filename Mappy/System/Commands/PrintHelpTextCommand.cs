using Mappy.Interfaces;

namespace Mappy.System.Commands;

internal class PrintHelpTextCommand : IPluginCommand
{
    public string CommandArgument => "help";

    public void Execute(string? additionalArguments)
    {
        switch (additionalArguments)
        {
            case null:
                break;

            default:
                IPluginCommand.PrintCommandError(CommandArgument, additionalArguments);
                break;
        }
    }
}