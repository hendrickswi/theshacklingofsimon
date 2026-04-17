#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;

public class AdrenalineItem : ActiveItem, IInventoryItem
{
    // sound effects should be internal to the class
    private string sfx;
    private float _timer;
    private readonly float _cooldownDuration;
    private bool _buffActive;

    private float _buffDuration;
    private float _moveSpeedMultiplier;
    private float _fireRateMultiplier;
    private float _projSpeedMultiplier;

    public AdrenalineItem(
        IDamageableEntity entity,
        float buffDuration = 4.0f,
        float cooldownDuration = 5.0f,
        float moveSpeedMultiplier = 2f,
        float fireRateMultiplier = 0.50f,
        float projSpeedMultiplier = 2f)
        : base(entity)
    {
        _cooldownDuration = cooldownDuration;

        Name = "Adrenaline";
        Description = "Massive speed, fire-rate, and projectile speed boost for a short time.";
        sfx = SoundManager.Instance.AddSFX("items","Powerup2");
        
        _buffDuration = buffDuration;
        _moveSpeedMultiplier = moveSpeedMultiplier;
        _fireRateMultiplier = fireRateMultiplier;
        _projSpeedMultiplier = projSpeedMultiplier;
    }

    public override void Update(GameTime gameTime)
    {
        if (!_buffActive) return;
        _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer <= 0f)
        {
            _buffActive = false;
        }
    }

    public override bool ApplyEffect()
    {
        if (_buffActive) return false;
        _buffActive = true;
        _timer = _cooldownDuration;
        
        // Create effects here to avoid issues with the effects being applied to the wrong entity
        var speedEffect = new MoveSpeedEffect(
            Name, 
            EffectType.MoveSpeed, 
            Entity,
            _moveSpeedMultiplier * Entity.GetStat(StatType.MoveSpeed), 
            _buffDuration
        );
        var primaryCooldownEffect = new PrimaryCooldownEffect(
            Name, 
            EffectType.PrimaryCooldown, 
            Entity,
            _fireRateMultiplier * Entity.GetStat(StatType.PrimaryCooldown), 
            _buffDuration
        );
        var secondaryCooldownEffect = new SecondaryCooldownEffect(
            Name, 
            EffectType.SecondaryCooldown, 
            Entity,
            _fireRateMultiplier * Entity.GetStat(StatType.SecondaryCooldown), 
            _buffDuration
        );
        var projectileSpeedEffect = new ProjectileSpeedEffect(
            Name, 
            EffectType.ProjectileSpeedMultiplier, 
            Entity,
            _projSpeedMultiplier * Entity.GetStat(StatType.ProjectileSpeedMultiplier), 
            _buffDuration
        );
        
        Entity.EffectManager.AddEffect(speedEffect);
        Entity.EffectManager.AddEffect(primaryCooldownEffect);
        Entity.EffectManager.AddEffect(secondaryCooldownEffect);
        Entity.EffectManager.AddEffect(projectileSpeedEffect);
        SoundManager.Instance.PlaySFX(sfx);

        return true;
    }

    public override void ClearEffect()
    {
    }
}