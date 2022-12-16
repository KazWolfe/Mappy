using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;
using Mappy.System;
using CommandManager = Dalamud.Game.Command.CommandManager;
using Condition = Dalamud.Game.ClientState.Conditions.Condition;

namespace Mappy;

internal class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static CommandManager Commands { get; private set; } = null!;
    [PluginService] public static ChatGui Chat { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static DataManager DataManager { get; private set; } = null!;
    [PluginService] public static ObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static Condition Condition { get; private set; } = null!;
    [PluginService] public static AetheryteList AetheryteList { get; private set; } = null!;
    [PluginService] public static ToastGui Toast { get; private set; } = null!;
    [PluginService] public static PartyList PartyList { get; private set; } = null!;

    public static CompositeLuminaCache Cache = null!;
    
    public static Configuration Configuration = null!;
    public static WindowManager WindowManager = null!;
    public static System.CommandManager CommandManager = null!;
    public static TeleportManager Teleporter = null!;
    public static LocalizationManager Localization = null!;
    public static PenumbraIntegration Penumbra = null!;
    
    public static MapManager MapManager = null!;
    public static GameIntegration GameIntegration = null!;
    public static MapContextMenu ContextMenu = null!;
    public static ModuleManager ModuleManager = null!;
    public static QuestManager QuestManager = null!;
}