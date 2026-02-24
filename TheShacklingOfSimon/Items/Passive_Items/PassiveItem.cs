using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items.Passive_Items;
public class PassiveItem : IItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; }

    
    public PassiveItem(IPlayer player)
    {
        Player = player;
    }
    
    public void Effect()
    {
        Player.DamageMultiplierStat += Effects.Attack;
        Player.MaxHealth += Effects.MaxHealth;
        Player.MoveSpeedStat += Effects.Speed;
    }
    public void ClearEffect()
    {
        Player.DamageMultiplierStat -= Effects.Attack;
        Player.MaxHealth -= Effects.MaxHealth;
        Player.MoveSpeedStat -= Effects.Speed;
    }
}