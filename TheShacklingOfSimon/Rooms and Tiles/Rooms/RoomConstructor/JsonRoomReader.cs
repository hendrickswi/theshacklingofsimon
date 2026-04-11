#region

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Content;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor
{
    // Reads room JSON files from Room_Manager/Rooms/Room_Constructor/Room_Jsons
    public sealed class JsonRoomReader
    {
        private readonly ContentManager content;
        private readonly JsonSerializerOptions options;

        // Folder under Content.RootDirectory where room json files live
        private static readonly string RoomFolder =
            Path.Combine("Room_Jsons");

        public JsonRoomReader(ContentManager content)
        {
            this.content = content;
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Allows: "type": "Rock" -> TileType.Rock
            options.Converters.Add(new JsonStringEnumConverter());
        }

        // roomId matches filename: "room_01" -> ".../Room_Jsons/room_01.json"
        public RoomFileData Read(string roomId)
        {
            string relativePath = Path.Combine(RoomFolder, $"{roomId}.json");
            string fullPath = Path.Combine(content.RootDirectory, relativePath);
            string absolutePath = Path.GetFullPath(fullPath);

            //Debug.WriteLine($"READ ROOM ID: {roomId}");
            //Debug.WriteLine($"READ ROOM PATH: {absolutePath}");

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Room file not found at {fullPath}");

            string json = File.ReadAllText(fullPath);

            //Debug.WriteLine("RAW JSON START");
            //Debug.WriteLine(json);
            //Debug.WriteLine("RAW JSON END");

            RoomFileData data = JsonSerializer.Deserialize<RoomFileData>(json, options)
                                ?? throw new InvalidOperationException($"Failed to deserialize room file {fullPath}");

            if (string.IsNullOrWhiteSpace(data.Id))
                data.Id = roomId;

            return data;
        }
    }
}