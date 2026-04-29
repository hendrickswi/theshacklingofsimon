#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

#endregion

namespace TheShacklingOfSimon.Controllers.Mouse;

public interface IMouseService
{
    void Update();
    Vector2 GetPosition();
    InputState GetButtonState(MouseButton button);
    IEnumerable<MouseButton> GetPressedButtons();
}