using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace Mappy.Config;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 2;
    
    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;
    public void Initialize(DalamudPluginInterface inputPluginInterface) => pluginInterface = inputPluginInterface;
    public void Save() => pluginInterface!.SavePluginConfig(this);
}