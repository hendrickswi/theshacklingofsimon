using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Class
{
    // Room grid dimensions and shared constants.
    // Interior is the playable 13x7 area. The full grid includes a 1-tile wall border.
    public static class RoomConstants
    {
        // Tile size in pixels.
        public const int TileSize = 64;

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
    }
}