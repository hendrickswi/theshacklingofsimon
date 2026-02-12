using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Players.States;

public interface IPlayerBodyState
{
    void Enter();
    void Exit();
    void Update(GameTime delta);
    void HandleMovement(Vector2 direction);
}