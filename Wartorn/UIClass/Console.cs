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
    class Console : UIObject
    {
        public InputBox inputbox;
        private Vector2 relativePositionToMouse = Vector2.Zero;
        private string text = null;

        public Console()
        {
            MouseDown += drag;
            inputbox = new InputBox()
            {
                Position = new Point(0,(int)Size.Y-30),
                Size = new Vector2((int)Size.X,25),
                backgroundColor = Color.LightGray,
                foregroundColor = Color.White
            };
            //inputbox.font = this.font;
            if (this.font == null)
            {
                //throw new NullReferenceException();
            }
        }

        public override void Update(InputState inputState, InputState lastInputState)
        {
            var keyboardState = inputState.keyboardState;
            var lastKeyboardState = lastInputState.keyboardState;

            if (keyboardState.IsKeyDown(Keys.OemTilde) && lastKeyboardState.IsKeyUp(Keys.OemTilde))
            {
                isVisible = !isVisible;
            }

            inputbox.Update(inputState, lastInputState);
            base.Update(inputState, lastInputState);
        }

        public void drag(object sender,UIEventArgs e)
        {
            //text = e.mouseState.Position.ToString() + " " + e.mouseState.LeftButton.ToString();

            Position = (Position.ToVector2() + (e.mouseState.Position.ToVector2() - new Vector2(Position.X + Size.X/2,Position.Y + Size.Y/2))).ToPoint();
            text = Position.ToString();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font != null ? font : CONTENT_MANAGER.defaultfont, (string.IsNullOrEmpty(text)) ? "" : text, new Vector2(rect.X, rect.Y) + Size / 4, foregroundColor, Rotation, Vector2.Zero, scale, SpriteEffects.None, LayerDepth.Gui);
            DrawingHelper.DrawRectangle(Position.ToVector2(), Size, backgroundColor, true);
            inputbox.Draw(spriteBatch);
        }
    }
}
