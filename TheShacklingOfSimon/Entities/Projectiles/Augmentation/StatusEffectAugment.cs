using TheShacklingOfSimon.Entities.Projectiles.Decorators;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.Entities.Projectiles.Augmentation;

public class StatusEffectAugment : IProjectileAugment
{
    private readonly IStatusEffect _statusEffectPrototype;
    
    public StatusEffectAugment(IStatusEffect statusEffectPrototype)
    {
        _statusEffectPrototype = statusEffectPrototype;
    }

    public IProjectile ApplyTo(IProjectile projectile)
    {
        return new StatusEffectProjectileDecorator(projectile, _statusEffectPrototype);
    }
}