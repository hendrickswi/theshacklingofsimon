#region

using System.Collections.Generic;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.Config;
public static class ConfigDBEnemy
{
    public static readonly Dictionary<string, EnemyConfig> Configs = new()
    {
        ["BlackMaw"] = new EnemyConfig
        {
            IsBoss = false,
            MoveSpeed = 90f,
            AttackCooldown = 3f,
            AttackRange = 10f,
            ContactDamage = 1f,
            MaxHealth = 3,
            InvulnerabilityDuration = 0.25f,
            DropItemPool = new[] { EnemyDropType.Health, EnemyDropType.Coin }
        },

        ["SpiderEnemy"] = new EnemyConfig
        {
            IsBoss = false,
            MoveSpeed = 60f,
            AttackCooldown = 3f,
            AttackRange = 8f,
            ContactDamage = 1f,
            MaxHealth = 2,
            InvulnerabilityDuration = 0.25f,
            DropItemPool = new[] { EnemyDropType.Coin, EnemyDropType.Health }
        },

        ["AngelicBaby"] = new EnemyConfig
        {
            IsBoss = false,
            MoveSpeed = 100f,
            AttackCooldown = 6f,
            AttackRange = 8f,
            ContactDamage = 1f,
            MaxHealth = 2,
            InvulnerabilityDuration = 0.25f,
            DropItemPool = new[] { EnemyDropType.Key, EnemyDropType.Coin }
        },

        ["AdultLeech"] = new EnemyConfig
        {
            IsBoss = true,
            MoveSpeed = 120f,
            AttackCooldown = 4f,
            AttackRange = 10f,
            ContactDamage = 3f,
            MaxHealth = 10,
            InvulnerabilityDuration = 0.25f,
            DropItemPool = new[] { EnemyDropType.Key, EnemyDropType.Health, EnemyDropType.Coin }
        },

        ["Cohort"] = new EnemyConfig
        {
            IsBoss = false,
            MoveSpeed = 8f,
            AttackCooldown = 1f,
            AttackRange = 10f,
            ContactDamage = 2f,
            MaxHealth = 5,
            InvulnerabilityDuration = 0.25f,
            DropItemPool = new[] { EnemyDropType.Coin, EnemyDropType.Speed }
        },

        ["BlindCreep"] = new EnemyConfig
        {
            IsBoss = false,
            MoveSpeed = 100f,
            AttackCooldown = 6f,
            AttackRange = 8f,
            ContactDamage = 1f,
            MaxHealth = 4,
            InvulnerabilityDuration = 0.25f,
            DropItemPool = new[] { EnemyDropType.Coin, EnemyDropType.Health }
        },
        
        ["Clotty"] = new EnemyConfig
        {
            IsBoss = false,
            MoveSpeed = 80f,
            AttackCooldown = 4f,
            AttackRange = 10f,
            ContactDamage = 1f,
            MaxHealth = 3,
            InvulnerabilityDuration = 0.25f,
            DropItemPool = new[] { EnemyDropType.Health, EnemyDropType.Coin }
        }
    };
}