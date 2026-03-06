using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyDeadState : IEnemyState
{
    private readonly IEnemy _enemy;
    private float _timer;
    private readonly float _deathDuration;
    private readonly string _deathAnimation;

    public EnemyDeadState(IEnemy enemy, string deathAnimation)
    {
        _enemy = enemy;
        _deathDuration = 1.0f;
        _deathAnimation = deathAnimation;
    }

    public void Enter()
    {
        _timer = 0f;

        // Stop movement
        _enemy.Velocity = Vector2.Zero;

        // Play death animation
        if (_enemy.Sprite != null)
        {
            _enemy.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(_deathAnimation, 0.2f);
        }
    }

    public void Exit()
    {
        // Nothing to reset for dead state
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;

        // Update sprite animation
        _enemy.Sprite?.Update(delta);

        // When timer finishes, mark for removal
        if (_timer >= _deathDuration)
        {
            _enemy.MarkForRemoval();
        }
    }

    public void HandleMovement(Vector2 direction)
    {
        // Dead enemies do not move
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        // Dead enemies cannot attack
    }
}