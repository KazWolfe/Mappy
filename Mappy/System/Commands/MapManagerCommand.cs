﻿using Mappy.Interfaces;

namespace Mappy.System.Commands;

public class MapManagerCommand : IPluginCommand
{
    public string CommandArgument => "manager";
    public void Execute(string? additionalArguments)
    {
        switch (additionalArguments)
        {
            case "follow":
                Service.Configuration.FollowPlayer.Value = !Service.Configuration.FollowPlayer.Value;
                break;
        }
    }
}