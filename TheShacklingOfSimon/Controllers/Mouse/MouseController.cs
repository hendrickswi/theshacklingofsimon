using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Mouse;

public class MouseController : IController<MouseInput>
{
    /*
     * Any dependencies are in a custom service class.
     */
    private readonly IMouseService _mouseService;
    private readonly Dictionary<MouseInput, Commands.ICommand> _map;
    private readonly Dictionary<MouseButton, InputState> _prevStates;
    
    public MouseController(IMouseService service)
    {
        _mouseService = service;
        _prevStates = new Dictionary<MouseButton, InputState>();
        foreach (MouseButton btn in System.Enum.GetValues(typeof(MouseButton)))
        {
            _prevStates.Add(btn, InputState.Released);
        }
        
        _map = new Dictionary<MouseInput, Commands.ICommand>();
    }

    public void RegisterCommand(MouseInput input, Commands.ICommand cmd)
    {
        bool success = _map.TryAdd(input, cmd);
        if (success)
        {
           _prevStates.Add(input.Button, InputState.Released);
        }
    }

    public void UnregisterCommand(MouseInput input)
    {
        bool success = _map.Remove(input);
        if (success)
        {
            _prevStates.Remove(input.Button);
        }
    }

    public void ClearCommands()
    {
        _map.Clear();
    }
    
    public void Update()
    {
        Vector2 pos = _mouseService.GetPosition();

        foreach (var entry in _map)
        {
            MouseInput inputDefinition = entry.Key;
            if (inputDefinition.Region.ContainsPoint(pos.X, pos.Y))
            {
                InputState currentState = _mouseService.GetButtonState(inputDefinition.Button);
                InputState previousState = _prevStates[inputDefinition.Button];
                
                bool isJustPressed =
                    currentState == InputState.Pressed &&
                    previousState == InputState.Released;
                
                if(
                    inputDefinition.State == InputState.Pressed && currentState == InputState.Pressed ||
                    inputDefinition.State == InputState.Released && currentState == InputState.Released ||
                    inputDefinition.State == InputState.JustPressed && isJustPressed
                    )
                {
                        entry.Value.Execute();
                }
            }
        }

        // Update the previous states for the next Update() call
        foreach (MouseButton btn in System.Enum.GetValues(typeof(MouseButton)))
        {
            _prevStates[btn] = _mouseService.GetButtonState(btn);
        }
    }
}