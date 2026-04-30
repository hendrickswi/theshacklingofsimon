#region

using System;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;

#endregion

namespace TheShacklingOfSimon.Controllers;

/// <summary>
/// Base interface for all controllers.
/// </summary>
/// <remarks>This interface has an incomplete set of functionality.
/// Clients should generally use <see cref="IController&lt;T&gt;"/> or more
/// specific extending interfaces such as <c>IMouseController</c> instead.
/// </remarks>
public interface IController 
{
    void Update();
    void ClearCommands();
    event Action<InputSchema> OnInputDetected;
}

/// <summary>
/// The interface for all controllers. Has the full set of generic functionality.
/// </summary>
/// <remarks>Clients can use more specific extensions of this interface, such as <c>IMouseController</c> for more
/// specific functionality.</remarks>
public interface IController<T> : IController
{
    void RegisterCommand(T input, ICommand command);
    void UnregisterCommand(T input);
}
