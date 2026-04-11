#region

using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles.Border.Doors
{
    // keep unlock rules in their own strategy objects so DoorTile does not
    // need if/else chains for every special case later.
    public interface IDoorUnlockCondition
    {
        bool IsSatisfied(IDoorContext roomContext);
    }

    public sealed class ClearEnemiesDoorCondition : IDoorUnlockCondition
    {
        public bool IsSatisfied(IDoorContext roomContext)
        {
            return roomContext != null && !roomContext.HasActiveEnemies();
        }
    }

    public sealed class AlwaysUnlockedDoorCondition : IDoorUnlockCondition
    {
        public bool IsSatisfied(IDoorContext roomContext)
        {
            return true;
        }
    }

    // Placeholder for future custom conditions.
    // For now this stays locked until I plug in extra logic
    public sealed class CustomDoorCondition : IDoorUnlockCondition
    {
        public bool IsSatisfied(IDoorContext roomContext)
        {
            return false;
        }
    }
}