using Dalamud.Game.Text.SeStringHandling;

namespace Mappy.Utilities;

internal static class Chat
{
    public static void Print(string tag, string message)
    {
        var stringBuilder = new SeStringBuilder();
        stringBuilder.AddUiForeground(45);
        stringBuilder.AddText("[Mappy] ");
        stringBuilder.AddUiForegroundOff();
        stringBuilder.AddUiForeground(62);
        stringBuilder.AddText($"[{tag}] ");
        stringBuilder.AddUiForegroundOff();
        stringBuilder.AddText(message);

        Service.Chat.Print(stringBuilder.BuiltString);
    }
    
    public static void PrintHelpText(string command, string? helpText = null)
    {
        var stringBuilder = new SeStringBuilder();
        stringBuilder.AddUiForeground("[Mappy] ", 45);
        stringBuilder.AddUiForeground("[Command] ", 62);
        stringBuilder.AddText(command);

        if (helpText is not null)
        {
            stringBuilder.AddUiForeground("- " + helpText, 32);
        }

        Service.Chat.Print(stringBuilder.BuiltString);
    }

    public static void PrintError(string message)
    {
        var stringBuilder = new SeStringBuilder();
        stringBuilder.AddUiForeground(45);
        stringBuilder.AddText("[Mappy] ");
        stringBuilder.AddUiForegroundOff();
        stringBuilder.AddText(message);

        Service.Chat.PrintError(stringBuilder.BuiltString);
    }
    
    public static void PrintCommandError(string message)
    {
        var stringBuilder = new SeStringBuilder();
        stringBuilder.AddUiForeground(45);
        stringBuilder.AddText("[Mappy] ");
        stringBuilder.AddUiForegroundOff();
        stringBuilder.AddUiForeground(62);
        stringBuilder.AddText($"[Command] ");
        stringBuilder.AddUiForegroundOff();
        stringBuilder.AddText(message);

        Service.Chat.PrintError(stringBuilder.BuiltString);
    }
}