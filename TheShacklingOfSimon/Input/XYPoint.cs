namespace TheShacklingOfSimon.Input;

public struct XYPoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public XYPoint(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}