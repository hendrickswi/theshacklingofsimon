#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles.Border
{
    /// Invisible, indestructible wall used for borders. Blocks everything.
    public sealed class WallTile : Tile
    {
        protected override TileCollisionFlags CollisionFlags =>
            TileCollisionFlags.BlocksGround |
            TileCollisionFlags.BlocksFly |
            TileCollisionFlags.BlocksProjectiles;

        public WallTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        // Invisible, don't draw anything
        public override void Draw(SpriteBatch spriteBatch) { }

        // No animation needed, avoid updating sprite work
        public override void Update(GameTime delta) { }

        public override void OnCollision(IPlayer player)
        {
            if (player == null || !IsActive) return;

            ResolveEntityCollision(player);
        }
    }
}