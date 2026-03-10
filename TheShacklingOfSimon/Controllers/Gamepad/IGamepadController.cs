namespace TheShacklingOfSimon.Controllers.Gamepad;

public interface IGamepadController : IController<GamepadButtonInput>, IController<GamepadJoystickInput>
{
    new void Update();
    new void ClearCommands();
}