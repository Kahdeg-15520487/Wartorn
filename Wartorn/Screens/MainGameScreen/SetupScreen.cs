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
            //declare ui elements
            Label label_playerinfo = new Label("kahdeg", new Point(10, 20), new Vector2(80, 30), CONTENT_MANAGER.arcadefont);

            Button button_selectmap = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Open), new Point(650, 20), 0.5f);
            Button button_exit = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Exit), new Point(5, 5), 0.5f);
            Button button_start = new Button("Start", new Point(100, 50), null, CONTENT_MANAGER.arcadefont);

            Label enter_name = new Label("Enter your name", new Point((this._device.Viewport.Width / 2) - (int)spriteFont.MeasureString("Enter your name").X / 2 , this._device.Viewport.Height / 2 - 140), null, CONTENT_MANAGER.arcadefont, 1);

            InputBox player_name = new InputBox("", new Point(this._device.Viewport.Width / 2 - 75, this._device.Viewport.Height / 2 - 120), new Vector2(150, 20), CONTENT_MANAGER.hackfont, Color.Black, Color.White);

            Label enter_ip_address = new Label("Enter server ip address", new Point((this._device.Viewport.Width / 2) - (int)spriteFont.MeasureString("Enter server ip address").X / 2 , this._device.Viewport.Height / 2 - 90), null, CONTENT_MANAGER.arcadefont, 1);

            InputBox ip_address = new InputBox("", new Point(this._device.Viewport.Width/2-75, this._device.Viewport.Height / 2-70), new Vector2(150, 20), CONTENT_MANAGER.hackfont, Color.Black, Color.White);

            string text_conect = "Connect to server";

            Point point_conect = new Point(this._device.Viewport.Width / 2 - (int)(spriteFont.MeasureString(text_conect).X + spriteFont.MeasureString(text_conect).X/2) / 2, this._device.Viewport.Height / 2 - 20);

            Button button_connect = new Button(text_conect, point_conect, null, CONTENT_MANAGER.arcadefont);

            //bind event
            button_selectmap.MouseClick += (sender, e) =>
            {
                string path = CONTENT_MANAGER.ShowFileOpenDialog(CONTENT_MANAGER.LocalRootPath);
                string content = string.Empty;
                try
                {
                    content = File.ReadAllText(path);
                }
                catch (Exception er)
                {
                    Utility.HelperFunction.Log(er);
                }

                if (!string.IsNullOrEmpty(content))
                {
                    mapdata = content;
                    var temp = Storage.MapData.LoadMap(content);
                    if (temp != null)
                    {
                        minimap = minimapgen.GenerateMapTexture(temp);
                        map = new Map();
                        map.Clone(temp);
                    }
                }
            };
            button_exit.MouseClick += (sender, e) =>
            {
                SCREEN_MANAGER.goto_screen("MainMenuScreen");
            };
            button_start.MouseClick += (sender, e) =>
            {
                if (map == null)
                {
                    return;
                }
                sessiondata = new SessionData();
                sessiondata.map = new Map();
                sessiondata.map.Clone(Storage.MapData.LoadMap(mapdata));
                sessiondata.gameMode = GameMode.campaign;
                sessiondata.playerInfos = new PlayerInfo[2];
                sessiondata.playerInfos[0] = new PlayerInfo(0, Owner.Red);
                sessiondata.playerInfos[1] = new PlayerInfo(1, Owner.Blue);
                ((GameScreen)SCREEN_MANAGER.get_screen("GameScreen")).InitSession(sessiondata);
                SCREEN_MANAGER.goto_screen("GameScreen");
            };

            //add to canvas
            canvas.AddElement("label_playerinfo", label_playerinfo);
            canvas.AddElement("button_selectmap", button_selectmap);
            canvas.AddElement("button_exit", button_exit);
            canvas.AddElement("button_start", button_start);
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
