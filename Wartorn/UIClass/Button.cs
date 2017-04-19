﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn
{
    namespace UIClass
    {
        enum ButtonContentType
        {
            Text,
            SpriteFromSheet,
            Sprite
        }
        class Button : Label
        {
            bool isPressed = false;
            Rectangle internalRect;
            ButtonContentType contentType;
            Rectangle spriteSourceRectangle = Rectangle.Empty;
            Texture2D sprite;
            public Texture2D Sprite { set { sprite = value; contentType = ButtonContentType.Sprite; } }

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

            /// <summary>
            /// Text button
            /// </summary>
            /// <param name="text">The text to display</param>
            /// <param name="position">Position of the top left corner</param>
            /// <param name="size">Size of the button</param>
            /// <param name="font">Font to use</param>
            public Button(string text, Point position, Vector2 size, SpriteFont font)
            {
                contentType = ButtonContentType.Text;
                Text = text;
                Position = position;
                Size = size;
                this.font = font;
                Init();
            }

            /// <summary>
            /// Sprite button
            /// </summary>
            /// <param name="sprite">The source rectangle of the sprite in UISpriteSheet</param>
            /// <param name="position">Position of the top left corner</param>
            /// <param name="scale">Scale of the button</param>
            public Button(Rectangle sprite, Point position, float scale = 1)
            {
                contentType = ButtonContentType.SpriteFromSheet;
                spriteSourceRectangle = sprite;
                Position = position;
                Size = spriteSourceRectangle.Size.ToVector2();
                Scale = scale;
                Init();
            }

            /// <summary>
            /// Sprite button
            /// </summary>
            /// <param name="spritename">The sprite sheet to load</param>
            /// <param name="position">Position of the top left corner</param>
            /// <param name="scale">Scale of the button</param>
            public Button(string spritename, Point position, float scale = 1)
            {
                contentType = ButtonContentType.Sprite;
                sprite = CONTENT_MANAGER.Content.Load<Texture2D>(spritename);
                Position = position;
                Size = sprite.Bounds.Size.ToVector2();
                Scale = scale;
                Init();
            }

            public Button(Texture2D sprite, Point position, float scale = 1)
            {
                contentType = ButtonContentType.Sprite;
                this.sprite = sprite;
                Position = position;
                Size = sprite.Bounds.Size.ToVector2();
                Scale = scale;
                Init();
            }

            private void Init()
            {
                MouseDown += (sender, e) =>
                {
                    isPressed = true;
                };
                MouseUp += (sender, e) =>
                {
                    isPressed = false;
                };
                MouseLeave += (sender, e) =>
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
                switch (contentType)
                {
                    case ButtonContentType.Text:
                        spriteBatch.DrawString(font != null ? font : CONTENT_MANAGER.defaultfont, (string.IsNullOrEmpty(text)) ? "" : text, new Vector2(rect.X, rect.Y) + Size / 4, foregroundColor, Rotation, Vector2.Zero, scale, SpriteEffects.None, LayerDepth.GuiUpper);
                        DrawingHelper.DrawRectangle(internalRect, isPressed ? buttonColorPressed : buttonColorReleased, true);
                        DrawingHelper.DrawRectangle(rect, borderColor, false);
                        break;
                    case ButtonContentType.SpriteFromSheet:
                        spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, Position.ToVector2(), spriteSourceRectangle, isPressed ? buttonColorPressed : buttonColorReleased, Rotation, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth.GuiUpper);
                        break;
                    case ButtonContentType.Sprite:
                        spriteBatch.Draw(sprite, Position.ToVector2(), null, isPressed ? buttonColorPressed : buttonColorReleased, Rotation, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth.GuiUpper);
                        break;
                    default:
                        break;
                }
            }

            public event EventHandler<UIEventArgs> ButtonPressed;
            protected virtual void OnButtonPressed(object sender, UIEventArgs e)
            {
                ButtonPressed?.Invoke(sender, e);
            }
        }
    }
}