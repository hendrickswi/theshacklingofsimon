#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;

#endregion

namespace TheShacklingOfSimon.UI
{
    internal sealed class MiniMap
    {
        private readonly RoomManager roomManager;
        private readonly Texture2D pixel;

        private readonly HashSet<string> visitedRooms = new();
        private readonly Dictionary<string, Point> roomPositions = new();

        private const int CellSize = 18;
        private const int CellGap = 4;
        private const int PanelPadding = 10;
        private const int PanelMargin = 20;
        private const int BorderThickness = 2;

        private static readonly Color PanelBackgroundColor = new(0, 0, 0, 160);
        private static readonly Color PanelBorderColor = new(220, 220, 220, 180);
        private static readonly Color VisitedRoomColor = new(90, 170, 255, 255);
        private static readonly Color FrontierRoomColor = new(80, 80, 80, 255);
        private static readonly Color CurrentRoomColor = new(255, 230, 120, 255);
        private static readonly Color CurrentRoomBorderColor = Color.White;

        public MiniMap(RoomManager roomManager, GraphicsDevice graphicsDevice)
        {
            this.roomManager = roomManager ?? throw new ArgumentNullException(nameof(roomManager));
            pixel = CreatePixel(graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice)));

            BuildRoomLayout();
            TrackVisitedRooms();
        }

        private static Texture2D CreatePixel(GraphicsDevice graphicsDevice)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }

        private void TrackVisitedRooms()
        {
            MarkVisited(roomManager.CurrentRoomId);
            roomManager.RoomChanged += room => MarkVisited(room.Id);
        }

        private void MarkVisited(string roomId)
        {
            if (!string.IsNullOrWhiteSpace(roomId))
            {
                visitedRooms.Add(roomId);
            }
        }

        // I build the minimap layout once from the starting room and derive neighboring
        // room positions from door directions so UI code does not need room-file knowledge.
        private void BuildRoomLayout()
        {
            roomPositions.Clear();

            string rootRoomId = roomManager.StartingRoomId;
            if (string.IsNullOrWhiteSpace(rootRoomId))
            {
                return;
            }

            Queue<string> queue = new Queue<string>();
            roomPositions[rootRoomId] = Point.Zero;
            queue.Enqueue(rootRoomId);

            while (queue.Count > 0)
            {
                string currentRoomId = queue.Dequeue();
                Point currentPosition = roomPositions[currentRoomId];

                foreach (RoomConnection connection in roomManager.GetConnections(currentRoomId))
                {
                    if (string.IsNullOrWhiteSpace(connection.ToRoomId))
                    {
                        continue;
                    }

                    Point neighborPosition = GetNeighborPosition(currentPosition, connection.Direction);

                    if (roomPositions.ContainsKey(connection.ToRoomId))
                    {
                        continue;
                    }

                    roomPositions[connection.ToRoomId] = neighborPosition;
                    queue.Enqueue(connection.ToRoomId);
                }
            }
        }

        private static Point GetNeighborPosition(Point origin, MapDirection direction)
        {
            return direction switch
            {
                MapDirection.North => new Point(origin.X, origin.Y - 1),
                MapDirection.South => new Point(origin.X, origin.Y + 1),
                MapDirection.West => new Point(origin.X - 1, origin.Y),
                MapDirection.East => new Point(origin.X + 1, origin.Y),
                _ => origin
            };
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null || roomPositions.Count == 0)
            {
                return;
            }

            HashSet<string> frontierRooms = GetFrontierRooms();
            HashSet<string> visibleRooms = GetVisibleRooms(frontierRooms);

            if (visibleRooms.Count == 0)
            {
                return;
            }

            GetVisibleBounds(visibleRooms, out Point minVisible, out Point maxVisible);
            Rectangle panelBounds = BuildPanelBounds(spriteBatch.GraphicsDevice.Viewport, minVisible, maxVisible);

            DrawFilledRectangle(spriteBatch, panelBounds, PanelBackgroundColor);
            DrawRectangleOutline(spriteBatch, panelBounds, PanelBorderColor, BorderThickness);

            foreach (string roomId in visibleRooms)
            {
                if (!roomPositions.TryGetValue(roomId, out Point roomPosition))
                {
                    continue;
                }

                Rectangle roomRectangle = GetRoomRectangle(roomPosition, minVisible, panelBounds);

                if (roomId == roomManager.CurrentRoomId)
                {
                    DrawFilledRectangle(spriteBatch, roomRectangle, CurrentRoomColor);
                    DrawRectangleOutline(spriteBatch, roomRectangle, CurrentRoomBorderColor, BorderThickness);
                }
                else if (visitedRooms.Contains(roomId))
                {
                    DrawFilledRectangle(spriteBatch, roomRectangle, VisitedRoomColor);
                }
                else if (frontierRooms.Contains(roomId))
                {
                    DrawFilledRectangle(spriteBatch, roomRectangle, FrontierRoomColor);
                }
            }
        }

        // Only reveal unvisited rooms that touch something already visited
        // so the player does not see the full dungeon layout immediately.
        private HashSet<string> GetFrontierRooms()
        {
            HashSet<string> frontierRooms = new HashSet<string>();

            foreach (string visitedRoomId in visitedRooms)
            {
                foreach (RoomConnection connection in roomManager.GetConnections(visitedRoomId))
                {
                    if (!visitedRooms.Contains(connection.ToRoomId))
                    {
                        frontierRooms.Add(connection.ToRoomId);
                    }
                }
            }

            return frontierRooms;
        }

        private HashSet<string> GetVisibleRooms(HashSet<string> frontierRooms)
        {
            HashSet<string> visibleRooms = new HashSet<string>(visitedRooms);

            foreach (string roomId in frontierRooms)
            {
                visibleRooms.Add(roomId);
            }

            return visibleRooms;
        }

        private static void GetVisibleBounds(HashSet<string> visibleRooms, out Point minVisible, out Point maxVisible, Dictionary<string, Point>? positions = null)
        {
            minVisible = new Point(int.MaxValue, int.MaxValue);
            maxVisible = new Point(int.MinValue, int.MinValue);

            if (positions == null)
            {
                return;
            }

            foreach (string roomId in visibleRooms)
            {
                if (!positions.TryGetValue(roomId, out Point position))
                {
                    continue;
                }

                minVisible.X = Math.Min(minVisible.X, position.X);
                minVisible.Y = Math.Min(minVisible.Y, position.Y);
                maxVisible.X = Math.Max(maxVisible.X, position.X);
                maxVisible.Y = Math.Max(maxVisible.Y, position.Y);
            }
        }

        private void GetVisibleBounds(HashSet<string> visibleRooms, out Point minVisible, out Point maxVisible)
        {
            minVisible = new Point(int.MaxValue, int.MaxValue);
            maxVisible = new Point(int.MinValue, int.MinValue);

            foreach (string roomId in visibleRooms)
            {
                if (!roomPositions.TryGetValue(roomId, out Point position))
                {
                    continue;
                }

                minVisible.X = Math.Min(minVisible.X, position.X);
                minVisible.Y = Math.Min(minVisible.Y, position.Y);
                maxVisible.X = Math.Max(maxVisible.X, position.X);
                maxVisible.Y = Math.Max(maxVisible.Y, position.Y);
            }
        }

        private static Rectangle BuildPanelBounds(Viewport viewport, Point minVisible, Point maxVisible)
        {
            int widthInCells = (maxVisible.X - minVisible.X) + 1;
            int heightInCells = (maxVisible.Y - minVisible.Y) + 1;

            int panelWidth = (PanelPadding * 2) + (widthInCells * CellSize) + ((widthInCells - 1) * CellGap);
            int panelHeight = (PanelPadding * 2) + (heightInCells * CellSize) + ((heightInCells - 1) * CellGap);

            return new Rectangle(
                viewport.Width - panelWidth - PanelMargin,
                PanelMargin,
                panelWidth,
                panelHeight);
        }

        private static Rectangle GetRoomRectangle(Point roomPosition, Point minVisible, Rectangle panelBounds)
        {
            int localX = roomPosition.X - minVisible.X;
            int localY = roomPosition.Y - minVisible.Y;

            int x = panelBounds.X + PanelPadding + (localX * (CellSize + CellGap));
            int y = panelBounds.Y + PanelPadding + (localY * (CellSize + CellGap));

            return new Rectangle(x, y, CellSize, CellSize);
        }

        private void DrawFilledRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(pixel, rectangle, color);
        }

        private void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, thickness), color);
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Left, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Left, rectangle.Top, thickness, rectangle.Height), color);
            spriteBatch.Draw(pixel, new Rectangle(rectangle.Right - thickness, rectangle.Top, thickness, rectangle.Height), color);
        }

        public void Reset()
        {
            visitedRooms.Clear();
            MarkVisited(roomManager.CurrentRoomId);
        }
    }
}