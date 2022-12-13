using System.Collections.Generic;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.System.Commands;

public class MapManagerCommand : IPluginCommand
{
    public string CommandArgument => "map";
    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "follow",
            CommandAction = () =>  Service.Configuration.FollowPlayer.Value = !Service.Configuration.FollowPlayer.Value,
            GetHelpText = () => Strings.Command.Follow
        },
        new SubCommand
        {
            CommandKeyword = "center",
            CanExecute = () => !Service.Configuration.FollowPlayer.Value,
            CommandAction = () => MapRenderer.Viewport.Center = Service.MapManager.MapTextureSize / 2.0f,
            GetHelpText = () => Strings.Command.Center
        },
        new SubCommand
        {
            CommandKeyword = "center",
            CanExecute = () => Service.Configuration.FollowPlayer.Value,
            CommandAction = () => Chat.PrintCommandError(Strings.Command.CenterMapError),
            GetHelpText = () => Strings.Command.Center
        }
    };
}