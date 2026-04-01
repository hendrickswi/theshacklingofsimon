#region

using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

#endregion


namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class ChaseEnemy : BaseEnemy
{
    private readonly IAttackBehaviour _attackBehavior;

    public ChaseEnemy(Vector2 startPosition, IWeapon weapon, string name = "BlackMaw")
        : base(startPosition, weapon, name)
    {
        _attackBehavior = new NoAttackBehaviour();
        _movementBehavior = new WanderMovementBehavior();
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        _attackBehavior.Execute(this, dt, targetDirection);
    }
}