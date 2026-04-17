#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items;

public class SpeedItem : PassiveItem, IInventoryItem
{
    private readonly float _amt;
    private readonly float _duration;
    
    public SpeedItem(
        IDamageableEntity entity, 
        string name = "Running Boots", 
        string description = "Increases speed by 25%", 
        float amt = 0.25f,
        float duration = float.MaxValue) 
        : base(entity)
    {
        Name = name;
        Description = description;
        _amt = amt;
        _duration = duration;
    }
    
    public override bool ApplyEffect()
    {
        IStatusEffect effect = new MoveSpeedEffect(
            Name, 
            EffectType.MoveSpeed, 
            Entity, 
            Entity.GetStat(StatType.MoveSpeed) * _amt, 
            _duration
        );
        Entity.EffectManager.AddEffect(effect);
        return true;
    }
}