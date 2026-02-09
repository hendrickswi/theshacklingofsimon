using System.Collections;
using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Controllers;

// Listing IEntity only for readability
public abstract class DamageableEntity : IEntity, IDamageable
{
    // From IEntity
    public Vector2 Position { get; set; }
    
    // From IDamageable
    public float Health { get; set; }
    public float MaxHealth { get; set; }

    public void TakeDamage(float amt)
    {
        Health -= amt;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Heal(float amt)
    {
        if (Health + amt <= MaxHealth)
        {
            Health += amt;
        }
        else
        {
            Health = MaxHealth;
        }
    }

    // Differing sprites per concrete class
    public abstract void Die();
}