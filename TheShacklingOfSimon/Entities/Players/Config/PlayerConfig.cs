#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

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
    
    // Sprite drawing data
    public Vector2 HeadOffset { get; set; }
    public Vector2 DamagedStateOffset { get; set; }
    public Dictionary<string, string> SkinsDictionary { get; set; }
}