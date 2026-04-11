#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyAttackingState : IEnemyState
{
    private IEnemy _enemy;
    private IWeapon _weapon;
    private Vector2 _direction;
    private float _timer;
    private readonly float _stateDuration;

    public EnemyAttackingState(IEnemy enemy, Vector2 direction, float stateDuration)
    {
        _enemy = enemy;
        _weapon = enemy.Weapon;
        _stateDuration = stateDuration;
        // Default to looking down
        _direction = (direction.LengthSquared() < 0.0001f) ? new Vector2(0, 1) : direction;
    }
    public void Enter()
    {
        _timer = 0f;

        string newAnimationName = _enemy.Name + "_Attack";

        _enemy.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(newAnimationName, 0.2f);

        // _direction is already "cardinalized" from PlayerHeadIdleState consider changing to match movement direction
        _weapon.Fire(_enemy.Position, _direction, new ProjectileStats(1, 200.0f, ProjectileOwner.Enemy));
    }
    
    public void Exit()
    {
        // Reset any idle-specific state if necessary
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;

        _enemy.Sprite?.Update(delta);

        if (_timer >= _stateDuration)
        {
            // Return to idle after attack finishes
            _enemy.ChangeState(new EnemyIdleState(_enemy, _direction));
        }
    }

    public void HandleMovement(Vector2 direction)
    {
        //disable movement while attacking
        _enemy.Velocity = Vector2.Zero;
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        // no-op
    }

    public void HandleDamage(int damage)
    {
        if (_enemy.Health <= 0)
        {
            _enemy.ChangeState(new EnemyDeadState(_enemy, 2.5f));
        }
        // else ignore damage reaction
    }
}