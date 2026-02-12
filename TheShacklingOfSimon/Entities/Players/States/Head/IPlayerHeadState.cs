using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Players.States;

public interface IPlayerHeadState
{
    void Enter();
    void Exit();
    void Update(GameTime delta);
    void HandleAttack(Vector2 direction);
    void HandleAttackSecondary(Vector2 direction);
}