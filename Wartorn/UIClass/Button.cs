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
            Rectangle spriteSourceRectangle = Rectangle.Empty;

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

            Color buttonColorPressed = Color.LightSlateGray;
            Color buttonColorReleased = Color.LightGray;
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
                Init();
            }
            public Button(string text,Point position,Vector2 size,SpriteFont font)
            {
                Text = text;
                Position = position;
                Size = size;
                this.font = font;
                Init();
            }
            public Button(Rectangle sprite,Point position,float scale)
            {
                spriteSourceRectangle = sprite;
                Position = position;
                Size = spriteSourceRectangle.Size.ToVector2();
                Scale = scale;
                Init();
            }

            private void Init()
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
                if (spriteSourceRectangle == Rectangle.Empty)
                {
                    spriteBatch.DrawString(font != null ? font : CONTENT_MANAGER.defaultfont, (string.IsNullOrEmpty(text)) ? "" : text, new Vector2(rect.X, rect.Y) + Size / 4, foregroundColor, Rotation, Vector2.Zero, scale, SpriteEffects.None, LayerDepth.Gui);
                    DrawingHelper.DrawRectangle(internalRect, isPressed ? buttonColorPressed : buttonColorReleased, true);
                    DrawingHelper.DrawRectangle(rect, borderColor, false);
                }
                else
                {
                    spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, Position.ToVector2(), spriteSourceRectangle, isPressed ? buttonColorPressed : buttonColorReleased, Rotation, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth.Gui);
                }
            }

            public event EventHandler<UIEventArgs> ButtonPressed;
            protected virtual void OnButtonPressed(object sender,UIEventArgs e)
            {
                ButtonPressed?.Invoke(sender, e);
            }
        }
    }
}