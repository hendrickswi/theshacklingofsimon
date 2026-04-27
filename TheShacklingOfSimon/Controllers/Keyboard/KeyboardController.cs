#region

using System;
using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

#endregion

namespace TheShacklingOfSimon.Controllers.Keyboard;

public class KeyboardController : IController<KeyboardInput>
{
    private readonly IKeyboardService _keyboardService;
    private readonly Dictionary<KeyboardInput, ICommand> _map;
    private readonly Dictionary<KeyboardButton, InputState> _previousStates;

    public KeyboardController(IKeyboardService service)
    {
        _keyboardService = service;
        _map = new Dictionary<KeyboardInput, ICommand>();
        _previousStates = new Dictionary<KeyboardButton, InputState>();
    }

    public void RegisterCommand(KeyboardInput input, ICommand command)
    {
        bool success = _map.TryAdd(input, command);
        if (success && !_previousStates.ContainsKey(input.Button))
        {
            _previousStates.Add(input.Button, InputState.Released);
        }
    }

    public void UnregisterCommand(KeyboardInput input)
    {
        bool success = _map.Remove(input);
        if (!success)
        {
            return;
        }

        bool buttonStillUsed = _map.Keys.Any(existingInput => existingInput.Button.Equals(input.Button));
        if (!buttonStillUsed)
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
        // we use a snapshot here so commands can safely change bindings during update.
        KeyValuePair<KeyboardInput, ICommand>[] bindings = _map.ToArray();

        foreach (KeyValuePair<KeyboardInput, ICommand> entry in bindings)
        {
            KeyboardInput input = entry.Key;
            ICommand command = entry.Value;

            if (!_previousStates.ContainsKey(input.Button))
            {
                _previousStates[input.Button] = InputState.Released;
            }

            InputState currentState = _keyboardService.GetKeyState(input.Button);
            InputState previousState = _previousStates[input.Button];

            bool isJustPressed =
                currentState == InputState.Pressed &&
                previousState == InputState.Released;

            if (
                (input.State == InputState.Pressed && currentState == InputState.Pressed) ||
                (input.State == InputState.Released && currentState == InputState.Released) ||
                (input.State == InputState.JustPressed && isJustPressed)
            )
            {
                command.Execute();
                OnInputDetected?.Invoke(InputSchema.Keyboard);
            }

            _previousStates[input.Button] = currentState;
        }
    }
    
    public event Action<InputSchema> OnInputDetected;
}