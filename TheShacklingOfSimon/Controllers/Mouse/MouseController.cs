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

public class MouseController : IController<MouseInput>
{
    private readonly IMouseService _mouseService;
    private readonly Dictionary<MouseInput, ICommand> _map;
    private readonly Dictionary<MouseButton, InputState> _prevStates;

    public MouseController(IMouseService service)
    {
        _mouseService = service;
        _prevStates = new Dictionary<MouseButton, InputState>();
        foreach (MouseButton btn in Enum.GetValues(typeof(MouseButton)))
        {
            _prevStates.Add(btn, InputState.Released);
        }

        _map = new Dictionary<MouseInput, ICommand>();
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
        Vector2 pos = _mouseService.GetPosition();

        // we use a snapshot here for the same reason as keyboard. (safely change binds)
        KeyValuePair<MouseInput, ICommand>[] bindings = _map.ToArray();

        foreach (KeyValuePair<MouseInput, ICommand> entry in bindings)
        {
            MouseInput inputDefinition = entry.Key;
            ICommand command = entry.Value;

            if (!inputDefinition.Region.ContainsPoint(pos.X, pos.Y))
            {
                continue;
            }

            InputState currentState = _mouseService.GetButtonState(inputDefinition.Button);
            InputState previousState = _prevStates[inputDefinition.Button];

            bool isJustPressed =
                currentState == InputState.Pressed &&
                previousState == InputState.Released;

            if (
                (inputDefinition.State == InputState.Pressed && currentState == InputState.Pressed) ||
                (inputDefinition.State == InputState.Released && currentState == InputState.Released) ||
                (inputDefinition.State == InputState.JustPressed && isJustPressed)
            )
            {
                command.Execute();
                OnInputDetected?.Invoke(InputSchema.Mouse);
            }
        }

        // Update the previous states for the next Update() call
        foreach (MouseButton btn in Enum.GetValues(typeof(MouseButton)))
        {
            _prevStates[btn] = _mouseService.GetButtonState(btn);
        }
    }
    
    public event Action<InputSchema> OnInputDetected;
}