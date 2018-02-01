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
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;
using Microsoft.Xna.Framework.Input.Touch;

namespace Wartorn {
	public struct InputState {
		public MouseState mouseState { get; set; }
		public KeyboardState keyboardState { get; set; }
		public JoystickState joystickState { get; set; }
		public GamePadState gamepadState { get; set; }
		public TouchCollection touchState { get; set; }
		public InputState(MouseState mousestate, KeyboardState keyboardstate) {
			mouseState = mousestate;
			keyboardState = keyboardstate;
			joystickState = new JoystickState();
			gamepadState = new GamePadState();
			touchState = new TouchCollection();
		}
		public InputState(JoystickState joystickstate, GamePadState gamepadstate) {
			mouseState = new MouseState();
			keyboardState = new KeyboardState();
			joystickState = joystickstate;
			gamepadState = gamepadstate;
			touchState = new TouchCollection();
		}
		public InputState(TouchCollection touchstate) {
			mouseState = new MouseState();
			keyboardState = new KeyboardState();
			joystickState = new JoystickState();
			gamepadState = new GamePadState();
			touchState = touchstate;
		}

		#region keyboard state
		public bool IsKeyDown(Keys k) {
			return keyboardState.IsKeyDown(k);
		}

		public bool IsKeyUp(Keys k) {
			return keyboardState.IsKeyUp(k);
		}
		#endregion

		#region mouse state
		public bool IsLeftMouseButtonDown() {
			return mouseState.LeftButton == ButtonState.Pressed;
		}

		public bool IsRightMouseButtonDown() {
			return mouseState.RightButton == ButtonState.Pressed;
		}

		public bool IsMiddleMouseButtonDown() {
			return mouseState.MiddleButton == ButtonState.Pressed;
		}
		#endregion

		#region touch state
		public bool IsGesture(GestureType gestureType) {
			return false;
		}
		#endregion
	}
}
