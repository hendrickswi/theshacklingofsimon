using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

namespace TheShacklingOfSimon.Controllers.Keyboard;

public interface IKeyboardController : IController<KeyboardInput>
{
    IEnumerable<KeyboardButton> GetPressedKeys();
    IEnumerable<KeyboardButton> GetJustPressedKeys();
    InputState GetKeyState(KeyboardButton key);
}