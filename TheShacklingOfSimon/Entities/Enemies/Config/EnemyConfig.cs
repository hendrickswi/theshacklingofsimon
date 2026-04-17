#region

using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.Config;
public enum EnemyDropType
{
    None,
    Health,
    Speed,
    Coin,
    Key
}

public class EnemyConfig
{
    public bool IsBoss { get; set; }
    public float MoveSpeed { get; set; }
    public float AttackCooldown { get; set; }
    public float AttackRange { get; set; }
    public float ContactDamage { get; set; }
    public int MaxHealth { get; set; }
    public IWeapon Weapon { get; set; }
    
    public float InvulnerabilityDuration { get; set; }
    public EnemyDropType DropItemType { get; set; } = EnemyDropType.None;
}