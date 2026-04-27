#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace TheShacklingOfSimon.Entities.Players.Config;

public static class ConfigDBPlayer
{
    public static readonly Dictionary<string, PlayerConfig> Configs = new()
    {
        ["PlayerWithTwoSprites"] = new PlayerConfig
        {
            MaxHealth = 10,
            HitboxWidth = 20,
            HitboxHeight = 20,
            InvulnerabilityDuration = 0.5f,
            MoveSpeed = 100f,
            MoveSpeedMultiplier = 1f,
            DamageMultiplier = 1,
            ProjectileSpeedMultiplier = 1f,
            PrimaryCooldown = 0.0f,
            SecondaryCooldown = 0f,
            MovementFrameDuration = 0.05f,
            DeathFrameDuration = 1f,
            HurtFrameDuration = 0.1f,
            HeadOffset = new Vector2(-4.75f, -16),
            DamagedStateOffset = new Vector2(0, -5),
            SkinsDictionary = new Dictionary<string, string>
            {
                {"Head", "PlayerHead"},
                {"Body", "PlayerBody"},
            }
        }
    };
}