namespace TheShacklingOfSimon.Controllers;

public interface IController<T>
{
    void ClearCommands();
    void Update();
    void RegisterCommand(T input, Commands.ICommand command);
    void UnregisterCommand(T input);
}
 