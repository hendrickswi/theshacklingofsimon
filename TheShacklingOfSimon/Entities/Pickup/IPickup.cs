using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;

namespace TheShacklingOfSimon.Entities.Pickup;
public interface IPickup : IEntity
{
    IItem Item { get; set; }
}