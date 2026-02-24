using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

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
        // _direction is already "cardinalized" from PlayerHeadIdleState consider changing to match movement direction
        _weapon.Fire(_enemy.Position, _direction, new ProjectileStats(1.0f, 200.0f));

        //string spriteAnimationName = "EnemyAttack";

        //_enemy.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(spriteAnimationName, 0.2f);
    }
    
    public void Exit()
    {
        // Reset any idle-specific state if necessary
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;
        if (_timer >= _stateDuration)
        {
            _enemy.ChangeState(new EnemyIdleState(_enemy, Vector2.Zero));
        }
        else
        {
            _enemy.Sprite.Update(delta);
        }
    }

    public void HandleMovement(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            _enemy.ChangeState(new EnemyMovingState(_enemy, direction));
        }
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        // no-op
    }
}