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
using Newtonsoft.Json;
using System.IO;

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
        private Rectangle mapArea;

        private SpriteSheetTerrain currentlySelectedTerrain = (SpriteSheetTerrain)1;

        //maybe use stack to save last action
        struct Action
        {
            public Point selectedMapCell;
            public SpriteSheetTerrain selectedMapCellTerrain;

            public Action(Point p,SpriteSheetTerrain t)
            {
                selectedMapCell = p;
                selectedMapCellTerrain = t;
            }
        }
        Stack<Action> undostack;

        public EditorScreen(GraphicsDevice device) : base(device, "EditorScreen")
        {
            map = new Map(50, 30);
            mapArea = new Rectangle(0, 0, (int)(map.Width*mapcellsize.X), (int)(map.Height*mapcellsize.Y));
            canvas = new Canvas();
            camera = new Camera(_device.Viewport);
            camera.Location -= offset;

            undostack = new Stack<Action>();

            InitUI();
            InitMap();
        }

        private void InitUI()
        {
            //declare ui element
            Button button_Undo = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Undo), new Point(20, 20), 0.5f);
            Button button_Save = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Save), new Point(50, 20), 0.5f);

            Label label_info = new Label("0", new Point(20, 100), new Vector2(100, 20), CONTENT_MANAGER.defaultfont);
            Label label_fps = new Label(" ", new Point(1, 1), new Vector2(50, 20), CONTENT_MANAGER.defaultfont);

            //bind action to ui event
            button_Undo.MouseClick += delegate (object sender, UIEventArgs e)
            {
                if (undostack.Count>0)
                {
                    var lastaction = undostack.Pop();
                    map[lastaction.selectedMapCell].terrain = lastaction.selectedMapCellTerrain;
                }
            };

            button_Save.MouseClick += delegate (object sender, UIEventArgs e)
            {
                var savedata = MapData.SaveMap(map);
                try
                {
                    File.WriteAllText(@"map/" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm") + ".map", savedata);
                }
                catch (Exception er)
                {
                    Utility.HelperFunction.Log(er);
                }
            };

            //add ui element to canvas
            canvas.AddElement("button_Undo", button_Undo);
            canvas.AddElement("button_Save", button_Save);
            canvas.AddElement("label_info", label_info);
            canvas.AddElement("label_fps", label_fps);
        }

        private void InitMap()
        {
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    map[i, j] = new MapCell(SpriteSheetTerrain.Sea);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);

            var mouseInputState = CONTENT_MANAGER.inputState.mouseState;
            var lastMouseInputState = CONTENT_MANAGER.lastInputState.mouseState;
            var keyboardInputState = CONTENT_MANAGER.inputState.keyboardState;
            var lastKeyboardInputState = CONTENT_MANAGER.lastInputState.keyboardState;

            //check if mouse click
            if (mouseInputState.LeftButton == ButtonState.Pressed)
            {
                //check mouse pos
                var mouseLocationInMap = camera.TranslateFromScreenToWorld(mouseInputState.Position.ToVector2());
                
                if (mapArea.Contains(mouseLocationInMap))
                {
                    //check if not any cell is seleted
                    if (selectedMapCell != null)
                    {
                        if (map[selectedMapCell].terrain != currentlySelectedTerrain)
                        {
                            undostack.Push(new Action(selectedMapCell, map[selectedMapCell].terrain));
                            map[selectedMapCell].terrain = currentlySelectedTerrain;
                        }
                    }
                }
            }

            //rotate through terrain sprite
            if (HelperFunction.IsKeyPress(Keys.E))
            {
                if (currentlySelectedTerrain.CompareTo((SpriteSheetTerrain)((int)SpriteSheetTerrain.Max-1))<0)
                {
                    currentlySelectedTerrain = (SpriteSheetTerrain)((int)currentlySelectedTerrain + 1);
                }
            }
            if (HelperFunction.IsKeyPress(Keys.Q))
            {
                if (currentlySelectedTerrain.CompareTo((SpriteSheetTerrain)((int)SpriteSheetTerrain.Min+1)) > 0)
                {
                    currentlySelectedTerrain = (SpriteSheetTerrain)((int)currentlySelectedTerrain - 1);
                }
            }

            //simulate scrolling
            if (keyboardInputState.IsKeyDown(Keys.Left))
            {
                camera.Location += new Vector2(-1, 0) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Right))
            {
                camera.Location += new Vector2(1, 0) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Up))
            {
                camera.Location += new Vector2(0, -1) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Down))
            {
                camera.Location += new Vector2(0, +1) * 10;
            }

            int mouseScrollAmount = GetMouseScrollAmount();
            if ((mouseScrollAmount > 0) && (camera.Zoom < 1))
            {
                camera.Zoom += 0.1f;
            }

            if ((mouseScrollAmount < 0) && (camera.Zoom > 0.3f))
            {
                camera.Zoom -= 0.1f;
            }


            selectedMapCell = TranslateMousePosToMapCellPos(CONTENT_MANAGER.inputState.mouseState.Position);

            int frameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            ((Label)canvas["label_fps"]).Text = frameRate.ToString();
        }

        private Point TranslateMousePosToMapCellPos(Point mousepos)
        {
            //calculate currently selected mapcell
            Vector2 temp = camera.TranslateFromScreenToWorld(mousepos.ToVector2());
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
                    spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * mapcellsize.X, j * mapcellsize.Y), SpriteSheetSourceRectangle.GetSpriteRectangle(map[i, j].terrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.Terrain);
                }
            }

            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * mapcellsize.X, selectedMapCell.Y * mapcellsize.Y), new Rectangle(0, 0, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.Gui);
            //end this batch
            spriteBatch.End();
            //start a new batch for whatever come after
            spriteBatch.Begin(SpriteSortMode.FrontToBack);
            spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(0,50), SpriteSheetSourceRectangle.GetSpriteRectangle(currentlySelectedTerrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.Terrain);
        }
    }
}
