using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Controllers.Gamepad;

public record struct GamepadButtonInput
{
    public InputState State;
    public GamepadButton Button;

    public GamepadButtonInput(InputState state, GamepadButton button)
    {
        State = state;
        Button = button;
    }
}