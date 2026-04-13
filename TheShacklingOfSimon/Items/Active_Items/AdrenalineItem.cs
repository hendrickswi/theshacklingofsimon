#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;

public class AdrenalineItem : ActiveItem
{
    // sound effects should be internal to the class
    private string sfx;
    private float _timer;
    private bool _buffActive;
    
    private readonly float _cooldownDuration;

    private readonly IStatusEffect _speedEffect;
    private readonly IStatusEffect _primaryCooldownEffect;
    private readonly IStatusEffect _secondaryCooldownEffect;
    private readonly IStatusEffect _projectileSpeedEffect;

    public AdrenalineItem(
        IDamageableEntity entity,
        float duration = 4.0f,
        float cooldownDuration = 5.0f,
        float moveSpeedMultiplier = 2f,
        float fireRateMultiplier = 0.50f,
        float projSpeedMultiplier = 2f)
        : base(entity)
    {
        _cooldownDuration = cooldownDuration;

        Name = "Adrenaline";
        Description = "Massive speed, fire-rate, and projectile speed boost for a short time.";
        sfx = SoundManager.Instance.NameSFX("items","Powerup2");
        SoundManager.Instance.AddSFX(sfx);
        
        // Status effect initialization
        _speedEffect = new MoveSpeedEffect(
            Name, 
            EffectType.MoveSpeed, 
            entity,
            moveSpeedMultiplier * entity.GetStat(StatType.MoveSpeed), 
            duration
        );
        _primaryCooldownEffect = new PrimaryCooldownEffect(
            Name, 
            EffectType.PrimaryCooldown, 
            entity,
            fireRateMultiplier * entity.GetStat(StatType.PrimaryCooldown), 
            duration
        );
        _secondaryCooldownEffect = new SecondaryCooldownEffect(
            Name, 
            EffectType.SecondaryCooldown, 
            entity,
            fireRateMultiplier * entity.GetStat(StatType.SecondaryCooldown), 
            duration
        );
        _projectileSpeedEffect = new ProjectileSpeedEffect(
            Name, 
            EffectType.ProjectileSpeedMultiplier, 
            entity,
            projSpeedMultiplier * entity.GetStat(StatType.ProjectileSpeedMultiplier), 
            duration
        );
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

    public override void ApplyEffect()
    {
        if (_buffActive) return;
        
        _buffActive = true;
        _timer = _cooldownDuration;
        Entity.EffectManager.AddEffect(_speedEffect);
        Entity.EffectManager.AddEffect(_primaryCooldownEffect);
        Entity.EffectManager.AddEffect(_secondaryCooldownEffect);
        Entity.EffectManager.AddEffect(_projectileSpeedEffect);
        SoundManager.Instance.PlaySFX(sfx);
    }

    public override void ClearEffect()
    {
    }
}