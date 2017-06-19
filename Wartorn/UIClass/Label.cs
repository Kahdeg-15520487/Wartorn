using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn
{
    namespace UIClass
    {
        public class Label : UIObject
        {

            public virtual bool AutoSize
            {
                get;
                set;
            }
            
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

            public override Vector2 Size
            {
                get
                {
                    return base.Size;
                }

                set
                {
                    base.Size = value;
                }
            }
            /// <summary>
            /// Offset of the text inside the button
            /// </summary>
            public Vector2 Origin
            {
                get
                {
                    return origin;
                }
                set
                {
                    origin = value;
                }
            }

            public float Depth { get; set; } = LayerDepth.GuiUpper;

            public Label()
            {

            }

            public Label(string text, Point position, Vector2 size, SpriteFont font)
            {
                Text = text;
                Position = position;
                Size = size;
                this.font = font;
                origin = new Vector2(rect.X, rect.Y) + Size / 4;
            }

            public Label(string text, Point position, Vector2? size, SpriteFont font,float _scale)
            {
                Text = text;
                Position = position;
                if (size != null)
                {
                    Size = size.Value;
                }
                else
                {
                    Size = font.MeasureString(text);
                    origin = position.ToVector2();
                }
                this.font = font;
                Scale = _scale;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(font != null ? font : CONTENT_MANAGER.defaultfont, (string.IsNullOrEmpty(text)) ? "" : text, Position.ToVector2() - origin, foregroundColor, Rotation, Vector2.Zero, scale, SpriteEffects.None, Depth);
                DrawingHelper.DrawRectangle(rect, backgroundColor, true);
                DrawingHelper.DrawRectangle(rect, borderColor, false);
            }
        }
    }
}