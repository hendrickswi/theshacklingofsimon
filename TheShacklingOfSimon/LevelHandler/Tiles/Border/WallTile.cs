using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.LevelHandler.Tiles.Border
{
    /// Invisible, indestructible wall used for borders. Blocks everything.
    public sealed class WallTile : Tile
    {
        public override bool BlocksGround => true;
        public override bool BlocksFly => true;
        public override bool BlocksProjectiles => true;

        public WallTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        // Invisible, don't draw anything
        public override void Draw(SpriteBatch spriteBatch) { }

        // No animation needed, avoid updating sprite work
        public override void Update(GameTime delta) { }
        public override void OnCollision(IPlayer player)
        {
            if (player == null || !IsActive) return;

            Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(player.Hitbox, this.Hitbox);
            if (mtv == Vector2.Zero) return;

            player.SetPosition(player.Position + mtv);
        }
    }
}