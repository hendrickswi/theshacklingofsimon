#region

using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Border.Doors
{
    public interface IDoorUnlockCondition
    {
        bool IsSatisfied(IDoorContext roomContext);
        bool TryUnlock(IPlayer player, IDoorContext roomContext);
    }

    public sealed class ClearEnemiesDoorCondition : IDoorUnlockCondition
    {
        public bool IsSatisfied(IDoorContext roomContext)
        {
            return roomContext != null && !roomContext.HasActiveEnemies();
        }

        public bool TryUnlock(IPlayer player, IDoorContext roomContext)
        {
            return IsSatisfied(roomContext);
        }
    }

    public sealed class AlwaysUnlockedDoorCondition : IDoorUnlockCondition
    {
        public bool IsSatisfied(IDoorContext roomContext)
        {
            return true;
        }

        public bool TryUnlock(IPlayer player, IDoorContext roomContext)
        {
            return true;
        }
    }

    public sealed class KeyRequiredDoorCondition : IDoorUnlockCondition
    {
        private bool _isUnlocked;

        public bool IsSatisfied(IDoorContext roomContext)
        {
            return _isUnlocked;
        }

        public bool TryUnlock(IPlayer player, IDoorContext roomContext)
        {
            if (_isUnlocked) return true;
            if (player == null) return false;
            if (player.Inventory == null) return false;
            if (player.Inventory.NumKeys <= 0) return false;

            player.Inventory.NumKeys -= 1;
            _isUnlocked = true;
            SoundManager.Instance.PlaySFX(RoomConstants.KeyUnlockSFX);
            return true;
        }
    }

    public sealed class CustomDoorCondition : IDoorUnlockCondition
    {
        public bool IsSatisfied(IDoorContext roomContext)
        {
            return false;
        }

        public bool TryUnlock(IPlayer player, IDoorContext roomContext)
        {
            return false;
        }
    }
}