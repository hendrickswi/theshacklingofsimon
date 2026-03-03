namespace TheShacklingOfSimon.Input.Mouse;

public struct MouseInputRegion
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public MouseInputRegion(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool ContainsPoint(float x, float y)
    {
        bool contains = false;
        if (x >= X && x <= X + Width &&
            y >= Y && y <= Y + Height)
        {
            contains = true;
        }

        return contains;
    }
}