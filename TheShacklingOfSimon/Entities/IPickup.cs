using System.ComponentModel;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities;
public interface IPickup : IEntity
{
    IItem Item { get; set; }
    // if we are to allow weapon pickups, I suggest adding items that give weapons or having iweapon extend item
}