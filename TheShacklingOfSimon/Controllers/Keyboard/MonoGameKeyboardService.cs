using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;

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

    public MonoGameKeyboardService()
    {
        // No-op
    }
    
    public BinaryInputState GetKeyState(KeyboardButton button)
    {
        BinaryInputState state = BinaryInputState.Released;
        if (_keyMap.TryGetValue(button, out Keys xnaKey))
        {
            state = Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(xnaKey)
                ? BinaryInputState.Pressed
                : BinaryInputState.Released;
        }
        return state;
    }
}