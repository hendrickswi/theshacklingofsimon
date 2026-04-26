#region

using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

#endregion

namespace TheShacklingOfSimon.Controllers.Keyboard;

public record struct KeyboardInput(
    KeyboardButton Button, 
    InputState State
);