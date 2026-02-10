namespace TheShacklingOfSimon.Controllers;

/*
 * Generic interface to allow client class (Game1) to link
 * inputs and commands easily.
 */
public interface IController<T>
{
    void Update();
    void RegisterCommand(T input, Commands.ICommand command);
}