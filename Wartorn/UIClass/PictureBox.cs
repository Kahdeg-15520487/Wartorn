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
        /// <summary>
        /// Contructor of PictureBox
        /// </summary>
        /// <param name="_texture2D"> Picture you want to display</param>
        /// <param name="_position"> Position you want PictrueBox to locate</param>
        /// <param name="_size">Size of PictureBox</param>
        public PictureBox(Texture2D _texture2D,Point _position,float _rotation,float _scale)
        {
            Texture2D = _texture2D;
            Position = _position;
            Rotation = _rotation;
            Scale = _scale;
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D, Position.ToVector2(), null, Color.White, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
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
