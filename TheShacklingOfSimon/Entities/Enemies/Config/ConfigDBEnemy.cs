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
            DropItemType = EnemyDropType.Health
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
            DropItemType = EnemyDropType.Coin
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
            DropItemType = EnemyDropType.Key
        },

        ["AdultLeech"] = new EnemyConfig
        {
            IsBoss = true,
            MoveSpeed = 80f,
            AttackCooldown = 4f,
            AttackRange = 10f,
            ContactDamage = 3f,
            MaxHealth = 10,
            InvulnerabilityDuration = 0.25f,
            DropItemType = EnemyDropType.Key
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
            DropItemType = EnemyDropType.Coin
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
            DropItemType = EnemyDropType.Coin
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
            DropItemType = EnemyDropType.Health
        }
    };
}