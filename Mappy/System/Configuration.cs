﻿using System;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Mappy.DataModels;

namespace Mappy.System;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public Setting<bool> KeepOpen = new(true);
    public Setting<bool> FollowPlayer = new(false); 
    public Setting<bool> LockWindow = new(false);
    public Setting<bool> HideWindowFrame = new(false);
    public Setting<bool> FadeWhenUnfocused = new(true);
    public Setting<bool> HideInDuties = new(true);
    public Setting<float> FadePercent = new(0.6f);
    
    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;
    public void Initialize(DalamudPluginInterface inputPluginInterface) => pluginInterface = inputPluginInterface;
    public void Save() => pluginInterface!.SavePluginConfig(this);
}