using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Keyboard;

public class KeyboardController : IController<KeyboardInput>
{
    private IKeyboardService _keyboardService;
    private Dictionary<KeyboardInput, Commands.ICommand> _map;
    private Dictionary<KeyboardButton, BinaryInputState> _previousStates;

	public KeyboardController(IKeyboardService service)
    {
        _keyboardService = service;
        _map = new Dictionary<KeyboardInput, Commands.ICommand>();
        _previousStates = new Dictionary<KeyboardButton, BinaryInputState>();
	}

	public void RegisterCommand(KeyboardInput k, Commands.ICommand cmd)
	{
		_map.TryAdd(k, cmd);

		if (!_previousStates.ContainsKey(k.Button))
		{
			_previousStates[k.Button] = BinaryInputState.Released;
		}
	}


	public void Update()
	{
		foreach (KeyboardInput k in _map.Keys)
		{
			BinaryInputState currentState = _keyboardService.GetKeyState(k.Button);
			BinaryInputState previousState = _previousStates[k.Button];

			bool isJustPressed =
				currentState == BinaryInputState.Pressed &&
				previousState == BinaryInputState.Released;

			if (
				(k.State == BinaryInputState.Pressed && currentState == BinaryInputState.Pressed) ||
				(k.State == BinaryInputState.Released && currentState == BinaryInputState.Released) ||
				(k.State == BinaryInputState.JustPressed && isJustPressed)
			)
			{
				_map[k].Execute();
			}

			_previousStates[k.Button] = currentState;
		}
	}

}