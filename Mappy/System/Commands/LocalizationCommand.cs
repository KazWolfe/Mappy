using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.System.Commands
{
    internal class LocalizationCommand : IPluginCommand
    {
        public string CommandArgument => "loc";

        public void Execute(string? additionalArguments)
        {
            switch (additionalArguments)
            {
                case "generate":
                    Chat.Print("Command", "Generating Localization File");
                    Service.Localization.ExportLocalization();
                    break;
                
                default:
                    Chat.Print("Command", "Invalid Localization Command");
                    break;
            }
        }
    }
}
