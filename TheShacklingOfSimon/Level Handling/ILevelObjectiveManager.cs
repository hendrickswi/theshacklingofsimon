#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.GameStates;

#endregion

namespace TheShacklingOfSimon.Level_Handling;

/// <summary>
/// Manages the objectives and win/loss conditions for a level in the game.
/// </summary>
public interface ILevelObjectiveManager
{
    /// <summary>
    /// Updates the state of the level objective manager, including checking
    /// the win and loss conditions for the current game session.
    /// </summary>
    /// <param name="delta">
    /// The elapsed game time since the last update, which can be used
    /// to handle time-based logic.
    /// </param>
    void Update(GameTime delta);

    /// <summary>
    /// Resets the internal state of the level objective manager.
    /// </summary>
    ///
    /// <remarks>This method should be called after the game is
    /// reset after a win/loss state has been pushed. Otherwise,
    /// pushing win/loss game states will not work.
    /// </remarks>
    void Reset();

    /// <summary>
    /// An event that is triggered when a new game state should
    /// be transitioned to.
    /// </summary>
    event Action<IGameState> OnTransitionRequested;
}