using System;

namespace Mappy.Interfaces;

public interface ISubCommand
{
    string? GetCommand();
    bool Execute(string[]? parameters = null);
    string? GetHelpText();
    bool Hidden { get; }
}

public class SubCommand : ISubCommand
{
    public string? CommandKeyword { get; init; }
    public Action? CommandAction { get; init; }
    public Action<string[]?>? ParameterAction { get; init; }
    public Func<bool>? CanExecute { get; init; }
    public Func<string>? GetHelpText { get; init; }
    public bool Hidden { get; init; }

    public string? GetCommand() => CommandKeyword;
    string? ISubCommand.GetHelpText() => GetHelpText?.Invoke();

    public bool Execute(string[]? parameters = null)
    {
        if (CanExecute?.Invoke() is null or true)
        {
            if (CommandAction is not null)
            {
                CommandAction.Invoke();
                return true;
            }

            if (ParameterAction is not null)
            {
                ParameterAction.Invoke(parameters);
                return true;
            }
        }

        return false;
    }
}