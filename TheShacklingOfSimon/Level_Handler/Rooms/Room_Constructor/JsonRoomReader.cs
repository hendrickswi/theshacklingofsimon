using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Text.Json;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Class;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Constructor
{
    // Reads room JSON files from Room_Manager/Rooms/Room_Constructor/Room_Jsons
    public sealed class JsonRoomReader
    {
        private readonly ContentManager content;
        private readonly JsonSerializerOptions options;

        // Folder under Content.RootDirectory where room json files live
        private static readonly string RoomFolder =
            Path.Combine("Room_Manager", "Rooms", "Room_Constructor", "Room_Jsons");

        public JsonRoomReader(ContentManager content)
        {
            this.content = content;
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // roomId matches filename: "room_01" -> ".../Room_Jsons/room_01.json"
        public RoomFileData Read(string roomId)
        {
            string relativePath = Path.Combine(RoomFolder, $"{roomId}.json");
            string fullPath = Path.Combine(content.RootDirectory, relativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Room file not found at {fullPath}");

            string json = File.ReadAllText(fullPath);

            RoomFileData data = JsonSerializer.Deserialize<RoomFileData>(json, options)
                                ?? throw new InvalidOperationException($"Failed to deserialize room file {fullPath}");

            if (string.IsNullOrWhiteSpace(data.Id))
                data.Id = roomId;

            return data;
        }
    }
}