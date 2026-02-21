using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Items.Active_Items;
public class ActiveItem : IItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; }

    
    public ActiveItem(IPlayer player)
    {
        Player = player;
    }
    
    public void Effect()
    {
        Player.DamageMultiplierStat += Effects.Attack;
        Player.Heal(Effects.Health);
        Player.MaxHealth += Effects.MaxHealth;
        Player.MoveSpeedStat += Effects.Speed;
        if (Effects.OneTime)
        {
            // I doubt we need to worry about this rn
        }
    }
    public void ClearEffect()
    {
        Player.DamageMultiplierStat -= Effects.Attack;
        Player.MaxHealth -= Effects.MaxHealth;
        Player.MoveSpeedStat -= Effects.Speed;
    }
}