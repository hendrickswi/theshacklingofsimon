#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles;

public sealed class GridPathfindingService : IPathfindingService
{
    private readonly TileMap _tileMap;

    private static readonly Point[] CardinalDirections =
    {
        new Point(0, -1),
        new Point(1, 0),
        new Point(0, 1),
        new Point(-1, 0)
    };

    public GridPathfindingService(TileMap tileMap)
    {
        _tileMap = tileMap ?? throw new ArgumentNullException(nameof(tileMap));
    }

    public Vector2 GetNextDirection(
        Vector2 currentPosition,
        Vector2 targetPosition,
        Func<ITile, bool> canTraverse,
        Func<ITile, float> getTraversalCost)
    {
        Point start = _tileMap.WorldToGrid(currentPosition);
        Point goal = _tileMap.WorldToGrid(targetPosition);

        if (!_tileMap.InBounds(start) || !_tileMap.InBounds(goal) || start == goal)
        {
            return Vector2.Zero;
        }

        Point next = FindNextStep(start, goal, canTraverse, getTraversalCost);
        if (next == start)
        {
            return Vector2.Zero;
        }

        Vector2 startWorld = GetCellCenter(start);
        Vector2 nextWorld = GetCellCenter(next);
        Vector2 direction = nextWorld - startWorld;

        if (direction.LengthSquared() < 0.0001f)
        {
            return Vector2.Zero;
        }

        direction.Normalize();
        return direction;
    }

    private Point FindNextStep(
        Point start,
        Point goal,
        Func<ITile, bool> canTraverse,
        Func<ITile, float> getTraversalCost)
    {
        var frontier = new PriorityQueue<Point, float>();
        var cameFrom = new Dictionary<Point, Point?>();
        var costSoFar = new Dictionary<Point, float>();

        frontier.Enqueue(start, 0f);
        cameFrom[start] = null;
        costSoFar[start] = 0f;

        while (frontier.Count > 0)
        {
            Point current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Point neighbor in GetNeighbors(current, canTraverse))
            {
                float traversalCost = GetCellTraversalCost(neighbor, getTraversalCost);
                if (float.IsPositiveInfinity(traversalCost))
                {
                    continue;
                }

                float newCost = costSoFar[current] + traversalCost;

                if (!costSoFar.TryGetValue(neighbor, out float oldCost) || newCost < oldCost)
                {
                    costSoFar[neighbor] = newCost;

                    float priority = newCost + Heuristic(neighbor, goal);
                    frontier.Enqueue(neighbor, priority);

                    cameFrom[neighbor] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            return start;
        }

        Point step = goal;
        while (cameFrom[step].HasValue && cameFrom[step].Value != start)
        {
            step = cameFrom[step].Value;
        }

        return step;
    }

    private IEnumerable<Point> GetNeighbors(Point cell, Func<ITile, bool> canTraverse)
    {
        foreach (Point offset in CardinalDirections)
        {
            Point neighbor = new Point(cell.X + offset.X, cell.Y + offset.Y);

            if (!_tileMap.InBounds(neighbor))
            {
                continue;
            }

            if (!IsWalkable(neighbor, canTraverse))
            {
                continue;
            }

            yield return neighbor;
        }
    }

    private bool IsWalkable(Point cell, Func<ITile, bool> canTraverse)
    {
        if (!_tileMap.TryGetTile(cell, out ITile tile))
        {
            return true;
        }

        return canTraverse == null || canTraverse(tile);
    }

    private float GetCellTraversalCost(Point cell, Func<ITile, float> getTraversalCost)
    {
        if (!_tileMap.TryGetTile(cell, out ITile tile))
        {
            return 1f;
        }

        if (getTraversalCost == null)
        {
            return 1f;
        }

        float cost = getTraversalCost(tile);
        return cost <= 0f ? 1f : cost;
    }

    private static float Heuristic(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private Vector2 GetCellCenter(Point cell)
    {
        Vector2 topLeft = _tileMap.GridToWorld(cell);
        float halfTile = RoomConstants.TileSize * 0.5f;
        return topLeft + new Vector2(halfTile, halfTile);
    }
}