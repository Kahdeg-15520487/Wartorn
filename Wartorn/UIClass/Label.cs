using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn
{
    namespace UIClass
    {
        class Label : UIObject
        {
            public Label()
            {

            }

            public override void Update(InputState inputState, InputState lastInputState)
            {
                UIEventArgs arg = new UIEventArgs(inputState.mouseState);

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
                    OnMouseHover(this,arg);
                }
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(font, (string.IsNullOrEmpty(text)) ? "" : text, new Vector2(rect.X, rect.Y) + Size / 4, foregroundColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                DrawingHelper.DrawRectangle(rect, backgroundColor, false);
            }
        }
    }
}