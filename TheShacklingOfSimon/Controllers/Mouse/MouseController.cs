#region

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

#endregion

namespace TheShacklingOfSimon.Controllers.Mouse;

public class MouseController : IMouseController
{
    private readonly IMouseService _mouseService;
    private readonly Dictionary<MouseInput, ICommand> _map;

    // State logic
    private HashSet<MouseButton> _prevPressedButtons;
    private HashSet<MouseButton> _currentPressedButtons;
    private Vector2 _prevMousePos;
    private Vector2 _currentMousePos;

    public event Action<InputSchema> OnInputDetected;
        
    public MouseController(IMouseService service)
    {
        _mouseService = service;
        _map = new Dictionary<MouseInput, ICommand>();
        _prevPressedButtons = new HashSet<MouseButton>();
        _currentPressedButtons = new HashSet<MouseButton>();
    }

    public void RegisterCommand(MouseInput input, ICommand cmd)
    {
        _map.TryAdd(input, cmd);
    }

    public void UnregisterCommand(MouseInput input)
    {
        _map.Remove(input);
    }

    public void ClearCommands()
    {
        _map.Clear();
    }

    public void Update()
    {
        _prevPressedButtons = _currentPressedButtons;
        _currentPressedButtons = new HashSet<MouseButton>(_mouseService.GetPressedButtons());
        _prevMousePos = _currentMousePos;
        _currentMousePos = _mouseService.GetPosition();

        // Do _map.ToList() to prevent the _map being modified during iteration (from the command execution)
        foreach (var pair in _map.ToList())
        {
            MouseInput input = pair.Key;
            ICommand command = pair.Value;

            if (GetButtonState(input.Button) == input.State && 
                input.Region.ContainsPoint(_currentMousePos.X, _currentMousePos.Y)
                )
            {
                command.Execute();
                OnInputDetected?.Invoke(InputSchema.Mouse);
            }
        }
    }
    
    public IEnumerable<MouseButton> GetPressedButtons()
    {
        return _currentPressedButtons;
    }
    
    public IEnumerable<MouseButton> GetJustPressedButtons()
    {
        return _currentPressedButtons.Except(_prevPressedButtons);
    }
    
    public InputState GetButtonState(MouseButton button)
    {
        bool isDownNow = _currentPressedButtons.Contains(button);
        bool wasDown = _prevPressedButtons.Contains(button);

        return DetermineState(isDownNow, wasDown);
    }

    public InputState GetCursorState(MouseInput input)
    {
        bool isInRegionNow = input.Region.ContainsPoint(_currentMousePos.X, _currentMousePos.Y);
        bool wasInRegion = input.Region.ContainsPoint(_prevMousePos.X, _prevMousePos.Y);
        
        return DetermineState(isInRegionNow, wasInRegion);       
    }

    public Vector2 GetPosition()
    {
        return _currentMousePos;
    }
    
    private InputState DetermineState(bool prevState, bool currentState)
    {
        if (currentState && !prevState) return InputState.JustPressed;
        else if (currentState && prevState) return InputState.Pressed;
        else if (!currentState && prevState) return InputState.JustReleased;
        else return InputState.Released;
    }
}