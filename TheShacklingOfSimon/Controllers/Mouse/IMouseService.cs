#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input.Mouse;

#endregion

namespace TheShacklingOfSimon.Controllers.Mouse;

public interface IMouseService
{
    Vector2 GetPosition();
    bool GetButtonState(MouseButton button);
    IEnumerable<MouseButton> GetPressedButtons();
}