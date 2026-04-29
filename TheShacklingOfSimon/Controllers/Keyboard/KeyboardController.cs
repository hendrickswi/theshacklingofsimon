#region

using System;
using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

#endregion

namespace TheShacklingOfSimon.Controllers.Keyboard;

public class KeyboardController : IKeyboardController
{
    private readonly IKeyboardService _keyboardService;
    private readonly Dictionary<KeyboardInput, ICommand> _map;
    
    // State logic
    private HashSet<KeyboardButton> _prevPressedKeys;
    private HashSet<KeyboardButton> _currentPressedKeys;
    
    public event Action<InputSchema> OnInputDetected;

    public KeyboardController(IKeyboardService service)
    {
        _keyboardService = service;
        _map = new Dictionary<KeyboardInput, ICommand>();
        _prevPressedKeys = new HashSet<KeyboardButton>();
        _currentPressedKeys = new HashSet<KeyboardButton>();
    }

    public void RegisterCommand(KeyboardInput input, ICommand command)
    {
        _map.TryAdd(input, command);
    }

    public void UnregisterCommand(KeyboardInput input)
    {
        _map.Remove(input);
    }

    public void ClearCommands()
    {
        _map.Clear();
    }

    public void Update()
    {
        _prevPressedKeys = _currentPressedKeys;
        _currentPressedKeys = new HashSet<KeyboardButton>(_keyboardService.GetPressedKeys());

        // Do _map.ToList() to prevent the _map being modified during iteration (from the command execution)
        foreach (var pair in _map.ToList())
        {
            KeyboardInput input = pair.Key;
            ICommand command = pair.Value;

            if (GetKeyState(input.Button) == input.State)
            {
                command.Execute();
                OnInputDetected?.Invoke(InputSchema.Keyboard);
            }
        }
    }

    public IEnumerable<KeyboardButton> GetPressedKeys()
    {
        return _currentPressedKeys;
    }

    public IEnumerable<KeyboardButton> GetJustPressedKeys()
    {
        return _currentPressedKeys.Except(_prevPressedKeys);
    }

    public InputState GetKeyState(KeyboardButton key)
    {
        bool isDownNow = _currentPressedKeys.Contains(key);
        bool wasDown = _prevPressedKeys.Contains(key);

        if (isDownNow && !wasDown) return InputState.JustPressed;
        else if (isDownNow && wasDown) return InputState.Pressed;
        else if (!isDownNow && wasDown) return InputState.JustReleased;
        else return InputState.Released;
    }
}