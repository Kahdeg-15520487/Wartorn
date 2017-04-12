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
    class EditorScreen : Screen
    {
        private Map map;
        private Canvas canvas;
        private Camera camera;

        private Point selectedMapCell = new Point(0, 0);
        private Vector2 offset = new Vector2(70, 70);
        private Vector2 mapcellsize = new Vector2(48, 48);

        private Terrain currentlySelectedTerrain = Terrain.Plain;

        public EditorScreen(GraphicsDevice device) : base(device, "EditorScreen")
        {
            map = new Map();
            canvas = new Canvas();
            camera = new Camera(_device.Viewport);
            InitUI();
            InitMap();
        }

        private void InitUI()
        {
            //declare ui element
            Button button_airport = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.AirPort), new Point(20, 20), 0.5f);
            button_airport.Text = Terrain.AirPort.ToString();
            Label label_info = new Label("0", new Point(20, 100), new Vector2(100, 20), CONTENT_MANAGER.defaultfont);
            Label label_fps = new Label(" ", new Point(1, 1), new Vector2(50, 20), CONTENT_MANAGER.defaultfont);

            //bind action to ui event
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
            canvas.AddElement("label_fps", label_fps);
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
                camera.Location += new Vector2(-1, 0) * 10;
            }
            if (Utility.HelperFunction.IsKeyPress(Keys.Right))
            {
                camera.Location += new Vector2(1, 0) * 10;
            }
            if (Utility.HelperFunction.IsKeyPress(Keys.Up))
            {
                camera.Location += new Vector2(0, -1) * 10;
            }
            if (Utility.HelperFunction.IsKeyPress(Keys.Down))
            {
                camera.Location += new Vector2(0, +1) * 10;
            }

            int mouseScrollAmount = GetMouseScrollAmount();
            if ((mouseScrollAmount > 0) && (camera.Zoom < 1))
            {
                camera.Zoom += 0.1f;
            }

            if ((mouseScrollAmount < 0) && (camera.Zoom > 0.1f))
            {
                camera.Zoom -= 0.1f;
            }


            ((Label)canvas["label_info"]).Text = camera.Zoom.ToString();

            selectedMapCell = TranslateMousePosToMapCellPos(CONTENT_MANAGER.inputState.mouseState.Position);

            int frameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            ((Label)canvas["label_fps"]).Text = frameRate.ToString();
        }

        private Point TranslateMousePosToMapCellPos(Point mousepos)
        {
            //calculate currently selected mapcell
            Vector2 temp = camera.TranslateFromScreenToWorld(mousepos.ToVector2());
            temp -= offset;
            temp.X = (int)(temp.X / mapcellsize.X);       //mapcell size
            temp.Y = (int)(temp.Y / mapcellsize.Y);

            if (temp.X >= 0 && temp.X < map.Width && temp.Y >= 0 && temp.Y < map.Height)
                return temp.ToPoint();
            return Point.Zero;
        }

        private int GetMouseScrollAmount()
        {
            return CONTENT_MANAGER.inputState.mouseState.ScrollWheelValue - CONTENT_MANAGER.lastInputState.mouseState.ScrollWheelValue;
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
            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * mapcellsize.X, j * mapcellsize.Y) + offset, SpriteSheetSourceRectangle.GetSpriteRectangle(map[i, j].terrain.ToString()), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.Terrain);
                }
            }

            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * mapcellsize.X, selectedMapCell.Y * mapcellsize.Y) + offset, new Rectangle(0, 0, 60, 60), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.Gui);
            //end this batch
            spriteBatch.End();
            //start a new batch for whatever come after
            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
