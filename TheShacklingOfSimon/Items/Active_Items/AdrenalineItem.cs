using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items.Active_Items;

public class AdrenalineItem : IItem
{
    public string Name { get; }
    public string Description { get; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; } // unused

    // Tunables
    private readonly float _durationSeconds;     // how long buff lasts
    private readonly float _cooldownSeconds;     // cooldown AFTER buff ends (match teleport)
    private readonly float _moveSpeedMultiplier; // noticeable movement boost
    private readonly float _fireRateMultiplier;  // < 1 => faster firing (lower cooldowns)
    private readonly float _projSpeedMultiplier; // faster projectiles

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

        Name = "Adrenaline";
        Description = "Massive speed, fire-rate, and projectile speed boost for a short time.";
        Effects = new ItemEffects(0, 0, 0, 0, false);
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
                EndBuff();

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
        _buffTimer = _durationSeconds;
    }

    private void StartBuff()
    {
        _buffActive = true;

        Player.MoveSpeedStat *= _moveSpeedMultiplier;

        // Faster firing by reducing cooldown numbers
        Player.PrimaryAttackCooldown *= _fireRateMultiplier;
        Player.SecondaryAttackCooldown *= _fireRateMultiplier;

        // Faster projectiles
        Player.ProjectileSpeedMultiplierStat *= _projSpeedMultiplier;

        // Safety clamp so you can't hit 0 cooldown by stacking bugs
        if (Player.PrimaryAttackCooldown < 0.05f) Player.PrimaryAttackCooldown = 0.05f;
        if (Player.SecondaryAttackCooldown < 0.05f) Player.SecondaryAttackCooldown = 0.05f;
    }

    private void EndBuff()
    {
        _buffActive = false;

        Player.MoveSpeedStat /= _moveSpeedMultiplier;

        Player.PrimaryAttackCooldown /= _fireRateMultiplier;
        Player.SecondaryAttackCooldown /= _fireRateMultiplier;

        Player.ProjectileSpeedMultiplierStat /= _projSpeedMultiplier;
    }
}