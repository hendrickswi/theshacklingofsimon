#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.States;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies;

public interface IEnemy : IDamageableEntity
{
    string Name { get; }

    // For the manager
    bool MarkedForRemoval { get; }

    // IEnemy-implementing classes will act as the context for the State pattern
    IEnemyState CurrentState { get; }
    
    float MoveSpeedStat { get; set; }
    float AttackCooldown { get; set; }
    float AttackRange { get; set; }
    float ContactDamage { get; set; }
    IItem EnemyDrop { get; set; } // we can set this to null/noneitem if we don't want an enemy dropping something

    IWeapon Weapon { get; }
    public event Action<IProjectile> OnProjectileCreated;

    void SetWeapon(IWeapon weapon);
    void MarkForRemoval();
    Vector2 FindTarget();
    void RegisterMovement(float dt, Vector2 targetPosition);
    void RegisterAttack(float dt, Vector2 targetDirection);
    void ChangeState(IEnemyState newState);
}