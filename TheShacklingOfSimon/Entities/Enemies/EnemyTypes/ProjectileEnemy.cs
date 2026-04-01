#region

using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

#endregion


namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class ProjectileEnemy : BaseEnemy
{
    private readonly IAttackBehaviour _attackBehavior;

    public ProjectileEnemy(Vector2 startPosition, IWeapon weapon, string name = "SpiderEnemy")
        : base(startPosition, weapon, name)
    {
        _attackBehavior = new ProjectileAttackBehaviour();

        _movementBehavior = new WanderMovementBehavior();
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        _attackBehavior.Execute(this, dt, targetDirection);
    }
}