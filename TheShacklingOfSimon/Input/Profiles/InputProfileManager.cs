#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;

#endregion

namespace TheShacklingOfSimon.Input.Profiles;

public static class InputProfileManager
{
    private static readonly string BaseDir = AppContext.BaseDirectory;
    private static readonly string JsonPath = Path.Combine(BaseDir, SanitizeFilePath("Content/controls_config.json"));
    
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void SaveProfile(InputProfile profile)
    {
        try
        {
            string directory = Path.GetDirectoryName(JsonPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            string jsonString = JsonSerializer.Serialize(profile, Options);
            File.WriteAllText(JsonPath, jsonString);
            Console.WriteLine("Profile saved successfully to " + JsonPath);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to save profile: " + e.Message);
        }
    }

    public static InputProfile LoadProfile()
    {
        if (!File.Exists(JsonPath))
        {
            Console.WriteLine("Profile file not found. Loading the default profile instead.");
            return GenerateDefaultProfile();
        }

        try
        {
            string jsonString = File.ReadAllText(JsonPath);
            return JsonSerializer.Deserialize<InputProfile>(jsonString, Options);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to load profile: " + e.Message);
            Console.WriteLine("Loading the default profile instead.");
            return GenerateDefaultProfile();
        }
    }
    
    public static InputProfile GenerateDefaultProfile()
    {
        InputProfile profile = new InputProfile()
        {
            KeyboardMap = new(),
            GamepadButtonMap = new(),
            GamepadJoystickMap = new(),
            MouseMap = new()
        };

        // Keyboard movement
        profile.KeyboardMap.Add(PlayerAction.MoveUp, [new(KeyboardButton.W, InputState.Pressed)]);
        profile.KeyboardMap.Add(PlayerAction.MoveLeft, [new(KeyboardButton.A, InputState.Pressed)]);
        profile.KeyboardMap.Add(PlayerAction.MoveDown, [new(KeyboardButton.S, InputState.Pressed)]);
        profile.KeyboardMap.Add(PlayerAction.MoveRight, [new(KeyboardButton.D, InputState.Pressed)]);

        // Keyboard attacking
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackUp, [new(KeyboardButton.Up, InputState.Pressed)]);
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackLeft, [new(KeyboardButton.Left, InputState.Pressed)]);
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackDown, [new(KeyboardButton.Down, InputState.Pressed)]);
        profile.KeyboardMap.Add(PlayerAction.PrimaryAttackRight, [new(KeyboardButton.Right, InputState.Pressed)]);
        profile.KeyboardMap.Add(PlayerAction.SecondaryAttackDown, [
            new(KeyboardButton.LeftShift, InputState.Pressed),
            new(KeyboardButton.RightShift, InputState.Pressed),
            new(KeyboardButton.E, InputState.Pressed)
        ]);

        // Keyboard inventory and weapons
        profile.KeyboardMap.Add(PlayerAction.NextPrimaryWeapon, [new(KeyboardButton.R, InputState.JustPressed)]);
        profile.KeyboardMap.Add(PlayerAction.NextSecondaryWeapon, [new(KeyboardButton.Q, InputState.JustPressed)]);
        profile.KeyboardMap.Add(PlayerAction.NextActiveItem, [new(KeyboardButton.F, InputState.JustPressed)]);
        profile.KeyboardMap.Add(PlayerAction.UseActiveItem, [new(KeyboardButton.Space, InputState.JustPressed)]);

        // Keyboard miscellaneous
        profile.KeyboardMap.Add(PlayerAction.Reset, [new(KeyboardButton.R, InputState.JustPressed)]);
        profile.KeyboardMap.Add(PlayerAction.Pause, [new(KeyboardButton.Escape, InputState.JustPressed)]);
        profile.KeyboardMap.Add(PlayerAction.Quit, [new(KeyboardButton.Q, InputState.JustPressed)]);
        profile.KeyboardMap.Add(PlayerAction.Resume, [new(KeyboardButton.Escape, InputState.JustPressed)]);
        
        // Keyboard menu navigation
        profile.KeyboardMap.Add(PlayerAction.MenuUp, [
            new(KeyboardButton.Up, InputState.Pressed),
            new(KeyboardButton.W, InputState.Pressed)
        ]);
        profile.KeyboardMap.Add(PlayerAction.MenuDown, [
            new(KeyboardButton.Down, InputState.Pressed),
            new(KeyboardButton.S, InputState.Pressed)
        ]);
        profile.KeyboardMap.Add(PlayerAction.MenuLeft, [
            new(KeyboardButton.Left, InputState.Pressed),
            new(KeyboardButton.A, InputState.Pressed)
        ]);
        profile.KeyboardMap.Add(PlayerAction.MenuRight, [
            new(KeyboardButton.Right, InputState.Pressed),
            new(KeyboardButton.D, InputState.Pressed)
        ]);
        profile.KeyboardMap.Add(PlayerAction.MenuConfirm, [
            new(KeyboardButton.Enter, InputState.JustPressed),
            new(KeyboardButton.Space, InputState.JustPressed)
        ]);
        profile.KeyboardMap.Add(PlayerAction.MenuCancel, [
            new(KeyboardButton.Escape, InputState.JustPressed),
            new(KeyboardButton.Back, InputState.JustPressed)
        ]);
        
        // Mouse menu navigation
        profile.MouseMap.Add(PlayerAction.MenuConfirm, [
            new(new MouseInputRegion(0, 0, 1024, 768), MouseButton.Left, InputState.JustPressed)
        ]);
        
        // Gamepad movement
        profile.GamepadJoystickMap.Add(PlayerAction.MoveUp, [ 
            new(GamepadStick.Left, new JoystickInputRegion(new Vector2(0, 1), 150f, 0.1f), InputState.Pressed) 
        ]);
        profile.GamepadJoystickMap.Add(PlayerAction.MoveLeft, [ 
            new(GamepadStick.Left, new JoystickInputRegion(new Vector2(-1, 0), 150f, 0.1f), InputState.Pressed) 
        ]);
        profile.GamepadJoystickMap.Add(PlayerAction.MoveRight, [ 
            new(GamepadStick.Left, new JoystickInputRegion(new Vector2(1, 0), 150f, 0.1f), InputState.Pressed) 
        ]);
        profile.GamepadJoystickMap.Add(PlayerAction.MoveDown, [ 
            new(GamepadStick.Left, new JoystickInputRegion(new Vector2(0, -1), 150f, 0.1f), InputState.Pressed) 
        ]);

        // Gamepad attacking
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackUp, [ 
            new(GamepadStick.Right, new JoystickInputRegion(new Vector2(0, 1), 90f, 0.1f), InputState.Pressed) 
        ]);
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackDown, [ 
            new(GamepadStick.Right, new JoystickInputRegion(new Vector2(0, -1), 90f, 0.1f), InputState.Pressed) 
        ]);
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackLeft, [ 
            new(GamepadStick.Right, new JoystickInputRegion(new Vector2(-1, 0), 90f, 0.1f), InputState.Pressed) 
        ]);
        profile.GamepadJoystickMap.Add(PlayerAction.PrimaryAttackRight, [ 
            new(GamepadStick.Right, new JoystickInputRegion(new Vector2(1, 0), 90f, 0.1f), InputState.Pressed) 
        ]);
        
        // Gamepad inventory & buttons
        profile.GamepadButtonMap.Add(PlayerAction.SecondaryAttackDown, [
            new(GamepadButton.B, InputState.Pressed),
            new(GamepadButton.LeftShoulder, InputState.Pressed),
            new(GamepadButton.LeftTrigger, InputState.Pressed),
        ]);
        profile.GamepadButtonMap.Add(PlayerAction.UseActiveItem, [
            new(GamepadButton.A, InputState.Pressed),
            new(GamepadButton.RightTrigger, InputState.Pressed),
            new(GamepadButton.RightShoulder, InputState.Pressed),
        ]);
        profile.GamepadButtonMap.Add(PlayerAction.NextActiveItem, [new(GamepadButton.DPadUp, InputState.JustPressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.PreviousActiveItem, [new(GamepadButton.DPadDown, InputState.JustPressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.NextSecondaryWeapon, [new(GamepadButton.DPadRight, InputState.JustPressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.NextPrimaryWeapon, [new(GamepadButton.DPadLeft, InputState.JustPressed)]);

        // Gamepad miscellaneous
        profile.GamepadButtonMap.Add(PlayerAction.Pause, [new(GamepadButton.Start, InputState.JustPressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.Reset, [new(GamepadButton.Back, InputState.JustPressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.Quit, [new(GamepadButton.X, InputState.JustPressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.Resume, [new(GamepadButton.A, InputState.JustPressed)]);
        
        // Gamepad menu navigation
        profile.GamepadButtonMap.Add(PlayerAction.MenuUp, [new(GamepadButton.DPadUp, InputState.Pressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.MenuDown, [new(GamepadButton.DPadDown, InputState.Pressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.MenuLeft, [new(GamepadButton.DPadLeft, InputState.Pressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.MenuRight, [new(GamepadButton.DPadRight, InputState.Pressed)]);
        profile.GamepadJoystickMap.Add(PlayerAction.MenuUp, [new(GamepadStick.Left, new JoystickInputRegion(new Vector2(0, 1), 90f, 0.5f), InputState.Pressed)]);
        profile.GamepadJoystickMap.Add(PlayerAction.MenuDown, [new(GamepadStick.Left, new JoystickInputRegion(new Vector2(0, -1), 90f, 0.5f), InputState.Pressed)]);
        profile.GamepadJoystickMap.Add(PlayerAction.MenuLeft, [new(GamepadStick.Left, new JoystickInputRegion(new Vector2(-1, 0), 90f, 0.5f), InputState.Pressed)]);
        profile.GamepadJoystickMap.Add(PlayerAction.MenuRight, [new(GamepadStick.Left, new JoystickInputRegion(new Vector2(1, 0), 90f, 0.5f), InputState.Pressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.MenuConfirm, [new(GamepadButton.A, InputState.JustPressed)]);
        profile.GamepadButtonMap.Add(PlayerAction.MenuCancel, [new(GamepadButton.B, InputState.JustPressed)]);
        
        return profile;
    }
    
    private static string SanitizeFilePath(string filePath)
    {
        // Replace all slashes from input with the correct OS-specific separators
        return filePath
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
    }
}