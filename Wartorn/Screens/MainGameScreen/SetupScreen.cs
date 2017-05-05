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
        SessionData sessiondata;
        Canvas canvas;

        Map map = null;

        MiniMapGenerator minimapgen;
        Texture2D minimap;

        public SetupScreen(GraphicsDevice device) : base(device, "SetupScreen")
        {

        }

        public override bool Init()
        {
            canvas = new Canvas();
            sessiondata = new SessionData();

            InitUI();

            minimapgen = new MiniMapGenerator(_device, CONTENT_MANAGER.Content, CONTENT_MANAGER.spriteBatch);

            return base.Init();
        }

        private void InitUI()
        {
            //declare ui elements
            Label label_playerinfo = new Label("kahdeg", new Point(10, 20), new Vector2(80, 30), CONTENT_MANAGER.arcadefont);

            Button button_selectmap = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Open), new Point(650, 20), 0.5f);
            Button button_exit = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Exit), new Point(5, 5), 0.5f);
            Button button_start = new Button("Start", new Point(100,50), new Vector2(80, 30), CONTENT_MANAGER.arcadefont);

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
                SCREEN_MANAGER.go_back();
            };
            button_start.MouseClick += (sender, e) =>
            {
                if (map == null)
                {
                    return;
                }
                sessiondata = new SessionData();
                sessiondata.map = map;
                sessiondata.gameMode = GameMode.campaign;
                sessiondata.playerId = new PlayerInfo[2];
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
            base.Shutdown();
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
