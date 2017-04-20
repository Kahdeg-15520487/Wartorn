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
    enum Side
    {
        Left, Right
    }

    class EditorScreen : Screen
    {
        private Map map;
        private Canvas canvas;
        private Camera camera;

        private Texture2D showtile;

        private Point selectedMapCell = new Point(0, 0);
        private Vector2 offset = new Vector2(70, 70);
        private Vector2 mapcellsize = new Vector2(48, 48);
        private Rectangle mapArea;

        private SpriteSheetTerrain currentlySelectedTerrain = SpriteSheetTerrain.Reef;

        private Side GuiSide = Side.Left;
        private bool isMenuOpen = true;

        //represent a changing map cell action
        struct Action
        {
            public Point selectedMapCell;
            public SpriteSheetTerrain selectedMapCellTerrain;

            public Action(Point p, SpriteSheetTerrain t)
            {
                selectedMapCell = p;
                selectedMapCellTerrain = t;
            }
        }
        Stack<Action> undostack;

        public EditorScreen(GraphicsDevice device) : base(device, "EditorScreen")
        {
            LoadContent();

            map = new Map(50, 30);
            mapArea = new Rectangle(0, 0, (int)(map.Width * mapcellsize.X), (int)(map.Height * mapcellsize.Y));
            canvas = new Canvas();
            camera = new Camera(_device.Viewport);
            //camera.Location -= offset;

            undostack = new Stack<Action>();

            InitUI();
            InitMap();
        }

        private void LoadContent()
        {
            showtile = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\showtile");
        }

        #region InitUI
        private void InitUI()
        {
            //declare ui element

            //escape menu
            Canvas canvas_Menu = new Canvas();
            Button button_Undo = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Undo), new Point(20, 20), 0.5f);
            Button button_Save = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Save), new Point(50, 20), 0.5f);
            Button button_Open = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Open), new Point(80, 20), 0.5f);
            Button button_Exit = new Button(UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Exit), new Point(110, 20), 0.5f);
            canvas_Menu.AddElement("button_Undo", button_Undo);
            canvas_Menu.AddElement("button_Save", button_Save);
            canvas_Menu.AddElement("button_Open", button_Open);
            canvas_Menu.AddElement("button_Exit", button_Exit);
            canvas_Menu.IsVisible = isMenuOpen;

            //terrain selection menu
            Canvas canvas_terrain_selection = new Canvas();


            Label label1 = new Label("Hor" + Environment.NewLine + "Ver", new Point(0, 0), new Vector2(30, 20), CONTENT_MANAGER.defaultfont);
            label1.Scale = 1.2f;
            Label label_Horizontal = new Label("1", new Point(40, 0), new Vector2(20, 20), CONTENT_MANAGER.defaultfont);
            label_Horizontal.Scale = 1.2f;
            Label label_Vertical = new Label("1", new Point(40, 20), new Vector2(20, 20), CONTENT_MANAGER.defaultfont);
            label_Vertical.Scale = 1.2f;
            //label1.foregroundColor = Color.White;

            //bind action to ui event
            button_Undo.MouseClick += (sender, e) =>
            {
                if (undostack.Count > 0)
                {
                    var lastaction = undostack.Pop();
                    map[lastaction.selectedMapCell].terrain = lastaction.selectedMapCellTerrain;
                }
            };

            button_Save.MouseClick += (sender, e) =>
            {
                var savedata = MapData.SaveMap(map);
                try
                {
                    File.WriteAllText(@"map/" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm") + ".map", savedata);
                }
                catch (Exception er)
                {
                    HelperFunction.Log(er);
                }
            };

            button_Open.MouseClick += (sender, e) =>
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
                    if (temp!=null)
                    {
                        map.Clone(temp);
                    }
                }
            };

            button_Exit.MouseClick += (sender, e) =>
            {
                Environment.Exit(0);
            };

            //add ui element to canvas
            canvas.AddElement("canvas_Menu", canvas_Menu);

            canvas.AddElement("label1", label1);
            canvas.AddElement("label_Horizontal", label_Horizontal);
            canvas.AddElement("label_Vertical", label_Vertical);
        }
        #endregion

        private void InitMap()
        {
            int count = 1;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    map[i, j] = new MapCell((SpriteSheetTerrain)count);
                    count += 1;
                    if (count == (int)(SpriteSheetTerrain.Max))
                    {
                        count = 1;
                    }
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

            selectedMapCell = TranslateMousePosToMapCellPos(CONTENT_MANAGER.inputState.mouseState.Position);
            ((Label)canvas.GetElement("label_Horizontal")).Text = (selectedMapCell.X + 1).ToString();
            ((Label)canvas.GetElement("label_Vertical")).Text = (selectedMapCell.Y + 1).ToString();

            OpenHideMenu();
            MoveGuiToAvoidMouse(mouseInputState);

            if (!isMenuOpen)
            {
                PlaceTile(mouseInputState);
                RotateThroughTerrain();
            }
            MoveCamera(keyboardInputState, mouseInputState);
            //ZoomCamera();
        }

        #region Update's sub method
        private void OpenHideMenu()
        {
            if (Utility.HelperFunction.IsKeyPress(Keys.Escape))
            {
                isMenuOpen = !isMenuOpen;
                ((Canvas)canvas.GetElement("canvas_Menu")).IsVisible = isMenuOpen;
            }
        }

        private void MoveGuiToAvoidMouse(MouseState mouseInputState)
        {
            //move gui around to avoid mouse
            if (mouseInputState.Position.X < 100 && GuiSide == Side.Left)
            {
                GuiSide = Side.Right;
                ((Label)canvas.GetElement("label1")).Position = new Point(670, 0);
                ((Label)canvas.GetElement("label_Horizontal")).Position = new Point(655, 0);
                ((Label)canvas.GetElement("label_Vertical")).Position = new Point(655, 20);
            }
            if (mouseInputState.Position.X > 620 && GuiSide == Side.Right)
            {
                GuiSide = Side.Left;
                ((Label)canvas.GetElement("label1")).Position = new Point(0, 0);
                ((Label)canvas.GetElement("label_Horizontal")).Position = new Point(40, 0);
                ((Label)canvas.GetElement("label_Vertical")).Position = new Point(40, 20);
            }
        }

        private void PlaceTile(MouseState mouseInputState)
        {

            var mouseLocationInMap = camera.TranslateFromScreenToWorld(mouseInputState.Position.ToVector2());
            //check if left mouse click
            if (mouseInputState.LeftButton == ButtonState.Pressed)
            {
                //check mouse pos
                if (mapArea.Contains(mouseLocationInMap))
                {
                    //check if not any cell is selected
                    if (selectedMapCell != null)
                    {
                        if (map[selectedMapCell].terrain != currentlySelectedTerrain)
                        {
                            undostack.Push(new Action(selectedMapCell, map[selectedMapCell].terrain));
                            map[selectedMapCell].terrain = currentlySelectedTerrain;
                            HandleMultiTileSprite(selectedMapCell, currentlySelectedTerrain);
                        }
                    }
                }
            }

            //check if right mouse click
            if (mouseInputState.RightButton == ButtonState.Pressed)
            {
                //check mouse pos
                if (mapArea.Contains(mouseLocationInMap))
                {
                    //check if not any cell is selected
                    if (selectedMapCell != null)
                    {
                        if (map[selectedMapCell].terrain != currentlySelectedTerrain)
                        {
                            currentlySelectedTerrain = map[selectedMapCell].terrain;
                        }
                    }
                }
            }
        }

        private void HandleMultiTileSprite(Point p,SpriteSheetTerrain t)
        {
            var mapcell = map[p];

            switch (t)
            {
                case SpriteSheetTerrain.Tree_4_1:
                    break;
                case SpriteSheetTerrain.Tree_4_2:
                    break;
                case SpriteSheetTerrain.Tree_4_3:
                    break;
                case SpriteSheetTerrain.Tree_4_4:
                    break;

                case SpriteSheetTerrain.Tree_9_1:
                    break;
                case SpriteSheetTerrain.Tree_9_2:
                    break;
                case SpriteSheetTerrain.Tree_9_3:
                    break;
                case SpriteSheetTerrain.Tree_9_4:
                    break;
                case SpriteSheetTerrain.Tree_9_5:
                    break;
                case SpriteSheetTerrain.Tree_9_6:
                    break;
                case SpriteSheetTerrain.Tree_9_7:
                    break;
                case SpriteSheetTerrain.Tree_9_8:
                    break;
                case SpriteSheetTerrain.Tree_9_9:
                    break;

                case SpriteSheetTerrain.Mountain_High_Upper:
                    break;
                case SpriteSheetTerrain.Mountain_High_Lower:
                    break;
                    
                case SpriteSheetTerrain.Tropical_Tree_4_1:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_4_2:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_4_3:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_4_4:
                    break;

                case SpriteSheetTerrain.Tropical_Tree_9_1:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_2:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_3:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_4:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_5:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_6:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_7:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_8:
                    break;
                case SpriteSheetTerrain.Tropical_Tree_9_9:
                    break;

                case SpriteSheetTerrain.Tropical_Mountain_High_Upper:
                    break;
                case SpriteSheetTerrain.Tropical_Mountain_High_Lower:
                    break;

                case SpriteSheetTerrain.Rain_Tree_4_1:
                    break;
                case SpriteSheetTerrain.Rain_Tree_4_2:
                    break;
                case SpriteSheetTerrain.Rain_Tree_4_3:
                    break;
                case SpriteSheetTerrain.Rain_Tree_4_4:
                    break;

                case SpriteSheetTerrain.Rain_Tree_9_1:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_2:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_3:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_4:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_5:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_6:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_7:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_8:
                    break;
                case SpriteSheetTerrain.Rain_Tree_9_9:
                    break;

                case SpriteSheetTerrain.Rain_Mountain_High_Upper:
                    break;
                case SpriteSheetTerrain.Rain_Mountain_High_Lower:
                    break;

                case SpriteSheetTerrain.Snow_Tree_4_1:
                    break;
                case SpriteSheetTerrain.Snow_Tree_4_2:
                    break;
                case SpriteSheetTerrain.Snow_Tree_4_3:
                    break;
                case SpriteSheetTerrain.Snow_Tree_4_4:
                    break;

                case SpriteSheetTerrain.Snow_Tree_9_1:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_2:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_3:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_4:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_5:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_6:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_7:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_8:
                    break;
                case SpriteSheetTerrain.Snow_Tree_9_9:
                    break;

                case SpriteSheetTerrain.Snow_Mountain_High_Upper:
                    break;
                case SpriteSheetTerrain.Snow_Mountain_High_Lower:
                    break;
                    
                case SpriteSheetTerrain.Desert_Mountain_High_Upper:
                    break;
                case SpriteSheetTerrain.Desert_Mountain_High_Lower:
                    break;

                case SpriteSheetTerrain.City_Upper:
                    break;
                case SpriteSheetTerrain.City_Lower:
                    break;
                case SpriteSheetTerrain.AirPort_Upper:
                    break;
                case SpriteSheetTerrain.AirPort_Lower:
                    break;
                case SpriteSheetTerrain.Harbor_Upper:
                    break;
                case SpriteSheetTerrain.Harbor_Lower:
                    break;
                case SpriteSheetTerrain.Radar_Upper:
                    break;
                case SpriteSheetTerrain.Radar_Lower:
                    break;
                case SpriteSheetTerrain.SupplyBase_Upper:
                    break;
                case SpriteSheetTerrain.SupplyBase_Lower:
                    break;

                case SpriteSheetTerrain.Red_City_Upper:
                    break;
                case SpriteSheetTerrain.Red_City_Lower:
                    break;
                case SpriteSheetTerrain.Red_AirPort_Upper:
                    break;
                case SpriteSheetTerrain.Red_AirPort_Lower:
                    break;
                case SpriteSheetTerrain.Red_Harbor_Upper:
                    break;
                case SpriteSheetTerrain.Red_Harbor_Lower:
                    break;
                case SpriteSheetTerrain.Red_Radar_Upper:
                    break;
                case SpriteSheetTerrain.Red_Radar_Lower:
                    break;
                case SpriteSheetTerrain.Red_SupplyBase_Upper:
                    break;
                case SpriteSheetTerrain.Red_SupplyBase_Lower:
                    break;
                case SpriteSheetTerrain.Red_Headquarter_Upper:
                    break;
                case SpriteSheetTerrain.Red_Headquarter_Lower:
                    break;

                case SpriteSheetTerrain.Blue_City_Upper:
                    break;
                case SpriteSheetTerrain.Blue_City_Lower:
                    break;
                case SpriteSheetTerrain.Blue_AirPort_Upper:
                    break;
                case SpriteSheetTerrain.Blue_AirPort_Lower:
                    break;
                case SpriteSheetTerrain.Blue_Harbor_Upper:
                    break;
                case SpriteSheetTerrain.Blue_Harbor_Lower:
                    break;
                case SpriteSheetTerrain.Blue_Radar_Upper:
                    break;
                case SpriteSheetTerrain.Blue_Radar_Lower:
                    break;
                case SpriteSheetTerrain.Blue_SupplyBase_Upper:
                    break;
                case SpriteSheetTerrain.Blue_SupplyBase_Lower:
                    break;
                case SpriteSheetTerrain.Blue_Headquarter_Upper:
                    break;
                case SpriteSheetTerrain.Blue_Headquarter_Lower:
                    break;

                case SpriteSheetTerrain.Green_City_Upper:
                    break;
                case SpriteSheetTerrain.Green_City_Lower:
                    break;
                case SpriteSheetTerrain.Green_AirPort_Upper:
                    break;
                case SpriteSheetTerrain.Green_AirPort_Lower:
                    break;
                case SpriteSheetTerrain.Green_Harbor_Upper:
                    break;
                case SpriteSheetTerrain.Green_Harbor_Lower:
                    break;
                case SpriteSheetTerrain.Green_Radar_Upper:
                    break;
                case SpriteSheetTerrain.Green_Radar_Lower:
                    break;
                case SpriteSheetTerrain.Green_SupplyBase_Upper:
                    break;
                case SpriteSheetTerrain.Green_SupplyBase_Lower:
                    break;
                case SpriteSheetTerrain.Green_Headquarter_Upper:
                    break;
                case SpriteSheetTerrain.Green_Headquarter_Lower:
                    break;

                case SpriteSheetTerrain.Yellow_City_Upper:
                    break;
                case SpriteSheetTerrain.Yellow_City_Lower:
                    break;
                case SpriteSheetTerrain.Yellow_AirPort_Upper:
                    break;
                case SpriteSheetTerrain.Yellow_AirPort_Lower:
                    break;
                case SpriteSheetTerrain.Yellow_Harbor_Upper:
                    break;
                case SpriteSheetTerrain.Yellow_Harbor_Lower:
                    break;
                case SpriteSheetTerrain.Yellow_Radar_Upper:
                    break;
                case SpriteSheetTerrain.Yellow_Radar_Lower:
                    break;
                case SpriteSheetTerrain.Yellow_SupplyBase_Upper:
                    break;
                case SpriteSheetTerrain.Yellow_SupplyBase_Lower:
                    break;
                case SpriteSheetTerrain.Yellow_Headquarter_Upper:
                    break;
                case SpriteSheetTerrain.Yellow_Headquarter_Lower:
                    break;

                case SpriteSheetTerrain.Missile_Silo_Upper:
                    break;
                case SpriteSheetTerrain.Missile_Silo_Lower:
                    break;
                default:
                    break;
            }


        }

        #region RotateThroughTerrain
        private void RotateThroughTerrain()
        {
            //rotate through terrain sprite
            if (HelperFunction.IsKeyPress(Keys.E))
            {
                currentlySelectedTerrain = GetNextTerrain(currentlySelectedTerrain);
            }
            if (HelperFunction.IsKeyPress(Keys.Q))
            {
                currentlySelectedTerrain = GetPreviousTerrain(currentlySelectedTerrain);
            }
        }

        private SpriteSheetTerrain GetNextTerrain(SpriteSheetTerrain t)
        {
            if (t.CompareWith(SpriteSheetTerrain.Max - 1) < 0)
            {
                t = t.Next();
            }
            else
            {
                t = SpriteSheetTerrain.Min.Next();
            }
            return t;
        }

        private SpriteSheetTerrain GetPreviousTerrain(SpriteSheetTerrain t)
        {
            if (t.CompareWith(SpriteSheetTerrain.Min + 1) > 0)
            {
                t = t.Previous();
            }
            else
            {
                t = SpriteSheetTerrain.Max.Previous();
            }
            return t;
        }
        #endregion

        private void MoveCamera(KeyboardState keyboardInputState, MouseState mouseInputState)
        {
            //simulate scrolling
            if (keyboardInputState.IsKeyDown(Keys.Left) || mouseInputState.Position.X.Between(100, 0))
            {
                camera.Location += new Vector2(-1, 0) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Right) || mouseInputState.Position.X.Between(720, 620))
            {
                camera.Location += new Vector2(1, 0) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Up) || mouseInputState.Position.Y.Between(50, 0))
            {
                camera.Location += new Vector2(0, -1) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Down) || mouseInputState.Position.Y.Between(480, 430))
            {
                camera.Location += new Vector2(0, +1) * 10;
            }
            camera.Location = new Vector2(Utility.HelperFunction.Clamp((int)camera.Location.X, 1680, 0), Utility.HelperFunction.Clamp((int)camera.Location.Y, 960, 0));
        }

        private void ZoomCamera()
        {
            int mouseScrollAmount = GetMouseScrollAmount();
            if ((mouseScrollAmount > 0) && (camera.Zoom < 1))
            {
                camera.Zoom += 0.1f;
            }

            if ((mouseScrollAmount < 0) && (camera.Zoom > 0.3f))
            {
                camera.Zoom -= 0.1f;
            }
        }
        #endregion

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
            CONTENT_MANAGER.spriteBatch.Draw(showtile, GuiSide == Side.Left ? new Vector2(0, 350) : new Vector2(630, 350), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiLower);
            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, map[selectedMapCell].terrain.ToString(), GuiSide == Side.Left ? new Vector2(0, 360) : new Vector2(650, 360), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, currentlySelectedTerrain.ToString(), GuiSide == Side.Left ? new Vector2(0, 430) : new Vector2(650, 430), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            CONTENT_MANAGER.spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, GuiSide == Side.Left ? new Vector2(10, 380) : new Vector2(660, 380), SpriteSheetSourceRectangle.GetSpriteRectangle(map[selectedMapCell].terrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
        }

        private void DrawMap(SpriteBatch spriteBatch)
        {
            //end that batch since the map will be render diffrently
            spriteBatch.End();
            //begin a new batch with translated matrix to simulate scrolling
            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);
            MapCell tempmapcell;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    tempmapcell = map[i, j];
                    if (tempmapcell.terrain != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * mapcellsize.X, j * mapcellsize.Y), SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainLower);
                    }
                    if (tempmapcell.overlappingterrain != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * mapcellsize.X, j * mapcellsize.Y), SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.overlappingterrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainUpper);
                    }
                }
            }

            spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(selectedMapCell.X * mapcellsize.X, selectedMapCell.Y * mapcellsize.Y), SpriteSheetSourceRectangle.GetSpriteRectangle(currentlySelectedTerrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiLower);
            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * mapcellsize.X, selectedMapCell.Y * mapcellsize.Y), new Rectangle(0, 0, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            //end this batch
            spriteBatch.End();
            //start a new batch for whatever come after
            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
