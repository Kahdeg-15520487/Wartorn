using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn.UIClass
{
    class InputBox : UIObject
    {
        StringBuilder textBuffer = new StringBuilder();
        public override string Text
        {
            get
            {
                return textBuffer.ToString();
            }
            set
            {
                textBuffer.Clear();
                textBuffer.Append(value);
            }
        }

        bool isFocused;
        public bool IsFocused
        {
            get
            {
                return isFocused;
            }
        }

        public override void Update(InputState inputState, InputState lastInputState)
        {
            UIEventArgs args = new UIEventArgs(inputState.mouseState);
            if (!rect.Contains(inputState.mouseState.Position) && inputState.mouseState.LeftButton == ButtonState.Pressed)
            {
                isFocused = false;
                OnLostFocus(this, args);
            }
            if (rect.Contains(inputState.mouseState.Position) && inputState.mouseState.LeftButton == ButtonState.Pressed)
            {
                isFocused = true;
                OnGotFocus(this, args);
            }

            var keyboardState = inputState.keyboardState;
            var lastKeyboardState = lastInputState.keyboardState;
            if (isFocused)
            {
                if (keyboardState.IsKeyDown(Keys.Back))
                {
                    if (textBuffer.Length > 0)
                    {
                        //textBuffer.Remove(textBuffer.Length - 1, 1);
                        textBuffer.Append(textBuffer[textBuffer.Length - 1]);
                        //textBuffer.Append(textBuffer.Length);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, textBuffer, rect.Location.ToVector2(), foregroundColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
            DrawingHelper.DrawRectangle(rect, backgroundColor, false);
        }
    }
}
