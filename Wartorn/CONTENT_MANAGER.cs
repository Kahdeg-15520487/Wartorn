using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;
using Wartorn.UIClass;
using System.Linq;
using System;

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

        public static RasterizerState antialiasing = new RasterizerState { MultiSampleAntiAlias = true };

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

        public static event EventHandler<MessageEventArgs> handler;

        public static void OnHandle(string e)
        {
            handler?.Invoke(null,new MessageEventArgs(e));
        }
    }
}
