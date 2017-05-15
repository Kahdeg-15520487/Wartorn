//using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;
using Wartorn.Utility.Drawing;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;

namespace Wartorn.Screens
{
    class TestConsole : Screen
    {
        Canvas canvas;
        Console console;

        public TestConsole(GraphicsDevice device) : base(device, typeof(TestConsole).Name)
        {}

        public override bool Init()
        {
            canvas = new Canvas();

            InitUI();

            return base.Init();
        }

        private void InitUI()
        {
            console = new Console(new Point(0, 50), new Vector2(400, 200), CONTENT_MANAGER.hackfont);
            Label lbl_test = new Label("", new Point(100, 0), new Vector2(80, 30), CONTENT_MANAGER.defaultfont);

            console.CommandSubmitted += (sender, e) =>
            {
                lbl_test.Text = console.Text;
            };

            canvas.AddElement("console", console);
            canvas.AddElement("lbl_test", lbl_test);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);

            if (HelperFunction.IsKeyPress(Keys.OemTilde))
            {
                console.IsVisible = !console.IsVisible;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
        }
    }
}