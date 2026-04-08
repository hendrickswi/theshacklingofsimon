#region

using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;
using TheShacklingOfSimon.Entities.Enemies.States;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion


namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class ChaseEnemy : BaseEnemy
{
    private readonly IAttackBehaviour _rangedAttackBehaviour;
    private readonly IAttackBehaviour _contactAttackBehaviour;

    public ChaseEnemy(Vector2 startPosition, IWeapon weapon, string name = "BlackMaw")
        : base(startPosition, weapon, name)
    {
        _rangedAttackBehaviour = new NoAttackBehaviour();
        _contactAttackBehaviour = new AllowedAttackBehaviour();
        _movementBehaviour = new WanderMovementBehavior();
    }

    public override void RegisterAttack(float dt, Vector2 targetDirection)
    {
        _rangedAttackBehaviour.Execute(this, dt, targetDirection);
    }

    public override void OnCollision(IPlayer player)
    {
        if (player == null || !IsActive) return;

        // Deal 1 damage on contact
        _contactAttackBehaviour.Execute(this, 0, Vector2.Zero);
        player.TakeDamage((int)ContactDamage);
    }
}