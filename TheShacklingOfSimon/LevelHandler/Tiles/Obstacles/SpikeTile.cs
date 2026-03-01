using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Level_Handler.Tiles.Tile_Constructor;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Level_Handler.Tiles.Obstacles
{
    // Walkable hazard; collision system should apply damage on overlap
    public sealed class SpikeTile : Tile, ITriggerTile
    {
        public override bool BlocksGround => false;
        public override bool BlocksFly => false;
        public override bool BlocksProjectiles => false;

        public SpikeTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public void OnIntersect(IEntity entity)
        {
            //wire into damage system later
            // Example later: if (entity is IDamageable d) d.TakeDamage(1);
        }
    }
}