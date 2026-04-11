namespace TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor
{
    // Tile can be affected by an explosion (bomb)
    public interface IBombableTile
    {
        void OnExplode();
    }

    // Tile can be affected by a projectile hit/overlap
    public interface IProjectileAffectableTile
    {
        void OnProjectileHit();
    }
}