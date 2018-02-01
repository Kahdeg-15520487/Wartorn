using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;
using Wartorn.CustomJsonConverter;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Input.Touch;




//Z̷̪̰̩̠̈́͑͑̇͛Å̖̯̺̜͗L͖̬̗͚ͥ̔͞G͕̝̥͊̋͐ͬͥ͊̑͜O͔͉̻̪̾͛̄̇ ̜ͨͣͧ͛̄̈́!̰͙̦̦̱̲̬̓̓ͥͯͬ̒͌ ̺̭͖̘͞h̺̮̣͎ͦ̓͑ḛ̗̰ͬ̌̓̂̊̚ ̰̥̱͕ͫ̔c͂̐ͤͧ́͗̍o̴̫͙̘͈͍͙ͫm̗̖͑̀ͧͮ͜e̜̺͈͎̬͔͌́̐̈́̅̔͂

namespace Wartorn.UIClass {
	public abstract class UIObject : IUIEvent {
		private UIObject _container = null;
		public UIObject Container {
			get { return _container; }
			set {
				_container = value;             //assign the elment to a container
				this.Position = this.Position;  //recalculate the element's position in relative to its container
			}
		}

		public Rectangle rect = Rectangle.Empty;

		protected Vector2 origin = Vector2.Zero;
		/// <summary>
		/// The position of the UI element a.k.a where the top left corner of this element should be
		/// </summary>
		public virtual Point Position {
			get {
				return rect.Location;
			}
			set {
				rect.Location = _container == null ? value : new Point(_container.Position.X + value.X, _container.Position.Y + value.Y);
			}
		}

		/// <summary>
		/// The size of the UI element a.k.a where the bottom right corner of this element should be offset by the position
		/// </summary>
		public virtual Vector2 Size {
			get {
				return rect.Size.ToVector2();
			}
			set {
				rect.Size = (value.X > 0 && value.Y > 0) ? value.ToPoint() : rect.Size;
			}
		}

		protected bool isFocused = false;
		/// <summary>
		/// Check if the UI element get focus
		/// </summary>
		public bool IsFocused {
			get {
				return isFocused;
			}
		}

		protected bool isVisible = true;
		/// <summary>
		/// To draw the control or not.
		/// </summary>
		public bool IsVisible {
			get {
				return isVisible;
			}
			set {
				isVisible = value;
			}
		}
		
		/// <summary>
		/// This must be assign with a font
		/// </summary>
		public SpriteFont Font { get; set; }

		/// <summary>
		/// Color of the background of the control.
		/// Default color is Transparent
		/// </summary>
		public virtual Color BackgroundColor {
			get { return backgroundColor; }
			set { backgroundColor = value; }
		}
		private Color backgroundColor = Color.Transparent;

		/// <summary>
		/// Color of the text that the control displays.
		/// Default color is Black
		/// </summary>
		public Color ForegroundColor {
			get { return foregroundColor; }
			set { foregroundColor = value; }
		}
		private Color foregroundColor = Color.Black;

		/// <summary>
		/// Color of the control's border.
		/// Default color is Transparent
		/// </summary>
		public Color BorderColor {
			get { return borderColor; }
			set { borderColor = value; }
		}
		private Color borderColor = Color.Transparent;
		/// <summary>
		/// Background color of the control when got focus.
		/// Default color is Transparent
		/// </summary>
		public Color gotfocusColor = Color.Transparent;
		/// <summary>
		/// Background color of the control when lost focus.
		/// Default color is Transparent
		/// </summary>
		public Color lostfocusColor = Color.Transparent;

		protected float rotation = 0f;
		/// <summary>
		/// Rotation calculated in radiant
		/// </summary>
		public float Rotation {
			get { return rotation; }
			set { rotation = value; }
		}
		protected float scale = 1.0f;
		/// <summary>
		/// Scale of the control.
		/// Default scale is 1.0f
		/// </summary>
		public virtual float Scale {
			get { return scale; }
			set {
				scale = value;
				Size = new Vector2(Size.X, Size.Y) * scale;
			}
		}

		public object MetaData { get; set; } = null;

		private bool isDraging = false;
		private Point drag_start;
		private Point drag_end;

		public virtual void Update(GameTime gameTime, InputState currentInputState, InputState lastInputState) {
			UIEventArgs arg = new UIEventArgs(currentInputState, lastInputState);

			//LostFocus
			if ((!rect.Contains(currentInputState.mouseState.Position) && currentInputState.mouseState.LeftButton == ButtonState.Pressed)
				|| !rect.Contains(currentInputState.touchState.FirstOrDefault().Position)) {
				isFocused = false;
				OnLostFocus(this, arg);
			}

			//GotFocus
			if ((rect.Contains(currentInputState.mouseState.Position) && currentInputState.mouseState.LeftButton == ButtonState.Pressed)
				|| rect.Contains(currentInputState.touchState.FirstOrDefault().Position)) {
				isFocused = true;

				OnGotFocus(this, arg);
			}

			//MouseClick
			if ((rect.Contains(currentInputState.mouseState.Position)
				&& (lastInputState.mouseState.LeftButton == ButtonState.Released
					&& currentInputState.mouseState.LeftButton == ButtonState.Pressed))
				||
				(rect.Contains(currentInputState.touchState.FirstOrDefault().Position)
				&& !rect.Contains(lastInputState.touchState.FirstOrDefault().Position))) {
				OnMouseClick(this, arg);
			}

			//MouseDown
			if ((rect.Contains(currentInputState.mouseState.Position)
				&& currentInputState.mouseState.LeftButton == ButtonState.Pressed)
				|| rect.Contains(currentInputState.touchState.FirstOrDefault().Position)) {
				OnMouseDown(this, arg);
				if (!isDraging) {
					isDraging = true;
					//get mouse absolute position
					drag_start = arg.currentMouseState.Position;
				}
			}

			//MouseUp
			if (rect.Contains(currentInputState.mouseState.Position)
				&& (lastInputState.mouseState.LeftButton == ButtonState.Pressed
					&& currentInputState.mouseState.LeftButton == ButtonState.Released)) {
				OnMouseUp(this, arg);
				if (isDraging) {
					isDraging = false;
					//get mouse absolute position
					drag_end = arg.currentMouseState.Position;
					//convert to size
					drag_end = new Point(drag_end.X - drag_start.X, drag_end.Y - drag_start.Y);
					OnMouseDrag(this, new Rectangle(drag_start, drag_end));
				}
			}

			//MouseEnter
			if ((rect.Contains(currentInputState.mouseState.Position)
				&& !rect.Contains(lastInputState.mouseState.Position))
				|| (rect.Contains(currentInputState.touchState.FirstOrDefault().Position))
					&& !rect.Contains(lastInputState.touchState.FirstOrDefault().Position)) {
				OnMouseEnter(this, arg);
			}

			//MouseLeave
			if ((!rect.Contains(currentInputState.mouseState.Position)
				&& rect.Contains(lastInputState.mouseState.Position))
				|| (!rect.Contains(currentInputState.touchState.FirstOrDefault().Position))
					&& rect.Contains(lastInputState.touchState.FirstOrDefault().Position)) {
				OnMouseLeave(this, arg);
			}

			//MouseHover
			if (rect.Contains(currentInputState.mouseState.Position)
				|| rect.Contains(currentInputState.touchState.FirstOrDefault().Position)) {
				OnMouseHover(this, arg);
			}

			//KeyPress
			if (isFocused && currentInputState.keyboardState.GetPressedKeys().GetLength(0) > 0) {
				OnKeyPress(this, arg);
			}
		}

		public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

		public event EventHandler<UIEventArgs> MouseClick;
		public event EventHandler<UIEventArgs> MouseDown;
		public event EventHandler<UIEventArgs> MouseUp;
		public event EventHandler<Rectangle> MouseDrag;
		public event EventHandler<UIEventArgs> MouseHover;
		public event EventHandler<UIEventArgs> MouseEnter;
		public event EventHandler<UIEventArgs> MouseLeave;
		public event EventHandler<UIEventArgs> GotFocus;
		public event EventHandler<UIEventArgs> LostFocus;

		public event EventHandler<UIEventArgs> KeyPress;

		protected virtual void OnMouseClick(object sender, UIEventArgs e) {
			MouseClick?.Invoke(sender, e);
		}

		protected virtual void OnMouseDown(object sender, UIEventArgs e) {
			MouseDown?.Invoke(sender, e);
		}

		protected virtual void OnMouseUp(object sender, UIEventArgs e) {
			MouseUp?.Invoke(sender, e);
		}

		protected virtual void OnMouseDrag(object sender, Rectangle e) {
			MouseDrag?.Invoke(sender, e);
		}

		protected virtual void OnMouseHover(object sender, UIEventArgs e) {
			MouseHover?.Invoke(sender, e);
		}

		protected virtual void OnMouseEnter(object sender, UIEventArgs e) {
			MouseEnter?.Invoke(sender, e);
		}

		protected virtual void OnMouseLeave(object sender, UIEventArgs e) {
			MouseLeave?.Invoke(sender, e);
		}

		protected virtual void OnGotFocus(object sender, UIEventArgs e) {
			GotFocus?.Invoke(sender, e);
		}

		protected virtual void OnLostFocus(object sender, UIEventArgs e) {
			LostFocus?.Invoke(sender, e);
		}

		protected virtual void OnKeyPress(object sender, UIEventArgs e) {
			KeyPress?.Invoke(sender, e);
		}
	}

	interface IUIEvent {
		event EventHandler<UIEventArgs> MouseClick;
		event EventHandler<UIEventArgs> MouseDown;
		event EventHandler<UIEventArgs> MouseUp;
		event EventHandler<Rectangle> MouseDrag;
		event EventHandler<UIEventArgs> MouseHover;
		event EventHandler<UIEventArgs> MouseEnter;
		event EventHandler<UIEventArgs> MouseLeave;
		event EventHandler<UIEventArgs> GotFocus;
		event EventHandler<UIEventArgs> LostFocus;
	}

	public class UIEventArgs : EventArgs {
		public InputState currentInputState { get; }
		public InputState lastInputState { get; }
		public MouseState currentMouseState { get { return currentInputState.mouseState; } }
		public MouseState lastMouseState { get { return lastInputState.mouseState; } }
		public KeyboardState currentKeyboardState { get { return currentInputState.keyboardState; } }
		public KeyboardState lastKeyboardState { get { return lastInputState.keyboardState; } }
		public TouchCollection currentTouchState { get { return currentInputState.touchState; } }
		public TouchCollection lastTouchState { get { return lastInputState.touchState; } }
		public UIEventArgs(InputState currentInputState, InputState lastInputState) {
			this.currentInputState = currentInputState;
			this.lastInputState = lastInputState;
		}
	}
}
