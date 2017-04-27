using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;
using Wartorn.UIClass;
using System.Linq;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Wartorn.GameData;
using Wartorn.Drawing.Animation;

namespace Wartorn
{
    //singleton to store common data
    public static class CONTENT_MANAGER
    {
        public static ContentManager Content;
        public static SpriteBatch spriteBatch;
        public static SpriteFont defaultfont;
        public static SpriteFont arcadefont;
        public static Texture2D spriteSheet;
        public static Texture2D UIspriteSheet;

        #region animation sprite sheet
        public static Dictionary<UnitType, Texture2D> animationSheets;
        public static List<Animation> animationTypes;
        #endregion

        public static string LocalRootPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);

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
        
        public static event EventHandler<MessageEventArgs> messagebox;
        public static event EventHandler<MessageEventArgs> fileopendialog;
        public static event EventHandler<MessageEventArgs> togglebackgroundform;

        public static void ShowMessageBox(string e)
        {
            messagebox?.Invoke(null, new MessageEventArgs(e));
        }

        public static string ShowFileOpenDialog(string rootpath)
        {
            MessageEventArgs e = new MessageEventArgs(rootpath);
            fileopendialog?.Invoke(null, e);
            return e.message;
        }

        public static void ToggleBackgroundForm(string e)
        {
            togglebackgroundform?.Invoke(null, new MessageEventArgs(e));
        }

        public static void ShowFPS(GameTime gameTime)
        {
            int frameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            spriteBatch.DrawString(defaultfont, frameRate.ToString(), new Vector2(0, 0), Color.Black);
        }
    }
}
