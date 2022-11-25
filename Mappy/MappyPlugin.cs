using Dalamud.Plugin;

namespace Mappy;

public sealed class MappyPlugin : IDalamudPlugin
{
    public string Name => "Mappy";

    public MappyPlugin(DalamudPluginInterface pluginInterface)
    {
        //pluginInterface.Create<Service>();
    }

    public void Dispose()
    {
    }
}
