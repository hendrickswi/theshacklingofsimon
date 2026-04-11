#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomManager
{
    //  for now use this so door tiles do not need to know about the full RoomManager.
    public interface IRoomNavigator
    {
        void RequestRoomSwitch(string roomId, Point spawnGrid, IPlayer player);
    }
}