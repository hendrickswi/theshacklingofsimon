using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

namespace TheShacklingOfSimon.Controllers.Keyboard;

public class KeyboardController : IController<KeyboardInput>
{
    private readonly IKeyboardService _keyboardService;
    private readonly Dictionary<KeyboardInput, Commands.ICommand> _map;
    private readonly Dictionary<KeyboardButton, InputState> _previousStates;

	public KeyboardController(IKeyboardService service)
    {
        _keyboardService = service;
        _map = new Dictionary<KeyboardInput, Commands.ICommand>();
        _previousStates = new Dictionary<KeyboardButton, InputState>();
	}

	public void RegisterCommand(KeyboardInput k, Commands.ICommand cmd)
	{
		bool success = _map.TryAdd(k, cmd);
		if (success)
		{
			_previousStates.Add(k.Button, InputState.Released);
		}
	}

	public void UnregisterCommand(KeyboardInput input)
	{
		bool success = _map.Remove(input);
		if (success)
		{
			_previousStates.Remove(input.Button);
		}
	}

	public void ClearCommands()
	{
		_map.Clear();
		_previousStates.Clear();
	}

	public void Update()
	{
		foreach (KeyboardInput k in _map.Keys)
		{
			InputState currentState = _keyboardService.GetKeyState(k.Button);
			InputState previousState = _previousStates[k.Button];

			bool isJustPressed =
				currentState == InputState.Pressed &&
				previousState == InputState.Released;

			if (
				(k.State == InputState.Pressed && currentState == InputState.Pressed) ||
				(k.State == InputState.Released && currentState == InputState.Released) ||
				(k.State == InputState.JustPressed && isJustPressed)
			)
			{
				_map[k].Execute();
			}

			_previousStates[k.Button] = currentState;
		}
	}

}