using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.LevelHandler.Tiles.Obstacles
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
            Discontinue();
        }

        public override void OnCollision(IPlayer player)
        {
            if (player == null || !IsActive) return;

            Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(player.Hitbox, this.Hitbox);
            if (mtv.LengthSquared() < 0.0001f) return;

            // Handles position, velocity, and hitbox
            player.SetPosition(player.Position + mtv);
        }
        
        public override void OnCollision(IEnemy enemy)
        {
            if (enemy == null || !IsActive) return;

            Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(enemy.Hitbox, this.Hitbox);
            if (mtv.LengthSquared() < 0.0001f) return;
            
            // Handles position, velocity, and hitbox
            enemy.SetPosition(enemy.Position + mtv);
        }
    }
}