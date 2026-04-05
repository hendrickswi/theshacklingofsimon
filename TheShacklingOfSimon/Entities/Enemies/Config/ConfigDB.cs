#region

using System.Collections.Generic;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.Config;
public static class ConfigDB
{
    public static readonly Dictionary<string, EnemyConfig> Configs = new()
    {
        ["BlackMaw"] = new EnemyConfig
        {
            MoveSpeed = 17f,
            AttackCooldown = 3f,
            AttackRange = 10f,
            ContactDamage = 1f,
            MaxHealth = 3,
            InvulnerabilityDuration = 0.25f
        },

        ["SpiderEnemy"] = new EnemyConfig
        {
            MoveSpeed = 25f,
            AttackCooldown = 1.5f,
            AttackRange = 8f,
            ContactDamage = 1f,
            MaxHealth = 2,
            InvulnerabilityDuration = 0.25f
        }
    };
}