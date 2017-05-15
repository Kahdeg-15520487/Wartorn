using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;
using Wartorn.Utility;

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
        public Color caretColor { get; set; } = Color.DarkGray;
        public int CursorPosition { get; set; }
        private int maxTextLength;
        private int textSpacing;

        public InputBox(string text, Point position, Vector2 size, SpriteFont font,Color foregroundColor ,Color backgroundColor)
        {
            Text = text;
            Position = position;
            Size = size;
            this.font = font;
            this.foregroundColor = foregroundColor;
            this.backgroundColor = backgroundColor;
            CursorPosition = 0;
            maxTextLength = findMaxTextLength();
            textSpacing = rect.Width / maxTextLength;
        }

        private int findMaxTextLength()
        {
            string teststr = "A";
            while (font.MeasureString(teststr).X < rect.Width)
            {
                teststr += "A";
            }
            return teststr.Length;
        }

        public override void Update(InputState inputState, InputState lastInputState)
        {
            base.Update(inputState, lastInputState);

            var keyboardState = inputState.keyboardState;
            var lastKeyboardState = lastInputState.keyboardState;
            if (isFocused)
            {
                if (CursorPosition < maxTextLength)
                {
                    if (keyboardState.IsKeyDown(Keys.Back) && lastKeyboardState.IsKeyUp(Keys.Back))
                    {
                        if (textBuffer.Length > 0)
                        {
                            textBuffer.Remove(textBuffer.Length - 1, 1);
                            CursorPosition--;
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
                                textBuffer.Insert(CursorPosition, GetCharKey(key));
                                CursorPosition++;
                            }
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

                case Keys.Space:
                    result = " ";
                    break;

                case Keys.NumPad0:
                case Keys.D0:
                    result = "0";
                    break;
                case Keys.NumPad1:
                case Keys.D1:
                    result = "1";
                    break;
                case Keys.NumPad2:
                case Keys.D2:
                    result = "2";
                    break;
                case Keys.NumPad3:
                case Keys.D3:
                    result = "3";
                    break;
                case Keys.NumPad4:
                case Keys.D4:
                    result = "4";
                    break;
                case Keys.NumPad5:
                case Keys.D5:
                    result = "5";
                    break;
                case Keys.NumPad6:
                case Keys.D6:
                    result = "6";
                    break;
                case Keys.NumPad7:
                case Keys.D7:
                    result = "7";
                    break;
                case Keys.NumPad8:
                case Keys.D8:
                    result = "8";
                    break;
                case Keys.NumPad9:
                case Keys.D9:
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

                case Keys.Left:
                    CursorPosition--;
                    break;
                case Keys.Right:
                    CursorPosition++;
                    break;
                default:
                    break;
            }
            CursorPosition = CursorPosition.Clamp(maxTextLength, 0);
            return result;
        }

        public void Clear()
        {
            textBuffer.Clear();
            CursorPosition = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, textBuffer, rect.Location.ToVector2(), foregroundColor, Rotation, origin, scale, SpriteEffects.None, LayerDepth.GuiLower);

            //Draw text caret
            spriteBatch.DrawString(font, "|", rect.Location.ToVector2() + new Vector2(CursorPosition * textSpacing - 5, -2), caretColor);

            DrawingHelper.DrawRectangle(rect, backgroundColor, true);
        }
    }
}
