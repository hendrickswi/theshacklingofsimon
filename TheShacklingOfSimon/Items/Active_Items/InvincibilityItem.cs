#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;

public class InvinciblityItem : ActiveItem, IInventoryItem
{
    private readonly string _sfx;
    private float _timer;
    private readonly float _cooldownDuration;
    private bool _buffActive;

    private readonly float _buffDuration;
    private readonly float _moveSpeedMultiplier;
    private readonly float _fireRateMultiplier;
    private readonly float _projSpeedMultiplier;

    public InvinciblityItem(
        IDamageableEntity entity,
        float buffDuration = 1000f,
        float cooldownDuration = 5.0f)
        : base(entity)
    {
        _cooldownDuration = cooldownDuration;

        Name = "Invincibility";
        Description = "Become Invincible.";
        _sfx = SoundManager.Instance.AddSFX("items","Powerup2");
        
        _buffDuration = buffDuration;
     
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
        var invincibleEffect = new InvulnerabilityDurationEffect(
            Name, 
            Entity,
            10000, 
            _buffDuration
        );
        Entity.EffectManager.AddTemporaryEffect(invincibleEffect);
        SoundManager.Instance.PlaySFX(_sfx);

        return true;
    }
}