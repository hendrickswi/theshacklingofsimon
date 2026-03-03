using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Mouse;

public record struct MouseInput
{
    public MouseInputRegion Region;
    public InputState State;
    public MouseButton Button;

    public MouseInput(MouseInputRegion region, InputState state, MouseButton button)
    {
        Region = region;
        State = state;
        Button = button;
    }
}