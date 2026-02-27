using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;

namespace TheShacklingOfSimon.Entities.Pickup;
public interface IPickup : IEntity
{
    /*
     * Inherits
     * Vector2 Position { get; }
     * Vector2 Velocity { get; }
     * bool IsActive { get; }
     * Rectangle Hitbox { get; }
     * ISprite Sprite { get; }
     *
     * void Update(GameTime delta),
     * void Draw(SpriteBatch spriteBatch),
     * void Discontinue();
     *
     */

    IItem Item { get; set; }
    // if we are to allow weapon pickups, I suggest adding items that give weapons or having iweapon extend item
    
}