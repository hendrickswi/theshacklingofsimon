using System.Collections;
using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities;

public abstract class DamageableEntity : IDamageable
{
    // Properties from IEntity
    public Vector2 Position { get; set; }
    
    // Properties from IDamageable
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    // Methods from IDamageable
    public void TakeDamage(int amt)
    {
        Health -= amt;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amt)
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
    
    // Methods from IEntity will go here
}