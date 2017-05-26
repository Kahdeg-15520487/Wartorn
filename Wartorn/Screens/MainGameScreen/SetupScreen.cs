using System;
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
using Microsoft.Xna.Framework.Content;
using Client;

namespace Wartorn.Screens.MainGameScreen
{
    class SetupScreen : Screen
    {
        Canvas canvas;

     

        public SetupScreen(GraphicsDevice device) : base(device, "SetupScreen")
        {

        }

        public override bool Init()
        {
            canvas = new Canvas();
            

            InitUI();

            return base.Init();
        }

        private void InitUI()
        {
            SpriteFont spriteFont = CONTENT_MANAGER.arcadefont;
            //declare ui elements
         

            Label enter_name = new Label("Enter your name", new Point((this._device.Viewport.Width / 2) - (int)spriteFont.MeasureString("Enter your name").X / 2 , this._device.Viewport.Height / 2 - 140), null, CONTENT_MANAGER.arcadefont, 1);

            InputBox player_name = new InputBox("", new Point(this._device.Viewport.Width / 2 - 75, this._device.Viewport.Height / 2 - 120), new Vector2(150, 20), CONTENT_MANAGER.hackfont, Color.Black, Color.White);

            Label enter_ip_address = new Label("Enter server ip address", new Point((this._device.Viewport.Width / 2) - (int)spriteFont.MeasureString("Enter server ip address").X / 2 , this._device.Viewport.Height / 2 - 90), null, CONTENT_MANAGER.arcadefont, 1);

            InputBox ip_address = new InputBox("", new Point(this._device.Viewport.Width/2-75, this._device.Viewport.Height / 2-70), new Vector2(150, 20), CONTENT_MANAGER.hackfont, Color.Black, Color.White);

            string text_conect = "Connect to server";

            Point point_conect = new Point(this._device.Viewport.Width / 2 - (int)(spriteFont.MeasureString(text_conect).X + spriteFont.MeasureString(text_conect).X/2) / 2, this._device.Viewport.Height / 2 - 20);

            Button button_connect = new Button(text_conect, point_conect, null, CONTENT_MANAGER.arcadefont);

            //bind event

            Player.Instance.connect_succeed += (sender, e) =>
            {
                SCREEN_MANAGER.goto_screen("Client_Screen");
            };

            button_connect.MouseClick += (sender, e) =>
            {
                Player.Instance.ConnectToServer(ip_address.Text);
            };


            //add to canvas
            canvas.AddElement("enter_name", enter_name);
            canvas.AddElement("enter_ip_address", enter_ip_address);
            canvas.AddElement("player_name", player_name);
            canvas.AddElement("ip_address", ip_address);
            canvas.AddElement("button_connect", button_connect);
        }

        public override void Shutdown()
        {
          
            Player.Instance.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
        }

        public override void Draw(GameTime gameTime)
        {
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
      
        }
    }


}
