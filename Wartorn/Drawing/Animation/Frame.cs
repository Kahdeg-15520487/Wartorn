using Microsoft.Xna.Framework;

namespace Wartorn.Drawing.Animation
{
    /// <summary>
    /// Represents a single frame in an animation
    /// </summary>
    public sealed class Frame
    {
        #region Fields

        // Holds a region of the sprite sheet 
        private Rectangle source;

        #endregion

        #region Properties

        public Rectangle Source
        {
            get { return source; }
        }

        //Utility Property for easy access to source.Width
        public int Width
        {
            get { return source.Width; }
        }

        //Utility Property for easy access to source.Height
        public int Height
        {
            get { return source.Height; }
        }

        #endregion

        #region Constructor

        public Frame(int x, int y, int width, int height)
        {
            source = new Rectangle(x, y, width, height);
        }

        #endregion
    }


}
