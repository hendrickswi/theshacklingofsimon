using TheShacklingOfSimon.StatusEffects;

namespace TheShacklingOfSimon.Entities;

public interface IDamageableEntity : IEntity
{
    int Health { get; }
    int MaxHealth { get; set; }
    StatusEffectManager EffectManager { get; }

    /// <summary>
    /// Applies damage to <c>this</c>, reducing its health by the specified amount. If the entity is invulnerable, no damage is applied.
    /// </summary>
    /// <param name="amt">The amount of damage to apply to the entity's health.</param>
    void TakeDamage(int amt);

    /// <summary>
    /// Heals <c>this</c> by the specified amount.
    /// </summary>
    /// <param name="amt">The amount to increase the health property of <c>this</c> by</param>
    void Heal(int amt);

    /// <summary>
    /// Retrieves the value of a specified stat for the damageable entity. If the stat is not defined, returns 0.
    /// </summary>
    /// <param name="stat">The enumerated stat type to retrieve</param>
    /// <returns>The value of the specified stat if it exists, otherwise 0.</returns>
    float GetStat(StatType stat);

    /// <summary>
    /// Sets the value of an enumerated stat of <c>this</c>.
    /// </summary>
    /// <param name="stat">The enumerated stat type to modify.</param>
    /// <param name="value">The value to assign to the specified stat.</param>
    void SetStat(StatType stat, float value);
}