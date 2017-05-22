using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wartorn.ScreenManager;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Drawing;
using System.IO;
using Client;

namespace Wartorn.Screens.MainGameScreen
{
    class Client_Screen : Screen
    {
        private bool isCreated_room;

        InputDialog enterRoom;
        SessionData sessiondata;
        Canvas canvas;

        Map map = null;

        MiniMapGenerator minimapgen;

        Texture2D minimap;

        string mapdata;

        public Client_Screen(GraphicsDevice device, string name) : base(device, name)
        {

        }


        public override bool Init()
        {

            isCreated_room = false;

            enterRoom = new InputDialog();



            canvas = new Canvas();



            sessiondata = new SessionData();

            InitUI();

            minimapgen = new MiniMapGenerator(_device, CONTENT_MANAGER.spriteBatch);

            return base.Init();
        }

        private void InitUI()
        {
            Button button_selectmap = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Open), new Point(650, 20), 0.5f);

            Button button_start = new Button("Start", new Point(100, 50), null, CONTENT_MANAGER.arcadefont);

            Button btn_create_room = new Button("Create Room", new Point(50, 50), null, CONTENT_MANAGER.arcadefont);

            Button btn_goto_room = new Button("Go To Room", new Point(50, 100), null, CONTENT_MANAGER.arcadefont);

            InputBox roomNumber = new InputBox("", new Point(150, 100), new Vector2(150, 20), CONTENT_MANAGER.arcadefont, Color.Black, Color.White);


            //bind event

            btn_goto_room.MouseClick += (sender, e) =>
            {
                enterRoom.ShowDialog();
            };

            enterRoom.button_OK.Click += (sender, e) =>
            {
                if (enterRoom.textBox_Input.Text != "")
                {

                    Player.Instance.GotoRoom(Convert.ToInt32(enterRoom.textBox_Input.Text));

                }
                else
                {
                    CONTENT_MANAGER.ShowMessageBox("You have't enter room number");
                }
            };

            enterRoom.button_Cancel.Click += (sender, e) =>
            {
                enterRoom.Hide();
            };


            Player.Instance.entered_succeed += (sender, e) =>
            {
                
            };

            btn_create_room.MouseClick += (sender, e) =>
            {
                if (map != null)
                {
                    Player.Instance.created_room += (_sender, _e) =>
                    {

                    };
                    isCreated_room = true;
                    Player.Instance.CreateRoom();
                }
                else
                {
                    CONTENT_MANAGER.ShowMessageBox("You don't have map");
                }
            };


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


            button_start.MouseClick += (sender, e) =>
            {
                if (map != null && isCreated_room == true)
                {
                    sessiondata = new SessionData();
                    sessiondata.map = new Map();
                    sessiondata.map.Clone(Storage.MapData.LoadMap(mapdata));
                    sessiondata.gameMode = GameMode.campaign;
                    sessiondata.playerInfos = new PlayerInfo[2];
                    sessiondata.playerInfos[0] = new PlayerInfo(0, Owner.Red);
                    sessiondata.playerInfos[1] = new PlayerInfo(1, Owner.Blue);
                    ((GameScreen)SCREEN_MANAGER.get_screen("GameScreen")).InitSession(sessiondata);
                    SCREEN_MANAGER.goto_screen("GameScreen");
                }

            };



            canvas.AddElement("button_selectmap", button_selectmap);
            canvas.AddElement("button_start", button_start);
            canvas.AddElement("btn_create_room", btn_create_room);
            canvas.AddElement("btn_gpto_room", btn_goto_room);
            canvas.AddElement("roomNumber", roomNumber);
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
        public override void Shutdown()
        {
            base.Shutdown();

        }
    }
}
