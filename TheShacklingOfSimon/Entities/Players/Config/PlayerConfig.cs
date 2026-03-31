namespace TheShacklingOfSimon.Entities.Players.Config;

public class PlayerConfig
{
    public int MaxHealth { get; set; }
    public int HitboxWidth { get; set; }
    public int HitboxHeight { get; set; }
    public float InvulnerabilityDuration { get; set; }
    public float MoveSpeed { get; set; }
    public int DamageMultiplier { get; set; }
    public float ProjectileSpeedMultiplier { get; set; }
    public float PrimaryCooldown { get; set; }
    public float SecondaryCooldown { get; set; }
    
    // Animation data
    public float MovementFrameDuration { get; set; }
    public float DeathFrameDuration { get; set; }
    public float HurtFrameDuration { get; set; }
}