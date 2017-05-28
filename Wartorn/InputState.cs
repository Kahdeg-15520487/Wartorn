using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;
using Wartorn.Utility.Drawing;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;

namespace Wartorn
{
    public struct InputState
    {
        public MouseState mouseState { get; set; }
        public KeyboardState keyboardState { get; set; }
        public JoystickState joystickState { get; set; }
        public GamePadState gamepadState { get; set; }
        public InputState(MouseState mousestate, KeyboardState keyboardstate)
        {
            mouseState = mousestate;
            keyboardState = keyboardstate;
            joystickState = new JoystickState();
            gamepadState = new GamePadState();
        }
        public InputState(MouseState mousestate, KeyboardState keyboardstate, JoystickState joystickstate, GamePadState gamepadstate)
        {
            mouseState = mousestate;
            keyboardState = keyboardstate;
            joystickState = joystickstate;
            gamepadState = gamepadstate;
        }

        public bool IsKeyDown(Keys k)
        {
            return keyboardState.IsKeyDown(k);
        }

        public bool IsKeyUp(Keys k)
        {
            return keyboardState.IsKeyUp(k);
        }

        public bool IsLeftMouseButtonDown()
        {
            return mouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightMouseButtonDown()
        {
            return mouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMiddleMouseButtonDown()
        {
            return mouseState.MiddleButton == ButtonState.Pressed;
        }
    }
}
