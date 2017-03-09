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

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(font, (string.IsNullOrEmpty(text)) ? "" : text, new Vector2(rect.X, rect.Y) + Size / 4, foregroundColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                DrawingHelper.DrawRectangle(rect, backgroundColor, true);
                DrawingHelper.DrawRectangle(rect, borderColor, false);
            }
        }
    }
}