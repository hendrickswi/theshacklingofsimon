#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Commands.Temporary_Commands;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass
{
    // Room grid dimensions and shared constants.
    // Interior is the playable 13x7 area. The full grid includes a 1-tile wall border.
    public static class RoomConstants
    {
        // Tile size in pixels.
        public const int TileSize = 32;

        // Playable interior dimensions (from JSON coordinates).
        public const int InteriorWidth = 13;
        public const int InteriorHeight = 7;

        // Border thickness (wall ring).
        public const int Border = 1;

        // Full grid dimensions (includes border).
        public const int GridWidth = InteriorWidth + (2 * Border);   // 15
        public const int GridHeight = InteriorHeight + (2 * Border); // 9

        // Useful derived values.
        public const int PixelWidth = GridWidth * TileSize;
        public const int PixelHeight = GridHeight * TileSize;

        // Offset to map interior coords -> full-grid coords.
        public static Point InteriorOffset => new Point(Border, Border);

        public static Rectangle WorldBounds => new Rectangle(0, 0, PixelWidth, PixelHeight);

        // Misc SFX that don't really fit elsewhere
        public static string EnemiesClearedSFX = SoundManager.Instance.AddSFX("enemy", "deathburst"); // clear room of enemies
        public static string DoorOpenSFX = SoundManager.Instance.AddSFX("other", "doorOpen"); // open door from enemy clear
        public static string KeyUnlockSFX = SoundManager.Instance.AddSFX("other", "unlock"); // open door from key
        public static string DoorCloseSFX = SoundManager.Instance.AddSFX("other", "doorClose"); // close door in enemy room
    }
}