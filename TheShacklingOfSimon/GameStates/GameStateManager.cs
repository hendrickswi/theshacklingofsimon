using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.GameStates;

public class GameStateManager
{
    private readonly Stack<IGameState> _states;

    public GameStateManager()
    {
        _states = new Stack<IGameState>();
    }

    public void AddState(IGameState state)
    {
        if (state == null)
        {
            return; 
        }
        _states.Push(state);
        _states.Peek().Enter();
    }

    public void RemoveTopState()
    {
        if (_states.Count <= 0)
        {
            return;
        }

        _states.Peek().Exit();
        _states.Pop();
        
        // Automatically enter the previous state (if there is one)
        if (_states.Count <= 0)
        {
            return;
            
        }
        _states.Peek().Enter();
    }

    public void Clear()
    {
        _states.Clear();
    }

    public void Update(GameTime delta)
    {
        // Only update the state at the top (if it exists)
        if (_states.Count <= 0)
        {
            return;
        }
        _states.Peek().Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch, bool drawOnlyTop)
    {
        if (_states.Count <= 0)
        {
            return;
        }

        if (drawOnlyTop)
        {
            _states.Peek().Draw(spriteBatch);
            return;
        }
        
        // Draw from bottom of stack to top for layering effect
        IGameState[] statesArray = _states.ToArray();
        for (int i = statesArray.Length - 1; i >= 0; i--)
        {
            statesArray[i].Draw(spriteBatch);
        }
    }
}