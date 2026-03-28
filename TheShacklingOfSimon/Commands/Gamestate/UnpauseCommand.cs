using TheShacklingOfSimon.GameStates;

namespace TheShacklingOfSimon.Commands.Gamestate;

public class UnpauseCommand : ICommand
{
    private readonly GameStateManager _stateManager;

    public UnpauseCommand(GameStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    public void Execute()
    {
        _stateManager.RemoveTopState();
    }
}