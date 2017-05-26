using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wartorn.UIClass
{
    class PictureBox : UIObject
    {
        private Texture2D texture2D;


        public Texture2D Texture2D
        {
            get
            {
                return texture2D;
            }

            set
            {
                texture2D = value;
            }
        }

        public float Depth { get; set; }

        private Rectangle? sourceRectangle = null;
        public Rectangle SourceRectangle
        {
            get
            {
                return sourceRectangle.GetValueOrDefault();
            }
            set
            {
                sourceRectangle = value;
            }
        }

        /// <summary>
        /// Contructor of PictureBox
        /// </summary>
        /// <param name="texture2D"> Picture you want to display</param>
        /// <param name="position"> Position you want PictrueBox to locate</param>
        /// <param name="_size">Size of PictureBox</param>
        public PictureBox(Texture2D texture2D, Point position,Rectangle? sourceRectangle, Vector2? origin, float rotation = 0f, float scale = 1f, float depth = 0f)
        {
            Texture2D = texture2D;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Depth = depth;
            this.origin = origin ?? Vector2.Zero;
            this.sourceRectangle = sourceRectangle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D, Position.ToVector2(), sourceRectangle, Color.White, Rotation, origin, Scale, SpriteEffects.None, Depth);
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
        public override float Scale
        {
            get
            {
                return base.Scale;
            }

            set
            {
                base.Scale = value;
            }
        }
        public override Point Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                base.Position = value;
            }
        }
        
    }
}
