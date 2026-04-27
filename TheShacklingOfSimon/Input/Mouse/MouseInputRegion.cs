namespace TheShacklingOfSimon.Input.Mouse;

public readonly record struct MouseInputRegion(float X, float Y, float Width, float Height)
{
    public bool ContainsPoint(float x, float y)
    {
        if (x >= X && x <= X + Width && y >= Y && y <= Y + Height)
        {
            return true;
        }
        return false;
    }
}