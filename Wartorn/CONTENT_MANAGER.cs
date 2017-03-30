using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;
using Wartorn.UIClass;
using System.Linq;

namespace Wartorn
{
    //singleton to store common data
    public static class CONTENT_MANAGER
    {
        public static ContentManager Content;
        public static SpriteBatch spriteBatch;
        public static SpriteFont defaultfont;
        public static Texture2D spriteSheet;
        public static Texture2D UIspriteSheet;

        private static InputState _inputState;
        public static InputState inputState
        {
            get
            {
                return _inputState;
            }
            set
            {
                lastInputState = _inputState;
                _inputState = value;
            }
        }
        public static InputState lastInputState { get; private set; }

    }
}
