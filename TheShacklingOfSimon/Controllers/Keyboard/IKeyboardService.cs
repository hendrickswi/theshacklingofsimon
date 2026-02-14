using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Keyboard;

public interface IKeyboardService
{
    BinaryInputState GetKeyState(KeyboardButton button);
}