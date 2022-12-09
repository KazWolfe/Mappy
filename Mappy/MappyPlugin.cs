using Dalamud.Plugin;
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

        // Load Localization First
        Service.Localization = new LocalizationManager();
        
        // Load Caches Next
        Service.Cache = new CompositeLuminaCache();

        // Load Non Critical Managers
        Service.Teleporter = new TeleportManager();
        Service.MapManager = new MapManager();
        
        // Load Critical Managers
        Service.CommandManager = new CommandManager();
        Service.WindowManager = new WindowManager();
        Service.GameIntegration = new GameIntegration();
    }

    public void Dispose()
    {
        Service.Localization.Dispose();
        Service.Teleporter.Dispose();
        Service.CommandManager.Dispose();
        Service.WindowManager.Dispose();
        Service.MapManager.Dispose();
        Service.Cache.Dispose();
        Service.GameIntegration.Dispose();
    }
}
