using System;

namespace Mappy.Interfaces;

public interface ISubCommand
{
    string? GetCommand();
    void Execute();
}

public class SubCommand : ISubCommand
{
    private string? CommandKeyword { get; }
    private Action CommandAction { get; }
    private Func<bool>? CanExecute { get; }
    
    public SubCommand(string? commandKeyword, Action commandAction, Func<bool>? canExecute = null)
    {
        CommandKeyword = commandKeyword;
        CommandAction = commandAction;
        CanExecute = canExecute;
    }

    public string? GetCommand() => CommandKeyword;

    public void Execute()
    {
        if (CanExecute?.Invoke() is null or true)
        {
            CommandAction();
        }
    }
}