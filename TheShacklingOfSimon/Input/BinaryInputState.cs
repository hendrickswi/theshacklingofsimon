namespace TheShacklingOfSimon.Input;

public enum BinaryInputState
{
    Pressed, //key is held down
    Released, // key is up
    JustPressed // key has just been held down ie, for switching but not doing every frame
}