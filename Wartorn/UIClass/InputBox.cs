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
    public class InputBox : UIObject
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
        public List<char> ignoreCharacter;
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
            ignoreCharacter = new List<char>();

            CONTENT_MANAGER.gameinstance.Window.TextInput += TextInputHandler;
        }

        private void TextInputHandler(object sender, TextInputEventArgs e)
        {
            if (isFocused)
            {
                if (font.Characters.Contains(e.Character) && !ignoreCharacter.Contains(e.Character))
                {
                    textBuffer.Append(e.Character);
                    CursorPosition++;
                }
            }
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

            if (HelperFunction.IsKeyPress(Keys.Back))
            {
                if (textBuffer.Length > 0)
                {
                    textBuffer.Remove((CursorPosition - 1).Clamp(textBuffer.Length, 0), 1);
                    CursorPosition--;
                }
            }

            if (inputState.keyboardState.IsKeyDown(Keys.LeftControl) && HelperFunction.IsKeyPress(Keys.V))
            {
                string paste = CONTENT_MANAGER.GetClipboard();
                textBuffer.Append(paste);
                CursorPosition += paste.Length;
            }
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
