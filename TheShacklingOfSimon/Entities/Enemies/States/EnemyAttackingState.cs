#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyAttackingState : IEnemyState
{
    private readonly IEnemy _enemy;
    private readonly IWeapon _weapon;
    private readonly Vector2 _direction;
    private float _timer;
    private readonly float _stateDuration;

    private const int ProjectileWidth = 8;
    private const int ProjectileHeight = 8;

    public EnemyAttackingState(IEnemy enemy, Vector2 direction, float stateDuration)
    {
        _enemy = enemy;
        _weapon = enemy.Weapon;
        _stateDuration = stateDuration;

        _direction = direction.LengthSquared() < 0.0001f
            ? new Vector2(0, 1)
            : GetCardinalDirection(direction);
    }

    public void Enter()
    {
        _timer = 0f;

        string newAnimationName = _enemy.Name + "_Attack";

        _enemy.HitboxEnabled = true;
        _enemy.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(newAnimationName, 0.5f);

        Vector2 projectileSpawnPosition = GetProjectileSpawnPosition();

        _weapon.Fire(
            projectileSpawnPosition,
            _direction,
            new ProjectileStats(1, 200.0f, ProjectileOwner.Enemy)
        );
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;

        _enemy.Sprite?.Update(delta);

        if (_timer >= _stateDuration)
        {
            _enemy.ChangeState(new EnemyIdleState(_enemy, _direction));
        }
    }

    public void HandleMovement(Vector2 direction)
    {
        _enemy.Velocity = Vector2.Zero;
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
    }

    public void HandleDamage(int damage)
    {
        if (_enemy.Health <= 0)
        {
            _enemy.ChangeState(new EnemyDeadState(_enemy, 0.5f));
        }
    }

    private Vector2 GetProjectileSpawnPosition()
    {
        Vector2 enemyCenter = new Vector2(
            _enemy.Hitbox.X + _enemy.Hitbox.Width / 2f,
            _enemy.Hitbox.Y + _enemy.Hitbox.Height / 2f
        );

        return new Vector2(
            enemyCenter.X - ProjectileWidth / 2f,
            enemyCenter.Y - ProjectileHeight / 2f
        );
    }

    private static Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (direction.LengthSquared() < 0.0001f)
        {
            return new Vector2(0, 1);
        }

        if (MathHelper.Distance(direction.X, 0f) > MathHelper.Distance(direction.Y, 0f))
        {
            return new Vector2(direction.X > 0f ? 1f : -1f, 0f);
        }

        return new Vector2(0f, direction.Y > 0f ? 1f : -1f);
    }
}