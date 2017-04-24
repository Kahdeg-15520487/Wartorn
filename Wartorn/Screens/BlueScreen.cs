using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Wartorn.ScreenManager;

using GeonBit.UI;
using GeonBitUI = GeonBit.UI.Entities;

namespace Wartorn.Screens
{
    class BlueScreen : Screen
    {
        private GeonBitUI.Panel mainpanel;

        public BlueScreen(GraphicsDevice device) : base(device, "BlueScreen")
        {
            mainpanel = new GeonBitUI.Panel(new Vector2(720, 480), skin: GeonBitUI.PanelSkin.None);
            mainpanel.Visible = false;

            GeonBitUI.Panel bluepanel = new GeonBitUI.Panel(new Vector2(200, 200));

            GeonBitUI.Label bluelabel = new GeonBitUI.Label("blue");
            GeonBitUI.Button bluebutton = new GeonBitUI.Button("go to red");

            bluebutton.OnClick += (sender) =>
            {
                SCREEN_MANAGER.goto_screen("RedScreen");
            };

            bluepanel.AddChild(bluelabel);
            bluepanel.AddChild(bluebutton);

            mainpanel.AddChild(bluepanel);
            UserInterface.AddEntity(mainpanel);
        }

        public override bool Init()
        {
            mainpanel.Visible = true;
            return base.Init();
        }

        public override void Shutdown()
        {
            mainpanel.Visible = false;
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            var inputState = CONTENT_MANAGER.inputState;
            var lastInputState = CONTENT_MANAGER.lastInputState;

            if (inputState.keyboardState.IsKeyUp(Keys.E) && lastInputState.keyboardState.IsKeyDown(Keys.E))
            {
                SCREEN_MANAGER.goto_screen("RedScreen");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //_device.Clear(Color.Blue);
        }
    }
}
