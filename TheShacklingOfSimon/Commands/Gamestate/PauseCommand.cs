using System;
using TheShacklingOfSimon.GameStates;

namespace TheShacklingOfSimon.Commands.Gamestate;

public class PauseCommand : ICommand
{
    private readonly GameStateManager _stateManager;
    private readonly Func<IGameState> _pauseStateFactory;

    public PauseCommand(GameStateManager stateManager, Func<IGameState> pauseStateFactory)
    {
        _stateManager = stateManager;
        _pauseStateFactory = pauseStateFactory;
    }

    public void Execute()
    {
        _stateManager.AddState(_pauseStateFactory());
    }
}