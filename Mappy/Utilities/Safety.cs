using System;
using Dalamud.Logging;

namespace Mappy.Utilities;

public static class Safety
{
    public static void ExecuteSafe(Action action, string? message = null) 
    {
        try 
        {
            action();
        } 
        catch (Exception exception)
        {
            PluginLog.Error(exception, message ?? "Caught Exception Safely");
        }
    }
}