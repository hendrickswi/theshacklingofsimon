#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles.Obstacles
{
    public sealed class HoleTile : Tile
    {
        private Texture2D debugTexture;

        protected override TileCollisionFlags CollisionFlags => TileCollisionFlags.BlocksGround;

        public HoleTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public override void Update(GameTime delta)
        {
            // No sprite to update for the temporary hole tile
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (debugTexture == null)
            {
                debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                debugTexture.SetData(new[] { Color.White });
            }

            spriteBatch.Draw(debugTexture, Hitbox, Color.Black);
        }

        public override void OnCollision(IPlayer player)
        {
            if (player == null || !IsActive) return;
            ResolveEntityCollision(player);
        }

        public override void OnCollision(IEnemy enemy)
        {
            if (enemy == null || !IsActive) return;
            ResolveEntityCollision(enemy);
        }
    }
}