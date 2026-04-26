#region

using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

#endregion

namespace TheShacklingOfSimon.Controllers.Mouse;

public record struct MouseInput(
    MouseInputRegion Region, 
    MouseButton Button, 
    InputState State
);