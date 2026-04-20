using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Entities.Projectiles.Augmentation;
using TheShacklingOfSimon.Entities.Projectiles.Decorators;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.Items.Passive_Items.Tear_Augmentation;

public class ProjectileAugmentItem : PassiveItem
{
    private readonly IProjectileAugment _augmentationEffect;
    
    public ProjectileAugmentItem(
        IDamageableEntity entity,
        string name,
        string description,
        IProjectileAugment augmentationEffect)
        : base(entity)
    {
        Entity = entity;
        Name = name;
        Description = description;
        _augmentationEffect = augmentationEffect;
    }

    public override bool ApplyEffect()
    {
        if (Entity is not IPlayer player) return false;

        player.Inventory.CurrentPrimaryWeapon?.AddAugment(_augmentationEffect);
        player.Inventory.CurrentSecondaryWeapon?.AddAugment(_augmentationEffect);
        
        return true;
    }

    public override void ClearEffect()
    {
        if (Entity is not IPlayer player) return;

        player.Inventory.CurrentPrimaryWeapon?.RemoveAugment(_augmentationEffect);
        player.Inventory.CurrentSecondaryWeapon?.RemoveAugment(_augmentationEffect);
    }
}