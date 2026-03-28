using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.LevelHandler.Tiles.Obstacles
{
    public sealed class SpikeTile : Tile
    {
        public override bool BlocksGround => false;
        public override bool BlocksFly => false;
        public override bool BlocksProjectiles => false;

        public SpikeTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public override void OnCollision(IPlayer player)
        {
            player.TakeDamage(1);
        }

        public override void OnCollision(IEnemy enemy)
        {
            enemy.TakeDamage(1);
        }
    }
}