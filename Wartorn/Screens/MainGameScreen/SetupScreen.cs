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
        SessionData sessiondata;
        Canvas canvas;

        Map map = null;

        MiniMapGenerator minimapgen;
        Texture2D minimap;

        string mapdata;

        public SetupScreen(GraphicsDevice device) : base(device, "SetupScreen")
        {
            
        }

        public override bool Init()
        {
            canvas = new Canvas();
            sessiondata = new SessionData();

            InitUI();

            minimapgen = new MiniMapGenerator(_device, CONTENT_MANAGER.spriteBatch);

            return base.Init();
        }

        private void InitUI()
        {
            //declare ui elements
            

            //Button button_selectmap = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Open), new Point(650, 20), 0.5f);
            Button button_exit = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Exit), new Point(5, 5), 0.5f);
            //Button button_start = new Button("Start", new Point(100, 50), null, CONTENT_MANAGER.arcadefont);
            InputBox ip_address = new InputBox("", new Point(this._device.Viewport.Width/2-50, this._device.Viewport.Height / 2-70), new Vector2(150, 20), CONTENT_MANAGER.hackfont, Color.Black, Color.White);
            Button button_connect = new Button("Connect to server", new Point(this._device.Viewport.Width / 2-70, this._device.Viewport.Height / 2-20), null, CONTENT_MANAGER.arcadefont);
            Label enter_name = new Label("ENTER YOUR NAME AND IP ADDRESS OF SERVER", new Point((this._device.Viewport.Width / 2 )- (int)CONTENT_MANAGER.arcadefont.MeasureString("ENTER YOUR NAME AND IP ADDRESS OF SERVER").X/2 -10,30 ), null, CONTENT_MANAGER.arcadefont,1);
            //bind event
            //button_selectmap.MouseClick += (sender, e) =>
            //{
            //    string path = CONTENT_MANAGER.ShowFileOpenDialog(CONTENT_MANAGER.LocalRootPath);
            //    string content = string.Empty;
            //    try
            //    {
            //        content = File.ReadAllText(path);
            //    }
            //    catch (Exception er)
            //    {
            //        Utility.HelperFunction.Log(er);
            //    }

            //    if (!string.IsNullOrEmpty(content))
            //    {
            //        mapdata = content;
            //        var temp = Storage.MapData.LoadMap(content);
            //        if (temp != null)
            //        {
            //            minimap = minimapgen.GenerateMapTexture(temp);
            //            map = new Map();
            //            map.Clone(temp);
            //        }
            //    }
            //};
            button_exit.MouseClick += (sender, e) =>
            {
                SCREEN_MANAGER.goto_screen("MainMenuScreen");
            };
            //button_start.MouseClick += (sender, e) =>
            //{
            //    if (map == null)
            //    {
            //        return;
            //    }
            //    sessiondata = new SessionData();
            //    sessiondata.map = new Map();
            //    sessiondata.map.Clone(Storage.MapData.LoadMap(mapdata));
            //    sessiondata.gameMode = GameMode.campaign;
            //    sessiondata.playerInfos = new PlayerInfo[2];
            //    sessiondata.playerInfos[0] = new PlayerInfo(0, Owner.Red);
            //    sessiondata.playerInfos[1] = new PlayerInfo(1, Owner.Blue);
            //    ((GameScreen)SCREEN_MANAGER.get_screen("GameScreen")).InitSession(sessiondata);
            //    SCREEN_MANAGER.goto_screen("GameScreen");
            //};
            //Intial all event for connect to server
            button_connect.MouseClick += (sender, e) =>
            {
                Player.Instance.ConnectToServer(ip_address.Text);               
            };
            //add to canvas
           
            //canvas.AddElement("button_selectmap", button_selectmap);
            canvas.AddElement("button_exit", button_exit);
            //canvas.AddElement("button_start", button_start);
            canvas.AddElement("ip_address", ip_address);
            canvas.AddElement("button_connect", button_connect);
            canvas.AddElement("enter_name", enter_name);
        }

       

       

        public override void Shutdown()
        {
            sessiondata.playerInfos = null;
            sessiondata.map = null;
            map = null;
            minimap?.Dispose();
            minimap = null;
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
        }

        public override void Draw(GameTime gameTime)
        {
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
            if (minimap != null)
            {
                CONTENT_MANAGER.spriteBatch.Draw(minimap, new Vector2(100, 100), Color.White);
            }
        }
    }


}
