#region

using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.Config;
public class EnemyConfig
{
    public float MoveSpeed { get; set; }
    public float AttackCooldown { get; set; }
    public float AttackRange { get; set; }
    public float ContactDamage { get; set; }
    public int MaxHealth { get; set; }
    public IWeapon Weapon { get; set; }
    
    public float InvulnerabilityDuration { get; set; }
}