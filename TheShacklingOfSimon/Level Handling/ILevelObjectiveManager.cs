using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.GameStates;

namespace TheShacklingOfSimon.Level_Handling;

public interface ILevelObjectiveManager
{
    void Update(GameTime delta);
    event Action<IGameState> OnTransitionRequested;
}