using System.Collections.Generic;
using System.Numerics;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.System.Commands;

public class GotoCommand : IPluginCommand
{
    public string CommandArgument => "goto";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        Capacity = 0
    };

    public void Execute(string? additionalArguments)
    {
        if (additionalArguments is not null)
        {
            var coordinateStrings = additionalArguments.Split(" ");
            var x = float.Parse(coordinateStrings[0]);
            var y = float.Parse(coordinateStrings[1]);
            
            var worldX = ConvertMapToWorld(x, Service.MapManager.Map?.SizeFactor ?? 100u, Service.MapManager.Map?.OffsetX ?? 0);
            var worldY = ConvertMapToWorld(y, Service.MapManager.Map?.SizeFactor ?? 100u, Service.MapManager.Map?.OffsetY ?? 0);

            MapRenderer.SetViewportCenter(new Vector2(worldX, worldY));
            MapRenderer.SetViewportZoom(2.0f);
        }
    }

    private static float ConvertMapToWorld(float value, uint scale, int offset)
    {
        var scaleFactor = scale / 100.0f;
       
        return - offset * scaleFactor + 50.0f * (value - 1) * scaleFactor;
    }
}