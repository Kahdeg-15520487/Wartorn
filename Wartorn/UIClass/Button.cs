using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn
{
    namespace UIClass
    {
        class Button : Label
        {
            bool isPressed = false;
            Rectangle internalRect;

            public override Point Position
            {
                get
                {
                    return rect.Location;
                }
                set
                {
                    rect.Location = value;
                    internalRect.Location = new Point(rect.Location.X + 1, rect.Location.Y + 1);
                }
            }
            public override Vector2 Size
            {
                get
                {
                    return rect.Size.ToVector2();
                }
                set
                {
                    rect.Size = (value.X > 0 && value.Y > 0) ? value.ToPoint() : rect.Size;
                    internalRect.Size = new Point(rect.Size.X - 1, rect.Size.Y - 1);
                }
            }

            Color buttonColorPressed;
            Color buttonColorReleased;
            public Color ButtonColorPressed
            {
                get
                {
                    return buttonColorPressed;
                }
                set
                {
                    buttonColorPressed = value;
                }
            }
            public Color ButtonColorReleased
            {
                get
                {
                    return buttonColorReleased;
                }
                set
                {
                    buttonColorReleased = value;
                }
            }

            public Button()
            {
                MouseDown += delegate (object sender, UIEventArgs e)
                {
                    isPressed = true;
                };
                MouseUp += delegate (object sender, UIEventArgs e)
                {
                    isPressed = false;
                };
            }

            public override void Update(InputState inputState, InputState lastInputState)
            {
                base.Update(inputState, lastInputState);
                if (isPressed)
                {
                    OnButtonPressed(this, new UIEventArgs(inputState.mouseState));
                }
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(font, (string.IsNullOrEmpty(text)) ? "" : text, new Vector2(rect.X, rect.Y) + Size / 4, foregroundColor, Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                DrawingHelper.DrawRectangle(internalRect, isPressed ? buttonColorPressed : buttonColorReleased, true);
                DrawingHelper.DrawRectangle(rect, borderColor, false);
                //base.Draw(spriteBatch);
            }

            public EventHandler<UIEventArgs> ButtonPressed;
            protected virtual void OnButtonPressed(object sender,UIEventArgs e)
            {
                ButtonPressed?.Invoke(sender, e);
            }
        }
    }
}