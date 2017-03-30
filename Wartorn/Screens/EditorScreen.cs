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

namespace Wartorn.Screens
{
    class EditorScreen : Screen
    {
        private Map map;
        private Canvas canvas;
        private Vector3 viewport = new Vector3(0, 0, 0);
        private Point selectedMapCell = new Point(0, 0);
        private Vector2 offset = new Vector2(70, 70);

        public EditorScreen(GraphicsDevice device) : base(device, "EditorScreen")
        {
            map = new Map();
            canvas = new Canvas();
            InitUI();
            InitMap();
        }

        private void InitUI()
        {
            //declare ui element
            Button button_airport = new Button(SpriteSheetSourceRectangle.AirPort, new Point(20, 20), 0.5f);
            Label label_info = new Label("0", new Point(20, 100), new Vector2(100, 20), CONTENT_MANAGER.defaultfont);

            //bind action to event
            button_airport.MouseClick += delegate (object sender, UIEventArgs e)
            {
                int temp;
                if (int.TryParse(label_info.Text, out temp))
                {
                    label_info.Text = (temp + 1).ToString();
                }
                else
                {
                    label_info.Text = "0";
                }
            };

            //add ui element to canvas
            canvas.AddElement("button_airport", button_airport);
            canvas.AddElement("label_info", label_info);
        }

        private void InitMap()
        {
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    map[i, j] = new MapCell(Terrain.Plain);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);

            //simulate scrolling
            if (Utility.HelperFunction.IsKeyPress(Keys.Left))
            {
                viewport += Vector3.Left * 10;
            }
            if (Utility.HelperFunction.IsKeyPress(Keys.Right))
            {
                viewport += Vector3.Right * 10;
            }
            if (Utility.HelperFunction.IsKeyPress(Keys.Up))
            {
                viewport -= Vector3.Up * 10;
            }
            if (Utility.HelperFunction.IsKeyPress(Keys.Down))
            {
                viewport -= Vector3.Down * 10;
            }

            //calculate currently selected mapcell
            Vector2 temp = CONTENT_MANAGER.inputState.mouseState.Position.ToVector2();
            temp -= offset;
            temp -= new Vector2(viewport.X,viewport.Y);

            temp.X = (int)(temp.X / 30f);
            temp.Y = (int)(temp.Y / 30f);

            if (temp.X>=0 && temp.X<map.Width && temp.Y>=0 && temp.Y<map.Height)
                selectedMapCell = temp.ToPoint();

            ((Label)canvas["label_info"]).Text = selectedMapCell.ToString();
        }

        public override void Draw(GameTime gameTime)
        {
            DrawMap(CONTENT_MANAGER.spriteBatch);
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
        }

        private void DrawMap(SpriteBatch spriteBatch)
        {
            //end that batch since the map will be render diffrently
            spriteBatch.End();
            //begin a new batch with translated matrix to simulate scrolling
            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: Matrix.CreateTranslation(viewport));
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * 30, j * 30) + offset, SpriteSheetSourceRectangle.GetSpriteRectangle(map[i, j].terrain), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, LayerDepth.Terrain);
                }
            }

            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * 30, selectedMapCell.Y * 30) + offset, new Rectangle(0, 0, 60, 60), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, LayerDepth.Gui);
            //end this batch
            spriteBatch.End();
            //start a new batch for whatever come after
            spriteBatch.Begin(SpriteSortMode.BackToFront);
        }
    }
}
