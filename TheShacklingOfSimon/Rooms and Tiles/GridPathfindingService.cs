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
        if (!TryGetNextWaypoint(
                currentPosition,
                targetPosition,
                canTraverse,
                getTraversalCost,
                out Vector2 nextWaypoint))
        {
            return Vector2.Zero;
        }

        Vector2 direction = nextWaypoint - currentPosition;

        if (direction.LengthSquared() < 0.0001f)
        {
            return Vector2.Zero;
        }

        direction.Normalize();
        return direction;
    }

    public bool TryGetNextWaypoint(
        Vector2 currentPosition,
        Vector2 targetPosition,
        Func<ITile, bool> canTraverse,
        Func<ITile, float> getTraversalCost,
        out Vector2 nextWaypoint)
    {
        nextWaypoint = Vector2.Zero;

        Point start = _tileMap.WorldToGrid(currentPosition);
        Point goal = _tileMap.WorldToGrid(targetPosition);

        if (!_tileMap.InBounds(start) || !_tileMap.InBounds(goal) || start == goal)
        {
            return false;
        }

        Point next = FindNextStep(start, goal, canTraverse, getTraversalCost);

        if (next == start)
        {
            return false;
        }

        nextWaypoint = GetCellCenter(next);
        return true;
    }

    public bool TryGetNextWaypointAwayFromThreat(
        Vector2 currentPosition,
        Vector2 threatPosition,
        Func<ITile, bool> canTraverse,
        Func<ITile, float> getTraversalCost,
        out Vector2 nextWaypoint)
    {
        nextWaypoint = Vector2.Zero;

        Point start = _tileMap.WorldToGrid(currentPosition);

        if (!_tileMap.InBounds(start))
        {
            return false;
        }

        Dictionary<Point, Point?> cameFrom = BuildReachableMap(start, canTraverse, getTraversalCost);

        if (cameFrom.Count <= 1)
        {
            return false;
        }

        Point bestCell = start;
        float bestDistanceSquared = Vector2.DistanceSquared(GetCellCenter(start), threatPosition);

        foreach (Point cell in cameFrom.Keys)
        {
            Vector2 cellCenter = GetCellCenter(cell);
            float distanceSquared = Vector2.DistanceSquared(cellCenter, threatPosition);

            if (distanceSquared > bestDistanceSquared)
            {
                bestDistanceSquared = distanceSquared;
                bestCell = cell;
            }
        }

        if (bestCell == start)
        {
            return false;
        }

        return TryGetFirstStepTowardCell(start, bestCell, cameFrom, out nextWaypoint);
    }

    public bool TryGetNextWaypointToAlignmentWithTarget(
        Vector2 currentPosition,
        Vector2 targetPosition,
        Func<ITile, bool> canTraverse,
        Func<ITile, float> getTraversalCost,
        out Vector2 nextWaypoint)
    {
        nextWaypoint = Vector2.Zero;

        Point start = _tileMap.WorldToGrid(currentPosition);
        Point target = _tileMap.WorldToGrid(targetPosition);

        if (!_tileMap.InBounds(start) || !_tileMap.InBounds(target))
        {
            return false;
        }

        Dictionary<Point, Point?> cameFrom = BuildReachableMap(start, canTraverse, getTraversalCost);

        if (cameFrom.Count <= 1)
        {
            return false;
        }

        Point bestCell = start;
        int bestPathLength = int.MaxValue;
        float bestDistanceToTarget = float.MaxValue;

        foreach (Point cell in cameFrom.Keys)
        {
            if (cell == start || cell == target)
            {
                continue;
            }

            bool sameColumn = cell.X == target.X;
            bool sameRow = cell.Y == target.Y;

            if (!sameColumn && !sameRow)
            {
                continue;
            }

            // Only choose a row/column position if a projectile can actually
            // travel from that tile to the player without hitting a blocking tile.
            if (!HasClearCardinalLineOfSight(cell, target))
            {
                continue;
            }

            int pathLength = GetPathLengthFromStart(cell, cameFrom);
            float distanceToTarget = Vector2.DistanceSquared(GetCellCenter(cell), targetPosition);

            if (pathLength < bestPathLength ||
                pathLength == bestPathLength && distanceToTarget < bestDistanceToTarget)
            {
                bestCell = cell;
                bestPathLength = pathLength;
                bestDistanceToTarget = distanceToTarget;
            }
        }

        if (bestCell == start)
        {
            return false;
        }

        return TryGetFirstStepTowardCell(start, bestCell, cameFrom, out nextWaypoint);
    }

    public bool HasClearShot(Vector2 fromWorld, Vector2 toWorld)
    {
        Point from = _tileMap.WorldToGrid(fromWorld);
        Point to = _tileMap.WorldToGrid(toWorld);

        if (!_tileMap.InBounds(from) || !_tileMap.InBounds(to))
        {
            return false;
        }

        return HasClearCardinalLineOfSight(from, to);
    }

    public bool IsAreaSafe(Rectangle area, Func<ITile, bool> canTraverse)
    {
        if (area.Left < _tileMap.RoomBoundsWorld.Left ||
            area.Right > _tileMap.RoomBoundsWorld.Right ||
            area.Top < _tileMap.RoomBoundsWorld.Top ||
            area.Bottom > _tileMap.RoomBoundsWorld.Bottom)
        {
            return false;
        }

        foreach (ITile tile in _tileMap.GetTilesIntersecting(area))
        {
            if (tile == null)
            {
                continue;
            }

            if (canTraverse != null && !canTraverse(tile))
            {
                return false;
            }
        }

        return true;
    }

    private Dictionary<Point, Point?> BuildReachableMap(
        Point start,
        Func<ITile, bool> canTraverse,
        Func<ITile, float> getTraversalCost)
    {
        var frontier = new Queue<Point>();
        var cameFrom = new Dictionary<Point, Point?>();

        frontier.Enqueue(start);
        cameFrom[start] = null;

        while (frontier.Count > 0)
        {
            Point current = frontier.Dequeue();

            foreach (Point neighbor in GetNeighbors(current, canTraverse))
            {
                if (cameFrom.ContainsKey(neighbor))
                {
                    continue;
                }

                float traversalCost = GetCellTraversalCost(neighbor, getTraversalCost);

                if (float.IsPositiveInfinity(traversalCost))
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                frontier.Enqueue(neighbor);
            }
        }

        return cameFrom;
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

    private bool TryGetFirstStepTowardCell(
        Point start,
        Point goal,
        Dictionary<Point, Point?> cameFrom,
        out Vector2 nextWaypoint)
    {
        nextWaypoint = Vector2.Zero;

        Point step = goal;

        while (cameFrom[step].HasValue && cameFrom[step].Value != start)
        {
            step = cameFrom[step].Value;
        }

        if (step == start)
        {
            return false;
        }

        nextWaypoint = GetCellCenter(step);
        return true;
    }

    private static int GetPathLengthFromStart(Point cell, Dictionary<Point, Point?> cameFrom)
    {
        int length = 0;
        Point step = cell;

        while (cameFrom[step].HasValue)
        {
            length++;
            step = cameFrom[step].Value;
        }

        return length;
    }

    private bool HasClearCardinalLineOfSight(Point from, Point to)
    {
        if (from.X != to.X && from.Y != to.Y)
        {
            return false;
        }

        Point direction;

        if (from.X == to.X)
        {
            direction = new Point(0, Math.Sign(to.Y - from.Y));
        }
        else
        {
            direction = new Point(Math.Sign(to.X - from.X), 0);
        }

        Point current = new Point(from.X + direction.X, from.Y + direction.Y);

        while (current != to)
        {
            if (!_tileMap.InBounds(current))
            {
                return false;
            }

            if (_tileMap.TryGetTile(current, out ITile tile))
            {
                if (tile.BlocksProjectiles)
                {
                    return false;
                }
            }

            current = new Point(current.X + direction.X, current.Y + direction.Y);
        }

        return true;
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