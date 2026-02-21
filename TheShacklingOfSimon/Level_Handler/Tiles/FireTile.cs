using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Level_Handler.Tiles
{
    // Walkable hazard that can be extinguished by projectiles
    public sealed class FireTile : Tile, IProjectileAffectableTile
    {
        public override bool BlocksGround => false;
        public override bool BlocksFly => false;
        public override bool BlocksProjectiles => false;

        public FireTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public void OnProjectileHit()
        {
            Discontinue(); // TileMap removes it becomes floor
        }
    }
}