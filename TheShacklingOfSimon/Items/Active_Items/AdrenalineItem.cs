#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;

public class AdrenalineItem : IItem
{
    public string Name { get; }
    public string Description { get; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; } // unused
    public string SFX { get; }

    // Tunables
    private readonly float _durationSeconds;     // how long buff lasts
    private readonly float _cooldownSeconds;     // cooldown AFTER buff ends (match teleport)
    private readonly float _moveSpeedMultiplier; // noticeable movement boost
    private readonly float _fireRateMultiplier;  // < 1 => faster firing (lower cooldowns)
    private readonly float _projSpeedMultiplier; // faster projectiles

    // Buffs
    private readonly IStatusEffect _speedBuff;
    private readonly IStatusEffect _primaryCooldownBuff;
    private readonly IStatusEffect _secondaryCooldownBuff;
    private readonly IStatusEffect _projectileSpeedBuff;
    
    // Timers/state
    private float _cooldownTimer;
    private float _buffTimer;
    private bool _buffActive;

    public AdrenalineItem(
        IPlayer player,
        float durationSeconds = 4.0f,
        float cooldownSeconds = 5.0f,     // match teleport
        float moveSpeedMultiplier = 2f,
        float fireRateMultiplier = 0.50f, // half cooldown => 2x fire rate
        float projSpeedMultiplier = 2f)
    {
        Player = player;

        _durationSeconds = durationSeconds;
        _cooldownSeconds = cooldownSeconds;
        _moveSpeedMultiplier = moveSpeedMultiplier;
        _fireRateMultiplier = fireRateMultiplier;
        _projSpeedMultiplier = projSpeedMultiplier;
        
        _speedBuff = new MoveSpeedEffect("Speed up", EffectType.MoveSpeed, Player, _moveSpeedMultiplier, _durationSeconds);
        _primaryCooldownBuff = new PrimaryCooldownEffect("Primary attack speed up", EffectType.PrimaryCooldown, Player, _fireRateMultiplier, _durationSeconds);
        _secondaryCooldownBuff = new SecondaryCooldownEffect("Secondary attack speed up", EffectType.SecondaryCooldown, Player, _fireRateMultiplier, _durationSeconds);
        _projectileSpeedBuff = new ProjectileSpeedEffect("Projectile speed up", EffectType.ProjectileSpeedMultiplier, Player, _projSpeedMultiplier, _durationSeconds);

        Name = "Adrenaline";
        Description = "Massive speed, fire-rate, and projectile speed boost for a short time.";
        Effects = new ItemEffects(0, 0, 0, 0, false);
        SFX = SoundManager.Instance.NameSFX("items","Powerup2");
        SoundManager.Instance.AddSFX(SFX);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Buff ticking
        if (_buffActive)
        {
            _buffTimer -= dt;
            if (_buffTimer <= 0f)
            {
                // Cooldown starts after the effect finishes
                _cooldownTimer = _cooldownSeconds;
            }
            return; // do not tick cooldown while buff is active
        }

        // Cooldown ticking (only when not buffing)
        if (_cooldownTimer > 0f)
            _cooldownTimer -= dt;
    }

    public void Effect()
    {
        // Can't activate during cooldown
        if (_cooldownTimer > 0f) return;

        // If already active, refresh duration (optional but possibly useful for another item)
        if (_buffActive)
        {
            _buffTimer = _durationSeconds;
            return;
        }

        StartBuff();
        SoundManager.Instance.PlaySFX(SFX);
        _buffTimer = _durationSeconds;
    }

    private void StartBuff()
    {
        _buffActive = true;
        
        Player.EffectManager.AddEffect(_speedBuff);
        Player.EffectManager.AddEffect(_primaryCooldownBuff);
        Player.EffectManager.AddEffect(_secondaryCooldownBuff);
        Player.EffectManager.AddEffect(_projectileSpeedBuff);
    }
    
    // No need for a RemoveBuff() method; the StatusEffectManager already handles effect removal
}