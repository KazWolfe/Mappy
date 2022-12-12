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
        new SubCommand("follow", () => Service.Configuration.FollowPlayer.Value = !Service.Configuration.FollowPlayer.Value),
        new SubCommand("center", () => MapRenderer.Viewport.Center = Service.MapManager.MapTextureSize / 2.0f, () => !Service.Configuration.FollowPlayer.Value),
        new SubCommand("center", () => Chat.PrintCommandError(Strings.Command.CenterMapError), () => Service.Configuration.FollowPlayer.Value),
    };
}