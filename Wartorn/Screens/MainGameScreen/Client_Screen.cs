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

        InputDialog enterRoom;

        Canvas canvas;



        public Client_Screen(GraphicsDevice device, string name) : base(device, name)
        {

        }


        public override bool Init()
        {
            enterRoom = new InputDialog();
            canvas = new Canvas();
            InitUI();
            return base.Init();
        }

        private void InitUI()
        {
            Button btn_create_room = new Button("Create Room", new Point(275, 150), new Vector2(170, 25), CONTENT_MANAGER.arcadefont);

            Button btn_goto_room = new Button("Go To Room", new Point(285, 200), new Vector2(150, 25), CONTENT_MANAGER.arcadefont);


            //bind event

            //Click to button go to room
            btn_goto_room.MouseClick += (sender, e) =>
            {
                enterRoom.ShowDialog();
            };

            //Click to button create room
            btn_create_room.MouseClick += (sender, e) =>
            {
                Player.Instance.CreateRoom();

            };

            //Click to ok button after enter room number 
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

            //Cancel go to room
            enterRoom.button_Cancel.Click += (sender, e) =>
            {
                enterRoom.Hide();
            };

            //Event enter success
            Player.Instance.entered_succeed += (sender, e) =>
            {
                SCREEN_MANAGER.goto_screen("Room_Screen");
            };

            //Event create room success
            Player.Instance.created_room += (_sender, _e) =>
            {
                SCREEN_MANAGER.goto_screen("Room_Screen");
            };

            canvas.AddElement("btn_create_room", btn_create_room);
            canvas.AddElement("btn_gpto_room", btn_goto_room);

        }

        public override void Update(GameTime gameTime)
        {
            if (canvas == null)
            {
                return;
            }
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
        }

        public override void Draw(GameTime gameTime)
        {
            if (canvas == null)
            {
                return;
            }
            canvas.Draw(CONTENT_MANAGER.spriteBatch);

        }
        public override void Shutdown()
        {
            base.Shutdown();

        }
    }
}
