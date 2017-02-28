using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wartorn.UIClass
{
    abstract class UIObject:IUIEvent
    {
        protected Rectangle rect = new Rectangle();
        protected string text;
        protected Vector2 origin;
        public virtual Point Position
        {
            get
            {
                return rect.Location;
            }
            set
            {
                rect.Location = value;
            }
        }
        public virtual Vector2 Size {
            get
            {
                return rect.Size.ToVector2();
            }
            set
            {
                rect.Size = (value.X > 0 && value.Y > 0) ? value.ToPoint() : rect.Size;
            }
        }
        public virtual string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = string.IsNullOrEmpty(value) ? text : value;
            }
        }

        public SpriteFont font { get; set; }
        public Color backgroundColor { get; set; }
        public Color foregroundColor { get; set; }
        public float rotation { get; set; }
        protected int scale;
        public virtual int Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                Size = new Vector2(Size.X, Size.Y) * scale;
            }
        }

        public abstract void Update(InputState inputState, InputState lastInputState);
        public abstract void Draw(SpriteBatch spriteBatch);

        public event EventHandler<UIEventArgs> MouseClick;
        public event EventHandler<UIEventArgs> MouseDown;
        public event EventHandler<UIEventArgs> MouseUp;
        public event EventHandler<UIEventArgs> MouseHover;
        public event EventHandler<UIEventArgs> MouseEnter;
        public event EventHandler<UIEventArgs> MouseLeave;
        public event EventHandler<UIEventArgs> GotFocus;
        public event EventHandler<UIEventArgs> LostFocus;

        protected virtual void OnMouseClick(object sender, UIEventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }

        protected virtual void OnMouseDown(object sender, UIEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        protected virtual void OnMouseUp(object sender, UIEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        protected virtual void OnMouseHover(object sender, UIEventArgs e)
        {
            MouseHover?.Invoke(this, e);
        }

        protected virtual void OnMouseEnter(object sender, UIEventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        protected virtual void OnMouseLeave(object sender, UIEventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        protected virtual void OnGotFocus(object sender, UIEventArgs e)
        {
            GotFocus?.Invoke(this, e);
        }

        protected virtual void OnLostFocus(object sender, UIEventArgs e)
        {
            LostFocus?.Invoke(this, e);
        }
    }

    interface IUIEvent
    {
        event EventHandler<UIEventArgs> MouseClick;
        event EventHandler<UIEventArgs> MouseDown;
        event EventHandler<UIEventArgs> MouseUp;
        event EventHandler<UIEventArgs> MouseHover;
        event EventHandler<UIEventArgs> MouseEnter;
        event EventHandler<UIEventArgs> MouseLeave;
        event EventHandler<UIEventArgs> GotFocus;
        event EventHandler<UIEventArgs> LostFocus;
    }

    public class UIEventArgs : EventArgs
    {
        public MouseState mouseState { get; set; }
        public UIEventArgs(MouseState mousestate)
        {
            mouseState = mousestate;
        }
    }

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
    }
}
