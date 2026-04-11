namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomClass
{
    // use this in room JSON/data so base doors can default to combat doors,
    // but still have room to add custom logic later.
    public enum DoorUnlockType
    {
        ClearEnemies,
        AlwaysUnlocked,
        Custom
    }
}