using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Keyboard;

public record struct KeyboardInput
{
    public BinaryInputState State;
    public KeyboardButton Button;

    public KeyboardInput(BinaryInputState state, KeyboardButton button)
    {
        State = state;
        Button = button;
    }
}