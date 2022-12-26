using Dalamud.Plugin;
using Mappy.Config;
using Mappy.System;
using Mappy.UI;

namespace Mappy;

public sealed class MappyPlugin : IDalamudPlugin
{
    public string Name => "Mappy";

    public MappyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize(Service.PluginInterface);

        KamiLib.KamiLib.Initialize(pluginInterface, Name);
        KamiLib.KamiLib.WindowManager.AddWindow(new ConfigWindow());
        KamiLib.KamiLib.WindowManager.AddWindow(new MapOverlay());

        Service.AreaMapAddon = new AreaMapAddon();
    }

    public void Dispose()
    {
        Service.AreaMapAddon.Dispose();
        
        KamiLib.KamiLib.Dispose();
    }
}
