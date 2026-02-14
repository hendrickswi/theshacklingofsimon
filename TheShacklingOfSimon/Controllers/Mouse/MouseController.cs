using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Mouse;

public class MouseController : IController<MouseInput>
{
    /*
     * Any dependencies are in a custom service class.
     */
    private IMouseService _mouseService;
    private Dictionary<MouseButton, BinaryInputState> _prevStates;
    
    private Dictionary<MouseInput, Commands.ICommand> _map;
    public MouseController(IMouseService service)
    {
        _mouseService = service;
        _prevStates = new Dictionary<MouseButton, BinaryInputState>();
        foreach (MouseButton btn in System.Enum.GetValues(typeof(MouseButton)))
        {
            _prevStates.Add(btn, BinaryInputState.Released);
        }
        
        _map = new Dictionary<MouseInput, Commands.ICommand>();
    }

    public void RegisterCommand(MouseInput input, Commands.ICommand cmd)
    {
        // Avoid an ArgumentException if k is already in the map.
        _map.TryAdd(input, cmd);
    }
    public void Update()
    {
        XYPoint pos = _mouseService.GetPosition();

        foreach (var entry in _map)
        {
            MouseInput inputDefinition = entry.Key;
            if (inputDefinition.Region.ContainsPoint(pos.X, pos.Y))
            {
                BinaryInputState currentState = _mouseService.GetButtonState(inputDefinition.Button);
                BinaryInputState previousState = _prevStates[inputDefinition.Button];
                
                /*
                 * Execute only if it's a "fresh" press
                 * AND if it is the correct, required state.
                 */
                if (currentState == entry.Key.State &&
                    previousState != currentState)
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