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
        
        protected Vector2 origin = Vector2.Zero;
        /// <summary>
        /// The position of the UI element a.k.a where the top left corner of this element should be
        /// </summary>
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
        /// <summary>
        /// The size of the UI element a.k.a where the bottom right corner of this element should be offset by the position
        /// </summary>
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

        protected bool isFocused = false;
        /// <summary>
        /// Check if the UI element get focus
        /// </summary>
        public bool IsFocused
        {
            get
            {
                return isFocused;
            }
        }

        protected bool isVisible = true;
        /// <summary>
        /// To draw the controle or not.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
            }
        }

        /// <summary>
        /// This must be assign with a font
        /// </summary>
        public SpriteFont font { get; set; }
        /// <summary>
        /// Color of the background of the control.
        /// Default color is Transparent
        /// </summary>
        public Color backgroundColor = Color.Transparent;
        /// <summary>
        /// Color of the text that the control displays.
        /// Default color is Black
        /// </summary>
        public Color foregroundColor = Color.Black;
        /// <summary>
        /// Color of the control's border.
        /// Default color is Transparent
        /// </summary>
        public Color borderColor = Color.Transparent;
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
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }
        protected float scale = 1.0f;
        /// <summary>
        /// Scale of the control.
        /// Default scale is 1.0f
        /// </summary>
        public virtual float Scale
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
        
        public virtual void Update(InputState inputState, InputState lastInputState)
        {
            UIEventArgs arg = new UIEventArgs(inputState.mouseState);

            //LostFocus
            if (!rect.Contains(inputState.mouseState.Position) && inputState.mouseState.LeftButton == ButtonState.Pressed)
            {
                isFocused = false;
                OnLostFocus(this, arg);
            }

            //GotFocus
            if (rect.Contains(inputState.mouseState.Position) && inputState.mouseState.LeftButton == ButtonState.Pressed)
            {
                isFocused = true;
                OnGotFocus(this, arg);
            }

            //MouseClick
            if (rect.Contains(inputState.mouseState.Position)
                && (lastInputState.mouseState.LeftButton == ButtonState.Released
                    && inputState.mouseState.LeftButton == ButtonState.Pressed))
            {
                OnMouseClick(this, arg);
            }

            //MouseDown
            if (rect.Contains(inputState.mouseState.Position)
                && inputState.mouseState.LeftButton == ButtonState.Pressed)
            {
                OnMouseDown(this, arg);
            }

            //MouseUp
            if (rect.Contains(inputState.mouseState.Position)
                && (lastInputState.mouseState.LeftButton == ButtonState.Pressed
                    && inputState.mouseState.LeftButton == ButtonState.Released))
            {
                OnMouseUp(this, arg);
            }

            //MouseEnter
            if (rect.Contains(inputState.mouseState.Position)
                && !rect.Contains(lastInputState.mouseState.Position))
            {
                OnMouseEnter(this, arg);
            }

            //MouseLeave
            if (!rect.Contains(inputState.mouseState.Position)
                && rect.Contains(lastInputState.mouseState.Position))
            {
                OnMouseLeave(this, arg);
            }

            //MouseHover
            if (rect.Contains(inputState.mouseState.Position))
            {
                OnMouseHover(this, arg);
            }
        }
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
