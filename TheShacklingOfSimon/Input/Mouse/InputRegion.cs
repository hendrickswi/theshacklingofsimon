namespace TheShacklingOfSimon.Input.Mouse;

public struct InputRegion
{
    public int X;
    public int Y;
    public int Width;
    public int Height;

    public InputRegion(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool ContainsPoint(int x, int y)
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