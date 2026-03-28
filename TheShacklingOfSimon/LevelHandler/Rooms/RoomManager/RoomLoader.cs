using System;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor;

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomManager
{
    // I split room creation/loading out of RoomManager so RoomManager can focus on navigation/state.
    public sealed class RoomLoader
    {
        private readonly JsonRoomReader roomReader;
        private readonly RoomFactory factory;
        private readonly GraphicsDevice graphicsDevice;

        public RoomLoader(
            JsonRoomReader roomReader,
            RoomFactory factory,
            GraphicsDevice graphicsDevice)
        {
            this.roomReader = roomReader ?? throw new ArgumentNullException(nameof(roomReader));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        }

        public RoomFileData ReadData(string roomId)
        {
            return roomReader.Read(roomId);
        }

        public Room CreateRoom(RoomFileData data)
        {
            var viewport = graphicsDevice.Viewport;
            return factory.Create(data, viewport.Width, viewport.Height);
        }
    }
}