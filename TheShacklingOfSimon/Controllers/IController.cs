namespace TheShacklingOfSimon.Controllers;

/*
 * Base interface for methods with no parameters
 * (avoids ambiguity compiler issue).
 */
public interface IController
{
    void Update();
    void ClearCommands();
}

/*
  * Generic interface for methods that use a generic parameter.
  */
 public interface IController<T> : IController
 {
     void RegisterCommand(T input, Commands.ICommand command);
     void UnregisterCommand(T input);
 }