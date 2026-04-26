#region

using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.AttackBehaviours;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class AxisEnemy : BaseEnemy
{
    private readonly IAttackBehaviour _rangedAttackBehaviour;
    private readonly IAttackBehaviour _contactAttackBehaviour;

    public AxisEnemy(Vector2 startPosition, IWeapon weapon, string name)
        : base(startPosition, weapon, name)
    {
        _rangedAttackBehaviour = new NoAttackBehaviour();
        _contactAttackBehaviour = new AllowedAttackBehaviour();

        if (name == "BlindCreep")
        {
            _movementBehaviour = new OneAxisPatrolMovementBehavior(MovementAxis.Y);
        }
        else
        {
            _movementBehaviour = new OneAxisPatrolMovementBehavior(MovementAxis.X);
        }
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        _rangedAttackBehaviour.Execute(this, dt, targetDirection);
    }

    public override void OnCollision(IPlayer player)
    {
        if (player == null || !IsActive) return;

        _contactAttackBehaviour.Execute(this, 0, Vector2.Zero);
        player.TakeDamage((int)ContactDamage);
    }
}