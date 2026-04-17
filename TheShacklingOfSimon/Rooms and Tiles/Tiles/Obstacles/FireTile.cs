#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Obstacles
{
    // Walkable hazard that can be extinguished by projectiles
    public sealed class FireTile : Tile, IProjectileAffectableTile
    {
        protected override TileCollisionFlags CollisionFlags => TileCollisionFlags.BlocksProjectiles;

        // Fire frames are ~32x41, so center them in a 64x64 tile
        private static readonly Vector2 DrawOffset =
            new Vector2(
                (RoomConstants.TileSize - 32) / 2f,
                (RoomConstants.TileSize - 41) / 2f
            );

        public FireTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Position + DrawOffset, Color.White);
        }

        public void OnProjectileHit()
        {
            Discontinue();
        }

        public override void OnCollision(IPlayer player)
        {
            player?.TakeDamage(1);
        }

        public override void OnCollision(IEnemy enemy)
        {
            enemy?.TakeDamage(1);
        }

        public override void OnCollision(IProjectile proj)
        {
            if (proj == null || !IsActive) return;
            proj.Discontinue();
            Discontinue();
        }
    }
}