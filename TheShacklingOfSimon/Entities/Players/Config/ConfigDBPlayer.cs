using System.Collections.Generic;

namespace TheShacklingOfSimon.Entities.Players.Config;

public static class ConfigDBPlayer
{
    public static readonly Dictionary<string, PlayerConfig> Configs = new()
    {
        ["PlayerWithTwoSprites"] = new PlayerConfig
        {
            MaxHealth = 6,
            HitboxWidth = 20,
            HitboxHeight = 20,
            InvulnerabilityDuration = 0.333334f,
            MoveSpeed = 100f,
            DamageMultiplier = 1,
            ProjectileSpeedMultiplier = 1f,
            PrimaryCooldown = 0.5f,
            SecondaryCooldown = 1.25f,
            MovementFrameDuration = 0.05f,
            DeathFrameDuration = 1f,
            HurtFrameDuration = 0.1f,
        }
    };
}