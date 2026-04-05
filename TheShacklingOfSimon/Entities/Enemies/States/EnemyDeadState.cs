#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyDeadState : IEnemyState
{
    private readonly IEnemy _enemy;
    private float _timer;
    private readonly float _stateDuration;

    public EnemyDeadState(IEnemy enemy, float stateDuration)
    {
        _enemy = enemy;
        _stateDuration = stateDuration;
        _timer = 0f;
    }

    public void Enter()
    {
        // Reset timer
        _timer = 0f;

        // Stop all movement
        _enemy.Velocity = Vector2.Zero;

        // Set death animation
        string spriteName = _enemy.Name + "_EnemyDeath";
        _enemy.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(spriteName, 0.2f);
        //AddPickup(new Pickup(_enemy.Position, _enemy.EnemyDrop, /* Sprite */));
    }

    public void Exit()
    {
        // Nothing to clean up for now
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;

        if (_timer >= _stateDuration)
        {
            //drop items
            _enemy.MarkForRemoval();
        }
        else
        {
            _enemy.Sprite?.Update(delta);
        }
    }

    public void HandleMovement(Vector2 movementInput)
    {
        //disable movement
        _enemy.Velocity = Vector2.Zero;
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        // No attacking when dead
    }
}