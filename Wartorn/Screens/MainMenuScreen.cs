using System;
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
using Wartorn.Drawing;


namespace Wartorn.Screens
{
    class MainMenuScreen : Screen
    {
        private Canvas canvas;
        private Camera backgroundCamera;

        private Texture2D SelectScreenBackground;
        private Texture2D TitleBackground;
        private SpriteFont menufont;

        private int maxXoffset, maxYoffset;

        public MainMenuScreen(GraphicsDevice device) : base(device, "MainMenuScreen")
        {
            LoadContent();
            backgroundCamera = new Camera(_device.Viewport);

            canvas = new Canvas();
            InitUI();
        }

        private void LoadContent()
        {
            SelectScreenBackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Select_screen");
            maxXoffset = 1000;
            maxYoffset = 1000;

            TitleBackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Title");
            menufont = CONTENT_MANAGER.Content.Load<SpriteFont>(@"sprite\GUI\menufont");
        }

        private void InitUI()
        {
            //declare ui element
            Label labelfps = new Label(" ",new Point(50,50),new Vector2(100,50),CONTENT_MANAGER.defaultfont);
            labelfps.foregroundColor = Color.DarkBlue;

            //TODO make button for main menu
            //Button buttonCampaign = new Button(@"sprite\GUI\")

            //bind action to ui event

            //add ui element to canvas
            canvas.AddElement("labelfps",labelfps);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);

            ((Label)canvas["labelfps"]).Text = maxXoffset.ToString() + " " + offset.Size.X.ToString() + '\n' + maxYoffset.ToString() + " " + offset.Size.Y.ToString();

            if (Utility.HelperFunction.IsKeyPress(Keys.A))
            {
                SCREEN_MANAGER.goto_screen("EditorScreen");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            DrawMenuBackground(CONTENT_MANAGER.spriteBatch);
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
        }

        private Rectangle offset = new Rectangle(0, 0, 720, 480);
        private Point moveoffset = new Point(1, 1);
        private void DrawMenuBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: backgroundCamera.TransformMatrix);
            {
                spriteBatch.Draw(SelectScreenBackground, new Rectangle(0,0,720,480), offset, Color.White, 0f, Vector2.Zero, SpriteEffects.None, LayerDepth.BackGround);

                //if (Utility.HelperFunction.IsKeyPress(Keys.P))
                {
                    if ((offset.Size.X > maxXoffset || offset.Size.Y > maxYoffset)
                      || (offset.Location.X < 0 || offset.Location.Y < 0))
                    {
                        moveoffset = new Point(-moveoffset.X, -moveoffset.Y);
                    }
                    offset.Location = new Point(offset.Location.X + moveoffset.X, offset.Location.Y + moveoffset.Y);
                    offset.Size = new Point(offset.Size.X + moveoffset.X, offset.Size.Y + moveoffset.Y);
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
