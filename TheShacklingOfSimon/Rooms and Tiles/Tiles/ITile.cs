#region

using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles
{
    // Tile-specific collision flags on top of standard IEntity stuff
    public interface ITile : IEntity
    {
        // still expose these as properties because the rest of the project likely uses them directly.
        bool BlocksGround { get; }

        bool BlocksFly { get; }

        bool BlocksProjectiles { get; }
    }
}