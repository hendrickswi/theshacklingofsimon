using System;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Controllers;

public interface IController 
{
    void Update();
    void ClearCommands();
    event Action<InputSchema> OnInputDetected;
}

// For the generic methods
public interface IController<T> : IController
{
    void RegisterCommand(T input, ICommand command);
    void UnregisterCommand(T input);
}
