using Dalamud.Plugin;
using Mappy.Config;
using Mappy.System;
using Mappy.Util;

namespace Mappy;

public sealed class MappyPlugin : IDalamudPlugin
{
    public string Name => "Mappy";

    public MappyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(Service.PluginInterface);

        Service.CommandManager = new CommandManager();
        Service.WindowManager = new WindowManager();
        Service.AreaMapAddon = new AreaMapAddon();
    }

    public void Dispose()
    {
        Service.AreaMapAddon.Dispose();
        Service.WindowManager.Dispose();
        Service.CommandManager.Dispose();
    }
}
