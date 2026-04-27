#region

using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

#endregion

namespace TheShacklingOfSimon.Controllers.Keyboard;

public readonly record struct KeyboardInput(
    KeyboardButton Button, 
    InputState State
);