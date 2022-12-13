using System;

namespace Mappy.Interfaces;

public interface ISubCommand
{
    string? GetCommand();
    void Execute();
    string? GetHelpText();
    bool Hidden { get; }
}

public class SubCommand : ISubCommand
{
    public string? CommandKeyword { get; set; }
    public Action? CommandAction { get; set; }
    public Func<bool>? CanExecute { get; set; }
    public Func<string>? GetHelpText { get; set; }
    public bool Hidden { get; init; }

    public string? GetCommand() => CommandKeyword;
    string? ISubCommand.GetHelpText() => GetHelpText?.Invoke();

    public void Execute()
    {
        if (CanExecute?.Invoke() is null or true)
        {
            CommandAction?.Invoke();
        }
    }
}