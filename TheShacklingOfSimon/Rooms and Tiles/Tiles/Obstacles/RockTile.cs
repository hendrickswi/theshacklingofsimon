#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Obstacles
{
    // Solid obstacle that can be destroyed by bombs
    public sealed class RockTile : Tile, IBombableTile
    {
        protected override TileCollisionFlags CollisionFlags =>
            TileCollisionFlags.BlocksGround |
            TileCollisionFlags.BlocksFly |
            TileCollisionFlags.BlocksProjectiles;

        public RockTile(ISprite sprite, Vector2 position) : base(sprite, position)
        {
            SFX = SoundManager.Instance.AddSFX("other", "Rock_crumble 0");
        }

        public void OnExplode()
        {
            SoundManager.Instance.PlaySFX(SFX);
            Discontinue();
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

        public override void OnCollision(IProjectile proj)
        {
            if (proj == null || !IsActive) return;
            proj.Discontinue();
        }
    }
}