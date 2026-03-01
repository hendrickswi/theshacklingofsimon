using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Text.Json;

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomManager
{
    public sealed class RoomIndexReader
    {
        private readonly ContentManager content;
        private readonly JsonSerializerOptions options;

        private static readonly string RoomFolder =
            Path.Combine("Room_Jsons");

        public RoomIndexReader(ContentManager content)
        {
            this.content = content;
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public RoomIndexData ReadIndex()
        {
            string relativePath = Path.Combine(RoomFolder, "room_index.json");
            string fullPath = Path.Combine(content.RootDirectory, relativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Room index file not found at {fullPath}");

            string json = File.ReadAllText(fullPath);

            RoomIndexData data = JsonSerializer.Deserialize<RoomIndexData>(json, options)
                                 ?? throw new InvalidOperationException($"Failed to deserialize room index file {fullPath}");

            if (data.Rooms == null)
                data.Rooms = new System.Collections.Generic.List<string>();

            return data;
        }
    }
}