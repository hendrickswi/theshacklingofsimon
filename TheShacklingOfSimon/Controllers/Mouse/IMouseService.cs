using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Mouse;

public interface IMouseService
{
    XYPoint GetPosition();
    BinaryInputState GetButtonState(MouseButton button);
}