using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Mouse;

public interface IMouseController : IController<MouseInput>
{
    IEnumerable<MouseButton> GetPressedButtons();
    IEnumerable<MouseButton> GetJustPressedButtons();
    InputState GetButtonState(MouseButton button);
}