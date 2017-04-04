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

        private Texture2D MenuBackground;
        private Texture2D TitleBackground;
        private SpriteFont menufont;

        public MainMenuScreen(GraphicsDevice device) : base(device, "MainMenuScreen")
        {
            MenuBackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Menuscreen");
            TitleBackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Title");
            menufont = CONTENT_MANAGER.Content.Load<SpriteFont>(@"sprite\GUI\menufont");

            backgroundCamera = new Camera(_device.Viewport);

            canvas = new Canvas();
            InitUI();
        }

        private void InitUI()
        {
            //declare ui element
            Label labelfps = new Label(" ",new Point(50,50),new Vector2(100,50),menufont);
            labelfps.foregroundColor = Color.DarkBlue;

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

            ((Label)canvas["labelfps"]).Text = "lala";
        }

        public override void Draw(GameTime gameTime)
        {
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
            //DrawMenuBackground(CONTENT_MANAGER.spriteBatch);
        }

        private Rectangle offset = new Rectangle(0, 0, 720, 480);
        private void DrawMenuBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: backgroundCamera.TransformMatrix);
            {
                spriteBatch.Draw(MenuBackground, offset, offset, Color.White, 0f, Vector2.Zero, SpriteEffects.None, LayerDepth.BackGround);
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
