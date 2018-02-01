
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Wartorn;
using Wartorn.ScreenManager;
using Wartorn.Screens;
using Wartorn.GameData;
using Wartorn.Utility;
using Wartorn.UIClass;

namespace Wartorn
{
    namespace Screens.MainGameScreen
    {
        public class EndGameScreen : Screen
        {
            SessionData sessiondata;
            Owner winner;
            Texture2D endgameBG;

            Canvas canvas;
            
            public EndGameScreen(GraphicsDevice device): base(device, "EndGameScreen")
            {
                LoadContent();
            }

            public void LoadContent()
            {
                endgameBG = CONTENT_MANAGER.Sprites.Where(x=>x.Key.Contains( "endgame")).PickRandom().Value;
            }

            public override bool Init()
            {
                InitUI();
                return true;
            }

            private void InitUI()
            {
                canvas = new Canvas();
                Label label_whowin = new Label(" ", new Point(720 / 2 - 125, 480 / 2 - 30), null, CONTENT_MANAGER.Fonts["hackfont"], 2);
                label_whowin.Origin = new Vector2(1, 1);
                label_whowin.Text = winner + " team won!";
                if (winner == Owner.Red)
                {
                    label_whowin.ForegroundColor = Color.Red;
                }
                else
                {
                    label_whowin.ForegroundColor = Color.Blue;
                }


                Button button_mainmenu = new Button("Return to Menu", new Point(0, 0), new Vector2(100,30), CONTENT_MANAGER.Fonts["hackfont"]);
                //button_mainmenu.Origin = new Vector2(15, 0);
                button_mainmenu.MouseClick += (sender, e) =>
                {
                    SCREEN_MANAGER.goto_screen("MainMenuScreen");
                };



                canvas.AddElement("label_whowin", label_whowin);
                canvas.AddElement("button_mainmenu", button_mainmenu);
            }

            public void InitEndGameScreen(SessionData sessiondata)
            {
                this.sessiondata = sessiondata;
                for (int i=0;i<sessiondata.playerInfos.GetLength(0);i++)
                {
                    if (sessiondata.map[sessiondata.playerInfos[i].HQlocation].owner == sessiondata.playerInfos[i].owner)
                    {
                        winner = sessiondata.playerInfos[i].owner;
                        break;
                    }
                }
            }

            public override void Update(GameTime gameTime)
            {
                canvas.Update(gameTime,CONTENT_MANAGER.currentInputState, CONTENT_MANAGER.lastInputState);
            }

            public override void Draw(GameTime gameTime)
            {
                CONTENT_MANAGER.spriteBatch.Draw(endgameBG, new Vector2(0, 0), Color.White);
                canvas.Draw(CONTENT_MANAGER.spriteBatch,gameTime);
            }
        }
    }
}