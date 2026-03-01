namespace TheShacklingOfSimon.Commands.Tile_Commands_and_temporary_Manager
{
    public sealed class PreviousRoomCommand : ICommand
    {
        private readonly TheShacklingOfSimon.Level_Handler.Rooms.RoomManager.RoomManager roomManager;

        public PreviousRoomCommand(TheShacklingOfSimon.Level_Handler.Rooms.RoomManager.RoomManager roomManager)
        {
            this.roomManager = roomManager;
        }

        public void Execute()
        {
            roomManager.PrevRoom();
        }
    }
}