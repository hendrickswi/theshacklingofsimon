#region

using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

#endregion

namespace TheShacklingOfSimon.Controllers.Keyboard;

public class MonoGameKeyboardService : IKeyboardService
{
    private static readonly Dictionary<KeyboardButton, Keys> _keyMap = new Dictionary<KeyboardButton, Keys>
    {
        { KeyboardButton.None, Keys.None },
        { KeyboardButton.Back, Keys.Back },
        { KeyboardButton.Tab, Keys.Tab },
        { KeyboardButton.Enter, Keys.Enter },
        { KeyboardButton.Pause, Keys.Pause },
        { KeyboardButton.CapsLock, Keys.CapsLock },
        { KeyboardButton.Escape, Keys.Escape },
        { KeyboardButton.Space, Keys.Space },
        { KeyboardButton.PageUp, Keys.PageUp },
        { KeyboardButton.PageDown, Keys.PageDown },
        { KeyboardButton.End, Keys.End },
        { KeyboardButton.Home, Keys.Home },
        { KeyboardButton.Left, Keys.Left },
        { KeyboardButton.Up, Keys.Up },
        { KeyboardButton.Right, Keys.Right },
        { KeyboardButton.Down, Keys.Down },
        { KeyboardButton.Select, Keys.Select },
        { KeyboardButton.Print, Keys.Print },
        { KeyboardButton.Execute, Keys.Execute },
        { KeyboardButton.PrintScreen, Keys.PrintScreen },
        { KeyboardButton.Insert, Keys.Insert },
        { KeyboardButton.Delete, Keys.Delete },
        { KeyboardButton.Help, Keys.Help },

        // Digits
        { KeyboardButton.D0, Keys.D0 }, { KeyboardButton.D1, Keys.D1 }, { KeyboardButton.D2, Keys.D2 },
        { KeyboardButton.D3, Keys.D3 }, { KeyboardButton.D4, Keys.D4 }, { KeyboardButton.D5, Keys.D5 },
        { KeyboardButton.D6, Keys.D6 }, { KeyboardButton.D7, Keys.D7 }, { KeyboardButton.D8, Keys.D8 },
        { KeyboardButton.D9, Keys.D9 },

        // Letters
        { KeyboardButton.A, Keys.A }, { KeyboardButton.B, Keys.B }, { KeyboardButton.C, Keys.C },
        { KeyboardButton.D, Keys.D }, { KeyboardButton.E, Keys.E }, { KeyboardButton.F, Keys.F },
        { KeyboardButton.G, Keys.G }, { KeyboardButton.H, Keys.H }, { KeyboardButton.I, Keys.I },
        { KeyboardButton.J, Keys.J }, { KeyboardButton.K, Keys.K }, { KeyboardButton.L, Keys.L },
        { KeyboardButton.M, Keys.M }, { KeyboardButton.N, Keys.N }, { KeyboardButton.O, Keys.O },
        { KeyboardButton.P, Keys.P }, { KeyboardButton.Q, Keys.Q }, { KeyboardButton.R, Keys.R },
        { KeyboardButton.S, Keys.S }, { KeyboardButton.T, Keys.T }, { KeyboardButton.U, Keys.U },
        { KeyboardButton.V, Keys.V }, { KeyboardButton.W, Keys.W }, { KeyboardButton.X, Keys.X },
        { KeyboardButton.Y, Keys.Y }, { KeyboardButton.Z, Keys.Z },

        // Windows / Apps
        { KeyboardButton.LeftWindows, Keys.LeftWindows },
        { KeyboardButton.RightWindows, Keys.RightWindows },
        { KeyboardButton.Apps, Keys.Apps },
        { KeyboardButton.Sleep, Keys.Sleep },

        // Numpad
        { KeyboardButton.NumPad0, Keys.NumPad0 }, { KeyboardButton.NumPad1, Keys.NumPad1 },
        { KeyboardButton.NumPad2, Keys.NumPad2 }, { KeyboardButton.NumPad3, Keys.NumPad3 },
        { KeyboardButton.NumPad4, Keys.NumPad4 }, { KeyboardButton.NumPad5, Keys.NumPad5 },
        { KeyboardButton.NumPad6, Keys.NumPad6 }, { KeyboardButton.NumPad7, Keys.NumPad7 },
        { KeyboardButton.NumPad8, Keys.NumPad8 }, { KeyboardButton.NumPad9, Keys.NumPad9 },
        { KeyboardButton.Multiply, Keys.Multiply },
        { KeyboardButton.Add, Keys.Add },
        { KeyboardButton.Separator, Keys.Separator },
        { KeyboardButton.Subtract, Keys.Subtract },
        { KeyboardButton.Decimal, Keys.Decimal },
        { KeyboardButton.Divide, Keys.Divide },

        // Function Keys
        { KeyboardButton.F1, Keys.F1 }, { KeyboardButton.F2, Keys.F2 }, { KeyboardButton.F3, Keys.F3 },
        { KeyboardButton.F4, Keys.F4 }, { KeyboardButton.F5, Keys.F5 }, { KeyboardButton.F6, Keys.F6 },
        { KeyboardButton.F7, Keys.F7 }, { KeyboardButton.F8, Keys.F8 }, { KeyboardButton.F9, Keys.F9 },
        { KeyboardButton.F10, Keys.F10 }, { KeyboardButton.F11, Keys.F11 }, { KeyboardButton.F12, Keys.F12 },
        { KeyboardButton.F13, Keys.F13 }, { KeyboardButton.F14, Keys.F14 }, { KeyboardButton.F15, Keys.F15 },
        { KeyboardButton.F16, Keys.F16 }, { KeyboardButton.F17, Keys.F17 }, { KeyboardButton.F18, Keys.F18 },
        { KeyboardButton.F19, Keys.F19 }, { KeyboardButton.F20, Keys.F20 }, { KeyboardButton.F21, Keys.F21 },
        { KeyboardButton.F22, Keys.F22 }, { KeyboardButton.F23, Keys.F23 }, { KeyboardButton.F24, Keys.F24 },

        // Locks
        { KeyboardButton.NumLock, Keys.NumLock },
        { KeyboardButton.Scroll, Keys.Scroll },

        // Modifiers
        { KeyboardButton.LeftShift, Keys.LeftShift },
        { KeyboardButton.RightShift, Keys.RightShift },
        { KeyboardButton.LeftControl, Keys.LeftControl },
        { KeyboardButton.RightControl, Keys.RightControl },
        { KeyboardButton.LeftAlt, Keys.LeftAlt },
        { KeyboardButton.RightAlt, Keys.RightAlt },

        // Browser
        { KeyboardButton.BrowserBack, Keys.BrowserBack },
        { KeyboardButton.BrowserForward, Keys.BrowserForward },
        { KeyboardButton.BrowserRefresh, Keys.BrowserRefresh },
        { KeyboardButton.BrowserStop, Keys.BrowserStop },
        { KeyboardButton.BrowserSearch, Keys.BrowserSearch },
        { KeyboardButton.BrowserFavorites, Keys.BrowserFavorites },
        { KeyboardButton.BrowserHome, Keys.BrowserHome },

        // Volume / Media
        { KeyboardButton.VolumeMute, Keys.VolumeMute },
        { KeyboardButton.VolumeDown, Keys.VolumeDown },
        { KeyboardButton.VolumeUp, Keys.VolumeUp },
        { KeyboardButton.MediaNextTrack, Keys.MediaNextTrack },
        { KeyboardButton.MediaPreviousTrack, Keys.MediaPreviousTrack },
        { KeyboardButton.MediaStop, Keys.MediaStop },
        { KeyboardButton.MediaPlayPause, Keys.MediaPlayPause },

        // Launchers
        { KeyboardButton.LaunchMail, Keys.LaunchMail },
        { KeyboardButton.LaunchApplication1, Keys.LaunchApplication1 },
        { KeyboardButton.LaunchApplication2, Keys.LaunchApplication2 },

        // OEM Keys
        { KeyboardButton.OemSemicolon, Keys.OemSemicolon },
        { KeyboardButton.OemPlus, Keys.OemPlus },
        { KeyboardButton.OemComma, Keys.OemComma },
        { KeyboardButton.OemMinus, Keys.OemMinus },
        { KeyboardButton.OemPeriod, Keys.OemPeriod },
        { KeyboardButton.OemQuestion, Keys.OemQuestion },
        { KeyboardButton.OemTilde, Keys.OemTilde },
        { KeyboardButton.OemOpenBrackets, Keys.OemOpenBrackets },
        { KeyboardButton.OemPipe, Keys.OemPipe },
        { KeyboardButton.OemCloseBrackets, Keys.OemCloseBrackets },
        { KeyboardButton.OemQuotes, Keys.OemQuotes },
        { KeyboardButton.Oem8, Keys.Oem8 },
        { KeyboardButton.OemBackslash, Keys.OemBackslash },
        { KeyboardButton.OemClear, Keys.OemClear },

        // ChatPad
        { KeyboardButton.ChatPadGreen, Keys.ChatPadGreen },
        { KeyboardButton.ChatPadOrange, Keys.ChatPadOrange },

        // Special
        { KeyboardButton.ProcessKey, Keys.ProcessKey },
        { KeyboardButton.Attn, Keys.Attn },
        { KeyboardButton.Crsel, Keys.Crsel },
        { KeyboardButton.Exsel, Keys.Exsel },
        { KeyboardButton.EraseEof, Keys.EraseEof },
        { KeyboardButton.Play, Keys.Play },
        { KeyboardButton.Zoom, Keys.Zoom },
        { KeyboardButton.Pa1, Keys.Pa1 }
    };
    
    private static readonly Dictionary<Keys, KeyboardButton> _reverseKeyMap = new Dictionary<Keys, KeyboardButton>
    {
        { Keys.None, KeyboardButton.None },
        { Keys.Back, KeyboardButton.Back },
        { Keys.Tab, KeyboardButton.Tab },
        { Keys.Enter, KeyboardButton.Enter },
        { Keys.Pause, KeyboardButton.Pause },
        { Keys.CapsLock, KeyboardButton.CapsLock },
        { Keys.Escape, KeyboardButton.Escape },
        { Keys.Space, KeyboardButton.Space },
        { Keys.PageUp, KeyboardButton.PageUp },
        { Keys.PageDown, KeyboardButton.PageDown },
        { Keys.End, KeyboardButton.End },
        { Keys.Home, KeyboardButton.Home },
        { Keys.Left, KeyboardButton.Left },
        { Keys.Up, KeyboardButton.Up },
        { Keys.Right, KeyboardButton.Right },
        { Keys.Down, KeyboardButton.Down },
        { Keys.Select, KeyboardButton.Select },
        { Keys.Print, KeyboardButton.Print },
        { Keys.Execute, KeyboardButton.Execute },
        { Keys.PrintScreen, KeyboardButton.PrintScreen },
        { Keys.Insert, KeyboardButton.Insert },
        { Keys.Delete, KeyboardButton.Delete },
        { Keys.Help, KeyboardButton.Help },

        // Digits
        { Keys.D0, KeyboardButton.D0 }, { Keys.D1, KeyboardButton.D1 }, { Keys.D2, KeyboardButton.D2 },
        { Keys.D3, KeyboardButton.D3 }, { Keys.D4, KeyboardButton.D4 }, { Keys.D5, KeyboardButton.D5 },
        { Keys.D6, KeyboardButton.D6 }, { Keys.D7, KeyboardButton.D7 }, { Keys.D8, KeyboardButton.D8 },
        { Keys.D9, KeyboardButton.D9 },

        // Letters
        { Keys.A, KeyboardButton.A }, { Keys.B, KeyboardButton.B }, { Keys.C, KeyboardButton.C },
        { Keys.D, KeyboardButton.D }, { Keys.E, KeyboardButton.E }, { Keys.F, KeyboardButton.F },
        { Keys.G, KeyboardButton.G }, { Keys.H, KeyboardButton.H }, { Keys.I, KeyboardButton.I },
        { Keys.J, KeyboardButton.J }, { Keys.K, KeyboardButton.K }, { Keys.L, KeyboardButton.L },
        { Keys.M, KeyboardButton.M }, { Keys.N, KeyboardButton.N }, { Keys.O, KeyboardButton.O },
        { Keys.P, KeyboardButton.P }, { Keys.Q, KeyboardButton.Q }, { Keys.R, KeyboardButton.R },
        { Keys.S, KeyboardButton.S }, { Keys.T, KeyboardButton.T }, { Keys.U, KeyboardButton.U },
        { Keys.V, KeyboardButton.V }, { Keys.W, KeyboardButton.W }, { Keys.X, KeyboardButton.X },
        { Keys.Y, KeyboardButton.Y }, { Keys.Z, KeyboardButton.Z },

        // Windows / Apps
        { Keys.LeftWindows, KeyboardButton.LeftWindows },
        { Keys.RightWindows, KeyboardButton.RightWindows },
        { Keys.Apps, KeyboardButton.Apps },
        { Keys.Sleep, KeyboardButton.Sleep },

        // Numpad
        { Keys.NumPad0, KeyboardButton.NumPad0 }, { Keys.NumPad1, KeyboardButton.NumPad1 },
        { Keys.NumPad2, KeyboardButton.NumPad2 }, { Keys.NumPad3, KeyboardButton.NumPad3 },
        { Keys.NumPad4, KeyboardButton.NumPad4 }, { Keys.NumPad5, KeyboardButton.NumPad5 },
        { Keys.NumPad6, KeyboardButton.NumPad6 }, { Keys.NumPad7, KeyboardButton.NumPad7 },
        { Keys.NumPad8, KeyboardButton.NumPad8 }, { Keys.NumPad9, KeyboardButton.NumPad9 },
        { Keys.Multiply, KeyboardButton.Multiply },
        { Keys.Add, KeyboardButton.Add },
        { Keys.Separator, KeyboardButton.Separator },
        { Keys.Subtract, KeyboardButton.Subtract },
        { Keys.Decimal, KeyboardButton.Decimal },
        { Keys.Divide, KeyboardButton.Divide },

        // Function Keys
        { Keys.F1, KeyboardButton.F1 }, { Keys.F2, KeyboardButton.F2 }, { Keys.F3, KeyboardButton.F3 },
        { Keys.F4, KeyboardButton.F4 }, { Keys.F5, KeyboardButton.F5 }, { Keys.F6, KeyboardButton.F6 },
        { Keys.F7, KeyboardButton.F7 }, { Keys.F8, KeyboardButton.F8 }, { Keys.F9, KeyboardButton.F9 },
        { Keys.F10, KeyboardButton.F10 }, { Keys.F11, KeyboardButton.F11 }, { Keys.F12, KeyboardButton.F12 },
        { Keys.F13, KeyboardButton.F13 }, { Keys.F14, KeyboardButton.F14 }, { Keys.F15, KeyboardButton.F15 },
        { Keys.F16, KeyboardButton.F16 }, { Keys.F17, KeyboardButton.F17 }, { Keys.F18, KeyboardButton.F18 },
        { Keys.F19, KeyboardButton.F19 }, { Keys.F20, KeyboardButton.F20 }, { Keys.F21, KeyboardButton.F21 },
        { Keys.F22, KeyboardButton.F22 }, { Keys.F23, KeyboardButton.F23 }, { Keys.F24, KeyboardButton.F24 },

        // Locks
        { Keys.NumLock, KeyboardButton.NumLock },
        { Keys.Scroll, KeyboardButton.Scroll },

        // Modifiers
        { Keys.LeftShift, KeyboardButton.LeftShift },
        { Keys.RightShift, KeyboardButton.RightShift },
        { Keys.LeftControl, KeyboardButton.LeftControl },
        { Keys.RightControl, KeyboardButton.RightControl },
        { Keys.LeftAlt, KeyboardButton.LeftAlt },
        { Keys.RightAlt, KeyboardButton.RightAlt },

        // Browser
        { Keys.BrowserBack, KeyboardButton.BrowserBack },
        { Keys.BrowserForward, KeyboardButton.BrowserForward },
        { Keys.BrowserRefresh, KeyboardButton.BrowserRefresh },
        { Keys.BrowserStop, KeyboardButton.BrowserStop },
        { Keys.BrowserSearch, KeyboardButton.BrowserSearch },
        { Keys.BrowserFavorites, KeyboardButton.BrowserFavorites },
        { Keys.BrowserHome, KeyboardButton.BrowserHome },

        // Volume / Media
        { Keys.VolumeMute, KeyboardButton.VolumeMute },
        { Keys.VolumeDown, KeyboardButton.VolumeDown },
        { Keys.VolumeUp, KeyboardButton.VolumeUp },
        { Keys.MediaNextTrack, KeyboardButton.MediaNextTrack },
        { Keys.MediaPreviousTrack, KeyboardButton.MediaPreviousTrack },
        { Keys.MediaStop, KeyboardButton.MediaStop },
        { Keys.MediaPlayPause, KeyboardButton.MediaPlayPause },

        // Launchers
        { Keys.LaunchMail, KeyboardButton.LaunchMail },
        { Keys.LaunchApplication1, KeyboardButton.LaunchApplication1 },
        { Keys.LaunchApplication2, KeyboardButton.LaunchApplication2 },

        // OEM Keys
        { Keys.OemSemicolon, KeyboardButton.OemSemicolon },
        { Keys.OemPlus, KeyboardButton.OemPlus },
        { Keys.OemComma, KeyboardButton.OemComma },
        { Keys.OemMinus, KeyboardButton.OemMinus },
        { Keys.OemPeriod, KeyboardButton.OemPeriod },
        { Keys.OemQuestion, KeyboardButton.OemQuestion },
        { Keys.OemTilde, KeyboardButton.OemTilde },
        { Keys.OemOpenBrackets, KeyboardButton.OemOpenBrackets },
        { Keys.OemPipe, KeyboardButton.OemPipe },
        { Keys.OemCloseBrackets, KeyboardButton.OemCloseBrackets },
        { Keys.OemQuotes, KeyboardButton.OemQuotes },
        { Keys.Oem8, KeyboardButton.Oem8 },
        { Keys.OemBackslash, KeyboardButton.OemBackslash },
        { Keys.OemClear, KeyboardButton.OemClear },

        // ChatPad
        { Keys.ChatPadGreen, KeyboardButton.ChatPadGreen },
        { Keys.ChatPadOrange, KeyboardButton.ChatPadOrange },

        // Special
        { Keys.ProcessKey, KeyboardButton.ProcessKey },
        { Keys.Attn, KeyboardButton.Attn },
        { Keys.Crsel, KeyboardButton.Crsel },
        { Keys.Exsel, KeyboardButton.Exsel },
        { Keys.EraseEof, KeyboardButton.EraseEof },
        { Keys.Play, KeyboardButton.Play },
        { Keys.Zoom, KeyboardButton.Zoom },
        { Keys.Pa1, KeyboardButton.Pa1 }
    };
    
    public bool GetKeyState(KeyboardButton button)
    {
        if (!_keyMap.TryGetValue(button, out var xnaKey)) return false;
        return Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(xnaKey);
    }

    public IEnumerable<KeyboardButton> GetPressedKeys()
    {
        Keys[] xnaKeys = Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys();
        var pressedButtons = new List<KeyboardButton>();

        foreach (var xnaKey in xnaKeys)
        {
            if (_reverseKeyMap.TryGetValue(xnaKey, out var key))
            {
                pressedButtons.Add(key);
            } 
        }

        return pressedButtons;
    }
}