using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.IoC;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
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
    [PluginService] public static KeyState Keys { get; private set; } = null!;

    public static System.WindowManager WindowManager = null!;
    public static System.CommandManager CommandManager = null!;
    public static System.MapManager MapManager = null!;
    public static System.LuminaCache<PlaceName> PlaceNameCache = null!;
    public static System.IconManager IconManager = null!;
    public static System.TeleportManager Teleporter = null!;
    public static System.LocalizationManager Localization = null!;
}