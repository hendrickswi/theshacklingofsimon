using System;

namespace TheShacklingOfSimon.Commands;

public class GenericActionCommand : ICommand
{
    private readonly Action _action;

    public GenericActionCommand(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action?.Invoke();
    }
}