using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.Level_Handler.Tiles
{
    // Tile-specific collision flags on top of standard IEntity stuff
    public interface ITile : IEntity
    {
        // Blocks entities that move on the ground
        bool BlocksGround { get; }

        // Blocks entities that can fly
        bool BlocksFly { get; }

        // Blocks projectiles (arrows, tears, etc.)
        bool BlocksProjectiles { get; }
    }
}