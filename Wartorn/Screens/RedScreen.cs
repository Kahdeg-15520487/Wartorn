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
    class RedScreen : Screen
    {
        private GeonBitUI.Panel mainPanel;

        public RedScreen(GraphicsDevice device) : base(device, "RedScreen")
        {
            mainPanel = new GeonBitUI.Panel(new Vector2(720, 480),skin: GeonBitUI.PanelSkin.None);
            mainPanel.Visible = false;

            GeonBitUI.Panel redpanel = new GeonBitUI.Panel(new Vector2(200, 200));

            GeonBitUI.Label redlabel = new GeonBitUI.Label("red");
            GeonBitUI.Button redbutton = new GeonBitUI.Button("go to red");

            redbutton.OnClick += (sender) =>
            {
                SCREEN_MANAGER.goto_screen("BlueScreen");
            };

            redpanel.AddChild(redlabel);
            redpanel.AddChild(redbutton);

            GeonBitUI.Panel lala = new GeonBitUI.Panel(new Vector2(200, 200), anchor: GeonBitUI.Anchor.TopRight);

            mainPanel.AddChild(redpanel);
            mainPanel.AddChild(lala);
            UserInterface.AddEntity(mainPanel);
        }

        public override bool Init()
        {
            mainPanel.Visible = true;
            return base.Init();
        }

        public override void Shutdown()
        {
            mainPanel.Visible = false;
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            var inputState = CONTENT_MANAGER.inputState;
            var lastInputState = CONTENT_MANAGER.lastInputState;

            if (Utility.HelperFunction.IsKeyPress(Keys.E))
            {
                SCREEN_MANAGER.goto_screen("BlueScreen");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //_device.Clear(Color.Red);
        }
    }
}
