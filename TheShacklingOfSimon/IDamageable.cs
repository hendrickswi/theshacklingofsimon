namespace TheShacklingOfSimon;

public interface IDamageable : IEntity
{
    float Health { get; set; }
    float MaxHealth { get; set; }

    void TakeDamage(float amt);
    void Heal(float amt);
    void Die();
}