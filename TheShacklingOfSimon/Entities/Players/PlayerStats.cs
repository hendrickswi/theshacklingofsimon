namespace TheShacklingOfSimon.Entities.Players;

public class PlayerStats
{
    public int DamageMultiplierStat { get; set; }
    public float MoveSpeedStat { get; set; }
    public float ProjectileSpeedMultiplierStat { get; set; }
    public float PrimaryAttackCooldown { get; set; }
    public float SecondaryAttackCooldown { get; set; }
    public float MovementFrameDuration { get; set; }
    public float DeathFrameDuration { get; set; }
    public float InvulnerabilityDuration { get; set; }

    public PlayerStats()
    {
        // Initialize default values
        DamageMultiplierStat = 1;
        MoveSpeedStat = 100.0f;
        PrimaryAttackCooldown = 0.5f;
        ProjectileSpeedMultiplierStat = 1.0f;
        SecondaryAttackCooldown = 1.5f;
        MovementFrameDuration = 0.05f;
        DeathFrameDuration = 1.0f;
        InvulnerabilityDuration = 0.333334f;
    }
    
    // TODO: Add buff/debuff methods here
    // TODO: (and) remove items manually modifying the player stats
}