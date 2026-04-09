#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace TheShacklingOfSimon.GameStates;

public class GameStateManager
{
    private readonly Stack<IGameState> _states;

    public GameStateManager()
    {
        _states = new Stack<IGameState>();
    }

    /// <summary>
    /// Adds a new game state to the stack and transitions to it.
    /// </summary>
    /// <remarks>
    /// The current state is exited before the new state is entered.
    /// </remarks>
    /// <param name="state">The object of type <c>IGameState</c> to add and transition to.</param>
    public void AddState(IGameState state)
    {
        if (state == null) return;

        if (_states.Count > 0)
        {
            _states.Peek().Exit();
        }
        
        _states.Push(state);
        state.Enter();
    }

    /// <summary>
    /// Removes the current game state from the stack and transitions to the previous state.
    /// </summary>
    /// <remarks>
    /// The current state is exited before being removed. If a previous state exists, it is
    /// transitioned to after.
    /// </remarks>
    public void RemoveState()
    {
        if (_states.Count <= 0) return;

        _states.Peek().Exit();
        _states.Pop();
        
        if (_states.Count > 0)
        {
            _states.Peek().Enter();
        }
    }

    /// <summary>
    /// Clears all game states from the stack.
    /// </summary>
    /// <remarks>
    /// The current game state is exited before clearing.
    /// </remarks>
    public void Clear()
    {
        if (_states.Count <= 0) return;
        _states.Peek().Exit();
        _states.Clear();
    }

    /// <summary>
    /// Updates the current game state with a given time step.
    /// </summary>
    /// <param name="delta">The <c>GameTime</c> object representing the elapsed game time
    /// since the last update.
    /// </param>
    public void Update(GameTime delta)
    {
        if (_states.Count <= 0) return;
        _states.Peek().Update(delta);
    }

    /// <summary>
    /// Renders the game states onto the screen using the given <c>SpriteBatch</c>.
    /// </summary>
    /// <remarks>
    /// If <paramref name="drawOnlyTop"/> is set to true, only the topmost game state
    /// in the stack is drawn. Otherwise, all game states in the stack are drawn
    /// in the order they were added.
    /// </remarks>
    /// <param name="spriteBatch">The <c>SpriteBatch</c> used for rendering the game states.</param>
    /// <param name="drawOnlyTop">A boolean indicating whether only the topmost game state should be drawn.</param>
    public void Draw(SpriteBatch spriteBatch, bool drawOnlyTop = false)
    {
        if (_states.Count <= 0) return;

        if (drawOnlyTop)
        {
            _states.Peek().Draw(spriteBatch);
            return;
        }

        var statesArray = _states.ToArray();
        for (int i = statesArray.Length - 1; i >= 0; i--)
        {
            statesArray[i].Draw(spriteBatch);
        }
    }
}