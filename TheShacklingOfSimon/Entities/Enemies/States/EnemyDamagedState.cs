#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyDamagedState : IEnemyState
{
    private IEnemy _enemy;
    private float _timer;
    private readonly float _stateDuration;

    public EnemyDamagedState(IEnemy enemy, float stateDuration)
    {
        _enemy = enemy;
        _stateDuration = stateDuration;
        _timer = 0f;
    }

    public void Enter()
    {
        // Set hurt sprite
        string spriteName = _enemy.Name + "_Hurt";
        _enemy.Sprite = SpriteFactory.Instance.CreateStaticSprite(spriteName);

        SoundManager.Instance.PlaySFX(_enemy.HurtSFX);

        // Optional: stop movement when hit
        _enemy.Velocity = Vector2.Zero;
    }

    public void Exit()
    {
        // Nothing for now
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;

        _enemy.Sprite?.Update(delta);

        if (_timer >= _stateDuration)
        {
            // Return to idle after damage
            _enemy.ChangeState(new EnemyIdleState(_enemy, new Vector2(0, 1)));
        }
    }

    public void HandleMovement(Vector2 movementInput)
    {
        //disable movement while damaged
        _enemy.Velocity = Vector2.Zero;
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        // no op
    }

    public void HandleDamage(int damage)
    {
        if (_enemy.Health <= 0)
        {
            _enemy.ChangeState(new EnemyDeadState(_enemy, 2.5f));
        }
        // else ignore (invulnerability window)
    }
}