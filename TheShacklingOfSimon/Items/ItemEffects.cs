namespace TheShacklingOfSimon.Items;

public record struct ItemEffects
{
    public int Attack;
    public int Health;
    public int MaxHealth;
    public int Speed;
    public bool OneTime;

    public ItemEffects(int attack, int health, int maxHealth, int speed, bool consumable)
    {
        Attack = attack;
        Health = health;
        MaxHealth = maxHealth;
        Speed = speed;
        OneTime = consumable;
    }
}