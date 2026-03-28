using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.LevelHandler.Tiles.Obstacles
{
    // Pit tile: blocks ground entities, flying entities can pass over it
    public sealed class HoleTile : Tile
    {
        public override bool BlocksGround => true;
        public override bool BlocksFly => false;
        public override bool BlocksProjectiles => false;

        public HoleTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

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