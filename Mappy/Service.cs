using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Mappy.Config;
using Mappy.System;
using DalamudCommandManager = Dalamud.Game.Command.CommandManager;

namespace Mappy;

internal class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static DalamudCommandManager Commands { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static ChatGui Chat { get; private set; } = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;

    public static Configuration Configuration = null!;
    public static WindowManager WindowManager = null!;
    public static CommandManager CommandManager = null!;
}