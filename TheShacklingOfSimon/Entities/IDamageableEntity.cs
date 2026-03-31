using TheShacklingOfSimon.StatusEffects;

namespace TheShacklingOfSimon.Entities;

public interface IDamageableEntity : IEntity
{
    int Health { get; }
    int MaxHealth { get; set; }
    StatusEffectManager EffectManager { get; }
    
    void TakeDamage(int amt);
    void Heal(int amt);

    float GetStat(StatType stat);
    void SetStat(StatType stat, float value);
}