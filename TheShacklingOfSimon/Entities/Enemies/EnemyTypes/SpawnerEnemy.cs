#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.AttackBehaviours;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;
using TheShacklingOfSimon.Entities.Enemies.Managers;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class SpawnerEnemy : BaseEnemy
{
    private readonly IAttackBehaviour _contactAttackBehaviour;
    private float _spawnTimer;
    private readonly float _spawnInterval;

    public SpawnerEnemy(Vector2 startPosition, IWeapon weapon, string name = "Cohort")
        : base(startPosition, weapon, name)
    {
        _contactAttackBehaviour = new AllowedAttackBehaviour();
        _movementBehaviour = new NoMovementBehaviour();
        _spawnInterval = 2f;
        _spawnTimer = _spawnInterval;
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        _spawnTimer -= dt;
        if (_spawnTimer > 0f)
        {
            return;
        }

        _spawnTimer = _spawnInterval;

        Vector2 spawnPosition = Position + new Vector2(0, Hitbox.Height + 4);
        IEnemy spawnedEnemy = new ProjectileEnemy(
            spawnPosition,
            EnemyWeaponFactory.CreateWeapon(WeaponTypeList.BasicWeapon),
            "SpiderEnemy"
        );

        if (spawnedEnemy is BaseEnemy spawnedBase)
        {
            spawnedBase.SetTargetPlayer(_targetPlayer);
            spawnedBase.SetPathfindingService(_pathfindingService);
        }

        SpawnEnemy(spawnedEnemy);
    }

    public override void OnCollision(IPlayer player)
    {
        if (player == null || !IsActive) return;

        _contactAttackBehaviour.Execute(this, 0, Vector2.Zero);
        player.TakeDamage((int)ContactDamage);
    }
}