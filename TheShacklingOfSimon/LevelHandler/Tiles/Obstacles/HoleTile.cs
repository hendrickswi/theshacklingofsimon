using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Level_Handler.Tiles.Obstacles
{
    // Pit tile: blocks ground entities, flying entities can pass over it
    public sealed class HoleTile : Tile
    {
        public override bool BlocksGround => true;
        public override bool BlocksFly => false;
        public override bool BlocksProjectiles => false;

        public HoleTile(ISprite sprite, Vector2 position) : base(sprite, position) { }
    }
}