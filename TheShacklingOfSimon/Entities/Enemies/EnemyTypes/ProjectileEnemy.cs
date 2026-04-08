#region

using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion


namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class ProjectileEnemy : BaseEnemy
{
    private readonly IAttackBehaviour _attackBehaviour;

    public ProjectileEnemy(Vector2 startPosition, IWeapon weapon, string name = "SpiderEnemy")
        : base(startPosition, weapon, name)
    {
        _attackBehaviour = new AllowedAttackBehaviour();

        _movementBehaviour = new WanderMovementBehavior();
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        _attackBehaviour.Execute(this, dt, targetDirection);
    }
}