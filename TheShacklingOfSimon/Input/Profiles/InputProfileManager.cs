using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;

namespace TheShacklingOfSimon.Input.Profiles;

public static class InputProfileManager
{
    private static readonly string _path = "controls_config.json";

    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void SaveProfile(InputProfile profile)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(profile, _options);
            File.WriteAllText(_path, jsonString);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to save profile: " + e.Message);
        }
    }

    public static InputProfile LoadProfile()
    {
        if (!File.Exists(_path))
        {
            return GenerateDefaultProfile();
        }

        try
        {
            string jsonString = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<InputProfile>(jsonString, _options);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to load profile: " + e.Message);
            Console.WriteLine("Loading the default profile instead.");
            return GenerateDefaultProfile();
        }
    }

    private static InputProfile GenerateDefaultProfile()
    {
        InputProfile profile = new InputProfile()
        {
            KeyboardMap = new(),
            GamepadButtonMap = new(),
            GamepadJoystickMap = new(),
            MouseMap = new()
        };

        // Keyboard movement
        profile.KeyboardMap.Add(PlayerAction.MoveUp, new KeyboardInput(InputState.Pressed, KeyboardButton.W));
        profile.KeyboardMap.Add(PlayerAction.MoveLeft, new KeyboardInput(InputState.Pressed, KeyboardButton.A));
        profile.KeyboardMap.Add(PlayerAction.MoveDown, new KeyboardInput(InputState.Pressed, KeyboardButton.S));
        profile.KeyboardMap.Add(PlayerAction.MoveRight, new KeyboardInput(InputState.Pressed, KeyboardButton.D));

        // Keyboard attacking
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackUp, new KeyboardInput(InputState.Pressed, KeyboardButton.Up));
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackLeft,
            new KeyboardInput(InputState.Pressed, KeyboardButton.Left));
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackDown,
            new KeyboardInput(InputState.Pressed, KeyboardButton.Down));
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackRight,
            new KeyboardInput(InputState.Pressed, KeyboardButton.Right));
        profile.KeyboardMap.Add(PlayerAction.SecondaryAttackDown,
            new KeyboardInput(InputState.Pressed, KeyboardButton.LeftShift));

        // keyboard inventory and weapons
        profile.KeyboardMap.Add(PlayerAction.NextPrimaryWeapon,
            new KeyboardInput(InputState.JustPressed, KeyboardButton.J));
        profile.KeyboardMap.Add(PlayerAction.NextSecondaryWeapon,
            new KeyboardInput(InputState.JustPressed, KeyboardButton.K));
        profile.KeyboardMap.Add(PlayerAction.NextActiveItem,
            new KeyboardInput(InputState.JustPressed, KeyboardButton.I));
        profile.KeyboardMap.Add(PlayerAction.PreviousActiveItem,
            new KeyboardInput(InputState.JustPressed, KeyboardButton.U));
        profile.KeyboardMap.Add(PlayerAction.UseActiveItem,
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Space));

        // Keyboard miscellaneous
        profile.KeyboardMap.Add(PlayerAction.Reset, new KeyboardInput(InputState.JustPressed, KeyboardButton.R));
        profile.KeyboardMap.Add(PlayerAction.Pause, new KeyboardInput(InputState.JustPressed, KeyboardButton.Escape));
        profile.KeyboardMap.Add(PlayerAction.Quit, new KeyboardInput(InputState.JustPressed, KeyboardButton.Q));
        profile.KeyboardMap.Add(PlayerAction.Resume, new KeyboardInput(InputState.JustPressed, KeyboardButton.Escape));
        
        // Gamepad movement
        profile.GamepadJoystickMap.Add(PlayerAction.MoveUp,
            new GamepadJoystickInput(GamepadStick.Left, new JoystickInputRegion(new Vector2(0, 1), 120f, 0.1f),
                InputState.Pressed));
        profile.GamepadJoystickMap.Add(PlayerAction.MoveLeft,
            new GamepadJoystickInput(GamepadStick.Left, new JoystickInputRegion(new Vector2(-1, 0), 120f, 0.1f),
                InputState.Pressed));
        profile.GamepadJoystickMap.Add(PlayerAction.MoveRight,
            new GamepadJoystickInput(GamepadStick.Left, new JoystickInputRegion(new Vector2(1, 0), 120f, 0.1f),
                InputState.Pressed));
        profile.GamepadJoystickMap.Add(PlayerAction.MoveDown,
            new GamepadJoystickInput(GamepadStick.Left, new JoystickInputRegion(new Vector2(0, -1), 120f, 0.1f),
                InputState.Pressed));

        // Gamepad attacking
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackUp,
            new GamepadJoystickInput(GamepadStick.Right, new JoystickInputRegion(new Vector2(0, 1), 90f, 0.1f),
                InputState.Pressed));
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackDown,
            new GamepadJoystickInput(GamepadStick.Right, new JoystickInputRegion(new Vector2(0, -1), 90f, 0.1f),
                InputState.Pressed));
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackLeft,
            new GamepadJoystickInput(GamepadStick.Right, new JoystickInputRegion(new Vector2(-1, 0), 90f, 0.1f),
                InputState.Pressed));
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackRight,
            new GamepadJoystickInput(GamepadStick.Right, new JoystickInputRegion(new Vector2(1, 0), 90f, 0.1f),
                InputState.Pressed));
        profile.GamepadButtonMap.Add(PlayerAction.SecondaryAttackDown,
            new GamepadButtonInput(InputState.Pressed, GamepadButton.B));

        // gamepad inventory & buttons
        profile.GamepadButtonMap.Add(PlayerAction.UseActiveItem,
            new GamepadButtonInput(InputState.Pressed, GamepadButton.RightTrigger));
        profile.GamepadButtonMap.Add(PlayerAction.NextActiveItem,
            new GamepadButtonInput(InputState.Pressed, GamepadButton.DPadUp));
        profile.GamepadButtonMap.Add(PlayerAction.PreviousActiveItem,
            new GamepadButtonInput(InputState.Pressed, GamepadButton.DPadDown));
        profile.GamepadButtonMap.Add(PlayerAction.NextSecondaryWeapon,
            new GamepadButtonInput(InputState.Pressed, GamepadButton.DPadRight));
        profile.GamepadButtonMap.Add(PlayerAction.NextPrimaryWeapon,
            new GamepadButtonInput(InputState.Pressed, GamepadButton.DPadLeft));

        // gamepad miscellaneous
        profile.GamepadButtonMap.Add(PlayerAction.Pause,
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.Start));
        profile.GamepadButtonMap.Add(PlayerAction.Reset,
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.Back));
        profile.GamepadButtonMap.Add(PlayerAction.Quit,
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.X));
        profile.GamepadButtonMap.Add(PlayerAction.Resume,
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.A));

        return profile;
    }
}