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
        Service.Penumbra = new PenumbraIntegration();
        
        // Load Caches Next
        Service.Cache = new CompositeLuminaCache();

        // Load Non Critical Managers
        Service.ModuleManager = new ModuleManager();
        Service.QuestManager = new QuestManager();
        
        Service.Teleporter = new TeleportManager();
        Service.MapManager = new MapManager();
        Service.ContextMenu = new MapContextMenu();
        
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
        Service.ModuleManager.Dispose();
    }
}
