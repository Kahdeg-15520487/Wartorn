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
            protected string text;
            public virtual string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    text = string.IsNullOrEmpty(value) ? text : value;
                }
            }

            public Label()
            {

            }

            public Label(string text, Point position, Vector2 size, SpriteFont font)
            {
                Text = text;
                Position = position;
                Size = size;
                this.font = font;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(font!= null?font:CONTENT_MANAGER.defaultfont, (string.IsNullOrEmpty(text)) ? "" : text, new Vector2(rect.X, rect.Y) + Size / 4, foregroundColor, Rotation, Vector2.Zero, scale, SpriteEffects.None, LayerDepth.GuiUpper);
                DrawingHelper.DrawRectangle(rect, backgroundColor, true);
                DrawingHelper.DrawRectangle(rect, borderColor, false);
            }
        }
    }
}