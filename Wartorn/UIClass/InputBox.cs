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
        /// <summary>
        /// The current text in the buffer. Assign text to this will clear the buffer.
        /// </summary>
        public virtual string Text
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

        

        public override void Update(InputState inputState, InputState lastInputState)
        {
            base.Update(inputState, lastInputState);

            var keyboardState = inputState.keyboardState;
            var lastKeyboardState = lastInputState.keyboardState;
            if (isFocused)
            {
                if (keyboardState.IsKeyDown(Keys.Back) && lastKeyboardState.IsKeyUp(Keys.Back))
                {
                    if (textBuffer.Length > 0)
                    {
                        textBuffer.Remove(textBuffer.Length - 1, 1);
                    }
                }
                else
                {
                    Keys[] keyInput = keyboardState.GetPressedKeys();
                    Keys[] lastKeyInput = lastKeyboardState.GetPressedKeys();
                    foreach (var key in keyInput)
                    {
                        if (!lastKeyInput.Contains(key) && GetCharKey(key) != null)
                        {
                            textBuffer.Append(GetCharKey(key));
                        }
                    }
                }
            }
        }

        protected string GetCharKey(Keys key)
        {
            string result=null;
            switch (key)
            {
                case Keys.A:
                case Keys.B:
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.I:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.M:
                case Keys.N:
                case Keys.O:
                case Keys.P:
                case Keys.Q:
                case Keys.R:
                case Keys.S:
                case Keys.T:
                case Keys.U:
                case Keys.V:
                case Keys.W:
                case Keys.X:
                case Keys.Y:
                case Keys.Z:
                    result = key.ToString();
                    break;
                case Keys.NumPad0:
                    result = "0";
                    break;
                case Keys.NumPad1:
                    result = "1";
                    break;
                case Keys.NumPad2:
                    result = "2";
                    break;
                case Keys.NumPad3:
                    result = "3";
                    break;
                case Keys.NumPad4:
                    result = "4";
                    break;
                case Keys.NumPad5:
                    result = "5";
                    break;
                case Keys.NumPad6:
                    result = "6";
                    break;
                case Keys.NumPad7:
                    result = "7";
                    break;
                case Keys.NumPad8:
                    result = "8";
                    break;
                case Keys.NumPad9:
                    result = "9";
                    break;
                case Keys.Multiply:
                    result = "*";
                    break;
                case Keys.Add:
                    result = "+";
                    break;
                case Keys.Subtract:
                    result = "-";
                    break;
                case Keys.Divide:
                    result = "/";
                    break;
                default:
                    break;
            }
            return result;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, textBuffer, rect.Location.ToVector2(), foregroundColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
            DrawingHelper.DrawRectangle(rect, backgroundColor, false);
        }
    }
}
