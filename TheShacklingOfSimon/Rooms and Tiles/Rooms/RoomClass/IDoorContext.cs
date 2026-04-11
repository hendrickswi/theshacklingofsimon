namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomClass
{
    // This keeps door logic from depending on the full Room class implementation.
    public interface IDoorContext
    {
        bool HasActiveEnemies();
    }
}