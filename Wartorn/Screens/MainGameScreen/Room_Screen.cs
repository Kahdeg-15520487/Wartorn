using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wartorn.ScreenManager;
using Wartorn.GameData;
using Wartorn.Drawing;
using Wartorn.UIClass;
using System.IO;
using Client;
using Newtonsoft.Json;
using System.Timers;
using Wartorn.Storage;

namespace Wartorn.Screens.MainGameScreen
{
    public class Room_Screen : Screen
    {
       

        SessionData sessiondata;

        Map map = null;

        MiniMapGenerator minimapgen;

        Texture2D minimap;

        Canvas canvas;

        string mapdata;

        string loadPath="";
        //Use if you are host, you can create room
        bool isLoadMap = false;
        /// <summary>
        /// true if player is host
        /// </summary>
     
        bool is_another_ready = false;

        //true if this player is ready
        bool is_this_ready = false;
               
        public override bool Init()
        {

            minimapgen = new MiniMapGenerator(_device, CONTENT_MANAGER.spriteBatch);

            sessiondata = new SessionData();

            canvas = new Canvas();

            InitUI();

            //Every 2 second check if two players are ready


            return base.Init();
        }




        //Load map with specific path
        private void LoadMap(string path)
        {
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
        }

        //go to game if both player ready
        //map can be load in two case: first if you are host, you choose map from your computer. 
        //If you are't host, you must wait for host load map and send for you
        private void GoToGame()
        {
            if (map != null && is_another_ready && is_this_ready )
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
        }

        //Notify another phayer that you are ready
        //( This method only use to phayer is no host )

        private void InitUI()
        {

            Button button_ready = new Button("Ready", new Point(127, 200), new Vector2(100, 25), CONTENT_MANAGER.arcadefont);

            Button button_selectmap = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Open), new Point(650, 20), 0.5f);

            Label label_this_ready = new Label("", new Point(50, 50), new Vector2(100, 25), CONTENT_MANAGER.arcadefont);

            Label label_another_ready = new Label("", new Point(490, 227), new Vector2(100, 25), CONTENT_MANAGER.arcadefont);

            Button separate = new Button("", new Point(355, 0), new Vector2(10, 480), CONTENT_MANAGER.arcadefont);

            Label player_name = new Label(PlayerName.Name, new Point(130, 0), new Vector2(100, 25), CONTENT_MANAGER.arcadefont);

            Label another_player_name = new Label("", new Point(490, 0), new Vector2(100, 25), CONTENT_MANAGER.arcadefont);

            // Bind event


            Player.Instance.another_goto_room += (sender, e) =>
            {
                another_player_name.Text = e;
            };

            //Khi chủ phòng load map và gửi cho người chơi khác
            //When select map
            button_selectmap.MouseClick += (sender, e) =>
             {
                 try
                 {
                     //If you are host, you can load map
                     if (Player.Instance.isHost)
                     {

                         string path = CONTENT_MANAGER.ShowFileOpenDialog(CONTENT_MANAGER.LocalRootPath);

                         LoadMap(path);


                         //Send map to another phayer
                         string send = Path.GetFileName(path);
                         Player.Instance.Update(send);
                     }
                 }
                 catch (Exception er)
                 {

                     Utility.HelperFunction.Log(er);
                     CONTENT_MANAGER.ShowMessageBox(er.Message);
                 }

             };


            //Người chơi khác nhận map 
            //Load map which host had been chosen
            Player.Instance.update += (sender, e) =>
            {
                
                string path = CONTENT_MANAGER.LocalRootPath + "\\map\\" + e;
                if (File.Exists(path))
                {

                    loadPath = path;
                    isLoadMap = true;
                }

            };

            //Event raise when another phayer ready
            Player.Instance.received_chat += (sender, e) =>
            {
                if (e == "Ready")
                {
                    is_another_ready = true;
                    label_another_ready.Text = "Ready";
                    
                }
                else
                {
                    is_another_ready = false;
                    label_another_ready.Text = "NotReady";
                }
            };


            //Player is ready
            button_ready.MouseClick += (sender, e) =>
            {
                if (Player.Instance.isHost)
                {
                    if (map != null && is_another_ready)
                    {
                        is_this_ready = true;
                        Player.Instance.ChatWithAnother("Ready");
                    }
                }
                else
                {
                    //Tell another phayer, you start
                    if (!is_this_ready)
                    {
                        is_this_ready = true;
                        label_this_ready.Text = "Ready";
                        Player.Instance.ChatWithAnother("Ready");

                    }
                    else
                    {
                        is_this_ready = false;
                        label_this_ready.Text = "";
                        Player.Instance.ChatWithAnother("NotReady");

                    }
                }
                

            };

            //Add to canvas to draw to screen
            canvas.AddElement("button_selectmap", button_selectmap);
            canvas.AddElement("button_ready", button_ready);
            canvas.AddElement("label_this_ready", label_this_ready);
            canvas.AddElement("separate", separate);
        }

        //Contructor
        public Room_Screen(GraphicsDevice device, string name) : base(device, name)
        {

        }

        //Update frames per second
        public override void Update(GameTime gameTime)
        {

            if (canvas == null)
            {
                return;
            }
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);

            if (isLoadMap)
            {
                LoadMap(loadPath);
                
            }

            GoToGame();
           
        }

        //Draw frames per second
        public override void Draw(GameTime gameTime)
        {

            if (canvas == null)
            {
                return;
            }
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
            if (minimap != null)
            {
                CONTENT_MANAGER.spriteBatch.Draw(minimap,new Rectangle(new Point(310,190),new Point(100,100)), Color.White);

            }
          
           
        }


    }
}
