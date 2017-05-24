using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;
using Wartorn.Utility.Drawing;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;

namespace Wartorn
{
    namespace ScreenManager
    {
        public class IntroScreen : Screen
        {
            protected GraphicsDevice _device = null;
            Video introvideo;
            VideoPlayer videoplayer;

            /// <summary>
            /// Wartorn1 Constructor
            /// </summary>
            /// <param name="name">Must be unique since when you use ScreenManager is per name</param>
            public IntroScreen(GraphicsDevice device) : base(device, "IntroScreen")
            {
                _device = device;
            }

            /// <summary>
            /// override Function that's called when entering a Wartorn1
            /// override it and add your own initialization code
            /// </summary>
            /// <returns></returns>
            public override bool Init()
            {
                LoadContent();
                videoplayer.Play(introvideo);
                return true;
            }

            private void LoadContent()
            {
                //try
                {
                    introvideo = CONTENT_MANAGER.Content.Load<Video>("intro");
                }
                //catch (Exception e)
                {
                    //CONTENT_MANAGER.ShowMessageBox(e.Message);
                }
                videoplayer = new VideoPlayer();
            }

            /// <summary>
            /// override Function that's called when exiting a Wartorn1
            /// override it and add your own shutdown code
            /// </summary>
            /// <returns></returns>
            public override void Shutdown()
            {
            }

            /// <summary>
            /// Override it to have access to elapsed time
            /// </summary>
            /// <param name="elapsed">GameTime</param>
            public override void Update(GameTime gameTime)
            {
                if (videoplayer.State == MediaState.Stopped)
                {
                    SCREEN_MANAGER.goto_screen("MainMenuScreen");
                }
            }

            public override void Draw(GameTime gameTime)
            {

            }

        }

    }
}