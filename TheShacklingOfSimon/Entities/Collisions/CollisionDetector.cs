using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Collisions;

public static class CollisionDetector
{
    public static bool CheckRectangleCollision(Rectangle r1, Rectangle r2)
    {
        return r1.Intersects(r2);
    }

    public static CollisionSide GetCollisionSide(Rectangle r1, Rectangle r2)
    {
        CollisionSide result = CollisionSide.None;
        
        if (r1.Intersects(r2))
        {
            // TODO
        }

        return result;
    }
}