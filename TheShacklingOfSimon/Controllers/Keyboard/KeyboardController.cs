using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Keyboard;

public class KeyboardController : IController<KeyboardInput>
{
    private IKeyboardService _keyboardService;
    private Dictionary<KeyboardInput, Commands.ICommand> _map;

    public KeyboardController(IKeyboardService service)
    {
        _keyboardService = service;
        _map = new Dictionary<KeyboardInput, Commands.ICommand>();
    }
    
    public void RegisterCommand(KeyboardInput k, Commands.ICommand cmd)
    {
        // Avoid an ArgumentException if k is already in the map.
        _map.TryAdd(k, cmd);
    }

    public void Update()
    {
        foreach (KeyboardInput k in _map.Keys)
        {
            if (_keyboardService.GetKeyState(k.Button) == BinaryInputState.Pressed)
            {
                _map[k].Execute();
            }
        }
    }
}