using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Level_Handler.Tiles.Border
{
    public sealed class DoorTile : Tile
    {
        public override bool BlocksGround => false;
        public override bool BlocksFly => false;
        public override bool BlocksProjectiles => false;

        public string ToRoom { get; }
        public Point SpawnInterior { get; }   // interior coords (0..12,0..6)
        public DoorSide Side { get; }         // North/South/East/West

        public DoorTile(
            ISprite sprite,
            Vector2 position,
            string toRoom,
            Point spawnInterior,
            DoorSide side)
            : base(sprite, position)
        {
            ToRoom = toRoom ?? "";
            SpawnInterior = spawnInterior;
            Side = side;
        }
    }
}