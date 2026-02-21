using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Level_Handler.Tiles
{
    // Solid obstacle that can be destroyed by bombs
    public sealed class RockTile : Tile, IBombableTile
    {
        public override bool BlocksGround => true;
        public override bool BlocksFly => true;
        public override bool BlocksProjectiles => true;

        public RockTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public void OnExplode()
        {
            Discontinue(); // TileMap removes it, becomes floor
        }
    }
}