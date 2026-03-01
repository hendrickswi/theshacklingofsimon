namespace TheShacklingOfSimon.Commands.Tile_Commands_and_temporary_Manager
{
    public sealed class NextRoomCommand : ICommand
    {
        private readonly TheShacklingOfSimon.Level_Handler.Rooms.RoomManager.RoomManager roomManager;

        public NextRoomCommand(TheShacklingOfSimon.Level_Handler.Rooms.RoomManager.RoomManager roomManager)
        {
            this.roomManager = roomManager;
        }

        public void Execute()
        {
            roomManager.NextRoom();
        }
    }
}