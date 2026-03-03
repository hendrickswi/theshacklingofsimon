using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Mouse;

public interface IMouseService
{
    Vector2 GetPosition();
    InputState GetButtonState(MouseButton button);
}