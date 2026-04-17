#region

using System;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;

#endregion

namespace TheShacklingOfSimon.Controllers;

/// <summary>
/// Base interface for all controllers.
/// <remarks>This interface has an incomplete set of functionality.
/// Clients should generally use <see cref="IController&lt;T&gt;"/> instead.
/// </remarks>
/// </summary>
public interface IController 
{
    void Update();
    void ClearCommands();
    event Action<InputSchema> OnInputDetected;
}

/// <summary>
/// The interface for all controllers. Has the full set of functionality.
/// </summary>
public interface IController<T> : IController
{
    void RegisterCommand(T input, ICommand command);
    void UnregisterCommand(T input);
}
