using System.Collections.Generic;
using System.Numerics;
using Dalamud.Utility;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.Utilities;

namespace Mappy.System.Commands;

public class GotoCommand : IPluginCommand
{
    public string CommandArgument => "goto";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            GetHelpText = () => Strings.Command.Goto
        }
    };

    public void Execute(string? additionalArguments)
    {
        if (additionalArguments is not null)
        {
            var coordinateStrings = additionalArguments.Split(" ");
            var x = float.Parse(coordinateStrings[0]);
            var y = float.Parse(coordinateStrings[1]);
            
            var textureSize = Service.MapManager.MapTextureSize;

            if (Service.MapManager.Map is { } map)
            {
                var mapMax = MapUtil.WorldToMap(textureSize, map);
            
                var worldX = ConvertMapToWorld(x, Service.MapManager.Map?.SizeFactor ?? 100u, Service.MapManager.Map?.OffsetX ?? 0);
                var worldY = ConvertMapToWorld(y, Service.MapManager.Map?.SizeFactor ?? 100u, Service.MapManager.Map?.OffsetY ?? 0);

                if (worldX <= mapMax.X && worldY <= mapMax.Y && worldX >= 1.0f && worldY >= 1.0f)
                {
                    MapRenderer.SetViewportCenter(new Vector2(worldX, worldY));
                    MapRenderer.SetViewportZoom(2.0f);
                    Chat.Print("Command", "Command Successful");
                    return;
                }
            }
        }
        
        Chat.Print("Command", "Error or out of range");
    }

    private static float ConvertMapToWorld(float value, uint scale, int offset)
    {
        var scaleFactor = scale / 100.0f;
       
        return - offset * scaleFactor + 50.0f * (value - 1) * scaleFactor;
    }
}