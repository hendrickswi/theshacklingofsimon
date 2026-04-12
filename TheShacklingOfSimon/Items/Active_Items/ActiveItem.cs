#region

using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;
public class ActiveItem : IItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string SFX { get; set; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; }

    
    public ActiveItem(IPlayer player)
    {
        Player = player;
    }
    
    public void Effect()
    {
        // TODO: use the StatusEffectManager instead of directly manipulating player stats
        Player.SetStat(StatType.DamageMultiplier, Player.GetStat(StatType.DamageMultiplier) + Effects.Attack);
        Player.Heal(Effects.Health);
        Player.MaxHealth += Effects.MaxHealth;
        Player.SetStat(StatType.MoveSpeed, Player.GetStat(StatType.MoveSpeed) + Effects.Speed);
        SoundManager.Instance.PlaySFX(SFX);
        if (Effects.OneTime)
        {
            // I doubt we need to worry about this rn
        }
    }
    public void ClearEffect()
    {
        // TODO: use the StatusEffectManager instead of directly manipulating player stats
        Player.SetStat(StatType.DamageMultiplier, Player.GetStat(StatType.DamageMultiplier - Effects.Attack));
        Player.MaxHealth -= Effects.MaxHealth;
        Player.SetStat(StatType.MoveSpeed, Player.GetStat(StatType.MoveSpeed) - Effects.Speed);
    }
}