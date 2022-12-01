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
        
        Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(Service.PluginInterface);

        Service.Localization = new LocalizationManager();
        Service.PlaceNameCache = new LuminaCache<PlaceName>();
        Service.IconManager = new IconManager();
        Service.Teleporter = new TeleportManager();
        
        Service.WindowManager = new WindowManager();
        Service.CommandManager = new CommandManager();
        Service.MapManager = new MapManager();
    }

    public void Dispose()
    {
        Service.Localization.Dispose();
        Service.IconManager.Dispose();
        Service.Teleporter.Dispose();
        
        Service.WindowManager.Dispose();
        Service.CommandManager.Dispose();
        Service.MapManager.Dispose();
    }
}
