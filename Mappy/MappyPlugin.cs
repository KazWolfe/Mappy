using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using Mappy.System;

namespace Mappy;

public sealed class MappyPlugin : IDalamudPlugin
{
    public string Name => "Mappy";

    public MappyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        Service.PlaceNameCache = new LuminaCache<PlaceName>();
        Service.IconManager = new IconManager();
        
        Service.WindowManager = new WindowManager();
        Service.CommandManager = new CommandManager();
        Service.MapManager = new MapManager();
    }

    public void Dispose()
    {
        Service.IconManager.Dispose();
        
        Service.WindowManager.Dispose();
        Service.CommandManager.Dispose();
        Service.MapManager.Dispose();
    }
}
