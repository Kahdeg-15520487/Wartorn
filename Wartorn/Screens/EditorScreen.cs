﻿using System;
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

        private SpriteSheetTerrain currentlySelectedTerrain = SpriteSheetTerrain.Tree_up_left;

        private Side GuiSide = Side.Left;
        private bool isMenuOpen = true;
        private bool isQuickRotate = true;

        //represent a changing map cell action
        struct Action
        {
            public Point selectedMapCell;
            public SpriteSheetTerrain selectedMapCellTerrain;
            public SpriteSheetTerrain selectedMapCellOverlappingTerrain;

            public Action(Point p, SpriteSheetTerrain t, SpriteSheetTerrain ot)
            {
                selectedMapCell = p;
                selectedMapCellTerrain = t;
                selectedMapCellOverlappingTerrain = ot;
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

            Dictionary<SpriteSheetTerrain, Button> terrainSelectionButton = new Dictionary<SpriteSheetTerrain, Button>();

            Button button_changeWaterTheme = new Button("Normal", new Point(10, 50), new Vector2(80, 20), CONTENT_MANAGER.defaultfont);
            button_changeWaterTheme.backgroundColor = Color.White;
            button_changeWaterTheme.foregroundColor = Color.Black;
            Button button_changeWaterWeather = new Button("Sunny", new Point(100, 50), new Vector2(80, 20), CONTENT_MANAGER.defaultfont);
            button_changeWaterWeather.backgroundColor = Color.White;
            button_changeWaterWeather.foregroundColor = Color.Black;

            Button button_changeRoadTreeMountainTheme = new Button("Normal", new Point(10, 160), new Vector2(80, 20), CONTENT_MANAGER.defaultfont);
            button_changeRoadTreeMountainTheme.backgroundColor = Color.White;
            button_changeRoadTreeMountainTheme.foregroundColor = Color.Black;

            int col = 0;
            int row = 0;

            //water selection
            for (SpriteSheetTerrain i = SpriteSheetTerrain.Reef; i.CompareWith(SpriteSheetTerrain.Invert_Coast_right_down) <= 0; i = i.Next())
            {
                Button temp = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(i), new Point(col * 26 + 10, row * 26 + 80), 0.5f, false);
                temp.Text = i.ToString();
                temp.MouseClick += (sender, e) =>
                {
                    currentlySelectedTerrain = temp.Text.ToEnum<SpriteSheetTerrain>();
                };
                terrainSelectionButton.Add(i, temp);
                col++;
                if (col == 27)
                {
                    col = 0;
                    row++;
                }
            }
            button_changeWaterTheme.MouseClick += (sender, e) =>
            {
                //current terrain is reef then next = 61 -> next terrain is rain_reef
                //current terrain is desert_reef then next = 0 -> next terrain is reef
                int next = 0;
                switch (terrainSelectionButton[SpriteSheetTerrain.Reef].Text.ToEnum<SpriteSheetTerrain>())
                {
                    case SpriteSheetTerrain.Reef:
                        next = 61;
                        button_changeWaterTheme.Text = "Rain";
                        break;
                    case SpriteSheetTerrain.Rain_Reef:
                        button_changeWaterTheme.Text = "Snow";
                        next = 122;
                        break;
                    case SpriteSheetTerrain.Snow_Reef:
                        button_changeWaterTheme.Text = "Desert";
                        next = 183;
                        break;
                    case SpriteSheetTerrain.Desert_Reef:
                        button_changeWaterTheme.Text = "Normal";
                        next = 0;
                        break;
                    default:
                        break;
                }
                for (SpriteSheetTerrain i = SpriteSheetTerrain.Reef; i.CompareWith(SpriteSheetTerrain.Invert_Coast_right_down) <= 0; i = i.Next())
                {
                    terrainSelectionButton[i].spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(i.Next(next));
                    terrainSelectionButton[i].Text = i.Next(next).ToString();
                }
            };

            //road,tree,mountain selection



            canvas_terrain_selection.AddElement("button_changeWaterTheme", button_changeWaterTheme);
            canvas_terrain_selection.AddElement("button_changeWaterWeather", button_changeWaterWeather);
            canvas_terrain_selection.AddElement("button_changeRoadTreeMountainTheme", button_changeRoadTreeMountainTheme);
            foreach (var item in terrainSelectionButton)
            {
                canvas_terrain_selection.AddElement(item.Key.ToString(), item.Value);
            }


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
                    map[lastaction.selectedMapCell].terrainbase = lastaction.selectedMapCellTerrain;
                    map[lastaction.selectedMapCell].terrainLower = lastaction.selectedMapCellOverlappingTerrain;
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
                    if (temp != null)
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
            canvas.AddElement("canvas_terrain_selection", canvas_terrain_selection);

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
                    map[i, j] = new MapCell(SpriteSheetTerrain.Sea);
                    continue;
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
                RotateThroughTerrain(keyboardInputState);
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
                ((Canvas)canvas.GetElement("canvas_terrain_selection")).IsVisible = isMenuOpen;
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
                        if (map[selectedMapCell].terrainbase != currentlySelectedTerrain)
                        {
                            undostack.Push(new Action(selectedMapCell, map[selectedMapCell].terrainbase, map[selectedMapCell].terrainLower));
                            if (currentlySelectedTerrain.ToString().Contains("Upper"))
                            {
                                map[selectedMapCell].terrainUpper = currentlySelectedTerrain;
                            }
                            else
                            {
                                if (currentlySelectedTerrain.ToString().Contains("Lower"))
                                {
                                    if (currentlySelectedTerrain.ToString().Contains("Mountain"))
                                    {
                                        map[selectedMapCell].terrainbase = currentlySelectedTerrain;
                                    }
                                    else
                                    {
                                        map[selectedMapCell].terrainLower = currentlySelectedTerrain;
                                    }
                                }
                                else
                                {
                                    map[selectedMapCell].terrainbase = currentlySelectedTerrain;
                                }
                            }
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
                        if (map[selectedMapCell].terrainbase != currentlySelectedTerrain)
                        {
                            currentlySelectedTerrain = map[selectedMapCell].terrainbase;
                        }
                    }
                }
            }
        }

        //xử lí việc 1 tile lớn gồm nhiều tile nhỏ được vẽ
        private void HandleMultiTileSprite(Point p, SpriteSheetTerrain t)
        {
            var mapcell = map[p];
            Point center;

            switch (t)
            {
                /*
                top_left    |   top_right
                ____________|____________
                bottom_left |   bottom_right
                            |
                top_left,top_right,bottom_left,bottom_right
                */

                //   *
                //top_left,top_right,bottom_left,bottom_right
                case SpriteSheetTerrain.Tree_top_left:
                case SpriteSheetTerrain.Tropical_Tree_top_left:
                case SpriteSheetTerrain.Rain_Tree_top_left:
                case SpriteSheetTerrain.Snow_Tree_top_left:
                    if (p.X.Between(map.Width - 1, 0) && p.Y.Between(map.Height - 1, 0))
                    {
                        map[p.GetNearbyPoint(Direction.East)].terrainbase = t.Next();
                        map[p.GetNearbyPoint(Direction.South)].terrainbase = t.Next().Next();
                        map[p.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next().Next().Next();
                    }
                    break;

                //             *
                //top_left,top_right,bottom_left,bottom_right
                case SpriteSheetTerrain.Tree_top_right:
                case SpriteSheetTerrain.Tropical_Tree_top_right:
                case SpriteSheetTerrain.Rain_Tree_top_right:
                case SpriteSheetTerrain.Snow_Tree_top_right:
                    if (p.X.Between(map.Width, 1) && p.Y.Between(map.Height - 1, 0))
                    {
                        map[p.GetNearbyPoint(Direction.West)].terrainbase = t.Previous();
                        map[p.GetNearbyPoint(Direction.South)].terrainbase = t.Next().Next();
                        map[p.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Next();
                    }
                    break;

                //                        *
                //top_left,top_right,bottom_left,bottom_right
                case SpriteSheetTerrain.Tree_bottom_left:
                case SpriteSheetTerrain.Tropical_Tree_bottom_left:
                case SpriteSheetTerrain.Rain_Tree_bottom_left:
                case SpriteSheetTerrain.Snow_Tree_bottom_left:
                    if (p.X.Between(map.Width - 1, 0) && p.Y.Between(map.Height, 1))
                    {
                        map[p.GetNearbyPoint(Direction.East)].terrainbase = t.Next();
                        map[p.GetNearbyPoint(Direction.North)].terrainbase = t.Previous().Previous();
                        map[p.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Previous();
                    }
                    break;

                //                                   *
                //top_left,top_right,bottom_left,bottom_right
                case SpriteSheetTerrain.Tree_bottom_right:
                case SpriteSheetTerrain.Tropical_Tree_bottom_right:
                case SpriteSheetTerrain.Rain_Tree_bottom_right:
                case SpriteSheetTerrain.Snow_Tree_bottom_right:
                    if (p.X.Between(map.Width, 1) && p.Y.Between(map.Height, 1))
                    {
                        map[p.GetNearbyPoint(Direction.West)].terrainbase = t.Previous();
                        map[p.GetNearbyPoint(Direction.North)].terrainbase = t.Previous().Previous();
                        map[p.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous().Previous().Previous();
                    }
                    break;
                /*
                    Tree_up_left        Tree_up_middle        Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left      Tree_down_middle      Tree_down_right,
                */

                /*
                   *Tree_up_left        Tree_up_middle        Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left      Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_up_left:
                case SpriteSheetTerrain.Tropical_Tree_up_left:
                case SpriteSheetTerrain.Rain_Tree_up_left:
                case SpriteSheetTerrain.Snow_Tree_up_left:
                    if (p.X.Between(map.Width - 2, 0) && p.Y.Between(map.Height - 2, 0))
                    {
                        center = p.GetNearbyPoint(Direction.SouthEast);

                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Next();
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Next(2);
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Next(3);
                        map[center].terrainbase = t.Next(4);
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Next(5);
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Next(6);
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Next(7);
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next(8);
                    }
                    break;

                /*
                    Tree_up_left       *Tree_up_middle        Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left      Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_up_middle:
                case SpriteSheetTerrain.Tropical_Tree_up_middle:
                case SpriteSheetTerrain.Rain_Tree_up_middle:
                case SpriteSheetTerrain.Snow_Tree_up_middle:
                    if (p.X.Between(map.Width - 1, 1) && p.Y.Between(map.Height - 2, 0))
                    {
                        center = p.GetNearbyPoint(Direction.South);

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous();
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Next();
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Next(2);
                        map[center].terrainbase = t.Next(3);
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Next(4);
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Next(5);
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Next(6);
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next(7);
                    }
                    break;
                /*
                    Tree_up_left        Tree_up_middle       *Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left      Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_up_right:
                case SpriteSheetTerrain.Tropical_Tree_up_right:
                case SpriteSheetTerrain.Rain_Tree_up_right:
                case SpriteSheetTerrain.Snow_Tree_up_right:
                    if (p.X.Between(map.Width, 2) && p.Y.Between(map.Height - 2, 0))
                    {
                        center = p.GetNearbyPoint(Direction.SouthWest);

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous(2);
                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Previous();
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Next();
                        map[center].terrainbase = t.Next(2);
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Next(3);
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Next(4);
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Next(5);
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next(6);
                    }
                    break;
                /*
                    Tree_up_left        Tree_up_middle        Tree_up_right, 
                   *Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left      Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_middle_left:
                case SpriteSheetTerrain.Tropical_Tree_middle_left:
                case SpriteSheetTerrain.Rain_Tree_middle_left:
                case SpriteSheetTerrain.Snow_Tree_middle_left:
                    if (p.X.Between(map.Width - 2, 0) && p.Y.Between(map.Height - 1, 1))
                    {
                        center = p.GetNearbyPoint(Direction.East);

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous(3);
                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Previous(2);
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Previous();
                        map[center].terrainbase = t.Next();
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Next(2);
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Next(3);
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Next(4);
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next(5);
                    }
                    break;

                /*
                    Tree_up_left        Tree_up_middle        Tree_up_right, 
                    Tree_middle_left   *Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left      Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_middle_middle:
                case SpriteSheetTerrain.Tropical_Tree_middle_middle:
                case SpriteSheetTerrain.Rain_Tree_middle_middle:
                case SpriteSheetTerrain.Snow_Tree_middle_middle:
                    if (p.X.Between(map.Width - 1, 1) && p.Y.Between(map.Height - 1, 1))
                    {
                        center = p;

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous(4);
                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Previous(3);
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Previous(2);
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Previous();
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Next();
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Next(2);
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Next(3);
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next(4);
                    }
                    break;

                /*
                    Tree_up_left        Tree_up_middle        Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle   *Tree_middle_right, 
                    Tree_down_left      Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_middle_right:
                case SpriteSheetTerrain.Tropical_Tree_middle_right:
                case SpriteSheetTerrain.Rain_Tree_middle_right:
                case SpriteSheetTerrain.Snow_Tree_middle_right:
                    if (p.X.Between(map.Width, 2) && p.Y.Between(map.Height - 1, 1))
                    {
                        center = p.GetNearbyPoint(Direction.West);

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous(5);
                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Previous(4);
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Previous(3);
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Previous(2);
                        map[center].terrainbase = t.Previous();
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Next();
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Next(2);
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next(3);
                    }
                    break;

                /*
                    Tree_up_left        Tree_up_middle        Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                   *Tree_down_left      Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_down_left:
                case SpriteSheetTerrain.Tropical_Tree_down_left:
                case SpriteSheetTerrain.Rain_Tree_down_left:
                case SpriteSheetTerrain.Snow_Tree_down_left:
                    if (p.X.Between(map.Width - 2, 0) && p.Y.Between(map.Height, 2))
                    {
                        center = p.GetNearbyPoint(Direction.NorthEast);

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous(6);
                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Previous(5);
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Previous(4);
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Previous(3);
                        map[center].terrainbase = t.Previous(2);
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Previous();
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Next();
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next(2);
                    }
                    break;

                /*
                    Tree_up_left        Tree_up_middle        Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left     *Tree_down_middle      Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_down_middle:
                case SpriteSheetTerrain.Tropical_Tree_down_middle:
                case SpriteSheetTerrain.Rain_Tree_down_middle:
                case SpriteSheetTerrain.Snow_Tree_down_middle:
                    if (p.X.Between(map.Width - 1, 1) && p.Y.Between(map.Height, 2))
                    {
                        center = p.GetNearbyPoint(Direction.North);

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous(7);
                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Previous(6);
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Previous(5);
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Previous(4);
                        map[center].terrainbase = t.Previous(3);
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Previous(2);
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Previous();
                        map[center.GetNearbyPoint(Direction.SouthEast)].terrainbase = t.Next();
                    }
                    break;

                /*
                    Tree_up_left        Tree_up_middle        Tree_up_right, 
                    Tree_middle_left    Tree_middle_middle    Tree_middle_right, 
                    Tree_down_left      Tree_down_middle     *Tree_down_right,
                */
                case SpriteSheetTerrain.Tree_down_right:
                case SpriteSheetTerrain.Tropical_Tree_down_right:
                case SpriteSheetTerrain.Rain_Tree_down_right:
                case SpriteSheetTerrain.Snow_Tree_down_right:
                    if (p.X.Between(map.Width, 2) && p.Y.Between(map.Height, 2))
                    {
                        center = p.GetNearbyPoint(Direction.NorthWest);

                        map[center.GetNearbyPoint(Direction.NorthWest)].terrainbase = t.Previous(8);
                        map[center.GetNearbyPoint(Direction.North)].terrainbase = t.Previous(7);
                        map[center.GetNearbyPoint(Direction.NorthEast)].terrainbase = t.Previous(6);
                        map[center.GetNearbyPoint(Direction.West)].terrainbase = t.Previous(5);
                        map[center].terrainbase = t.Previous(4);
                        map[center.GetNearbyPoint(Direction.East)].terrainbase = t.Previous(3);
                        map[center.GetNearbyPoint(Direction.SouthWest)].terrainbase = t.Previous(2);
                        map[center.GetNearbyPoint(Direction.South)].terrainbase = t.Previous();
                    }
                    break;

                //Mountain Upper
                case SpriteSheetTerrain.Mountain_High_Upper:
                case SpriteSheetTerrain.Tropical_Mountain_High_Upper:
                case SpriteSheetTerrain.Rain_Mountain_High_Upper:
                case SpriteSheetTerrain.Snow_Mountain_High_Upper:
                case SpriteSheetTerrain.Desert_Mountain_High_Upper:
                    if (p.Y.Between(map.Height - 1, 0))
                    {
                        map[p.GetNearbyPoint(Direction.South)].terrainbase = t.Next();
                    }
                    break;
                //Building Upper
                case SpriteSheetTerrain.City_Upper:
                case SpriteSheetTerrain.AirPort_Upper:
                case SpriteSheetTerrain.Harbor_Upper:
                case SpriteSheetTerrain.Radar_Upper:
                case SpriteSheetTerrain.SupplyBase_Upper:

                case SpriteSheetTerrain.Red_City_Upper:
                case SpriteSheetTerrain.Red_AirPort_Upper:
                case SpriteSheetTerrain.Red_Harbor_Upper:
                case SpriteSheetTerrain.Red_Radar_Upper:
                case SpriteSheetTerrain.Red_SupplyBase_Upper:

                case SpriteSheetTerrain.Blue_City_Upper:
                case SpriteSheetTerrain.Blue_AirPort_Upper:
                case SpriteSheetTerrain.Blue_Harbor_Upper:
                case SpriteSheetTerrain.Blue_Radar_Upper:
                case SpriteSheetTerrain.Blue_SupplyBase_Upper:

                case SpriteSheetTerrain.Green_City_Upper:
                case SpriteSheetTerrain.Green_AirPort_Upper:
                case SpriteSheetTerrain.Green_Harbor_Upper:
                case SpriteSheetTerrain.Green_Radar_Upper:
                case SpriteSheetTerrain.Green_SupplyBase_Upper:

                case SpriteSheetTerrain.Yellow_City_Upper:
                case SpriteSheetTerrain.Yellow_AirPort_Upper:
                case SpriteSheetTerrain.Yellow_Harbor_Upper:
                case SpriteSheetTerrain.Yellow_Radar_Upper:
                case SpriteSheetTerrain.Yellow_SupplyBase_Upper:

                case SpriteSheetTerrain.Red_Headquarter_Upper:
                case SpriteSheetTerrain.Blue_Headquarter_Upper:
                case SpriteSheetTerrain.Green_Headquarter_Upper:
                case SpriteSheetTerrain.Yellow_Headquarter_Upper:

                case SpriteSheetTerrain.Missile_Silo_Upper:
                    if (p.Y.Between(map.Height - 1, 0))
                    {
                        map[p.GetNearbyPoint(Direction.South)].terrainLower = t.Next();
                    }
                    break;

                //Mountain Lower
                case SpriteSheetTerrain.Mountain_High_Lower:
                case SpriteSheetTerrain.Tropical_Mountain_High_Lower:
                case SpriteSheetTerrain.Rain_Mountain_High_Lower:
                case SpriteSheetTerrain.Snow_Mountain_High_Lower:
                case SpriteSheetTerrain.Desert_Mountain_High_Lower:
                    if (p.Y.Between(map.Height, 0))
                    {
                        map[p.GetNearbyPoint(Direction.North)].terrainUpper = t.Previous();
                    }
                    break;

                //Building Lower
                case SpriteSheetTerrain.City_Lower:
                case SpriteSheetTerrain.AirPort_Lower:
                case SpriteSheetTerrain.Harbor_Lower:
                case SpriteSheetTerrain.Radar_Lower:
                case SpriteSheetTerrain.SupplyBase_Lower:

                case SpriteSheetTerrain.Red_City_Lower:
                case SpriteSheetTerrain.Red_AirPort_Lower:
                case SpriteSheetTerrain.Red_Harbor_Lower:
                case SpriteSheetTerrain.Red_Radar_Lower:
                case SpriteSheetTerrain.Red_SupplyBase_Lower:

                case SpriteSheetTerrain.Blue_City_Lower:
                case SpriteSheetTerrain.Blue_AirPort_Lower:
                case SpriteSheetTerrain.Blue_Harbor_Lower:
                case SpriteSheetTerrain.Blue_Radar_Lower:
                case SpriteSheetTerrain.Blue_SupplyBase_Lower:

                case SpriteSheetTerrain.Green_City_Lower:
                case SpriteSheetTerrain.Green_AirPort_Lower:
                case SpriteSheetTerrain.Green_Harbor_Lower:
                case SpriteSheetTerrain.Green_Radar_Lower:
                case SpriteSheetTerrain.Green_SupplyBase_Lower:

                case SpriteSheetTerrain.Yellow_City_Lower:
                case SpriteSheetTerrain.Yellow_AirPort_Lower:
                case SpriteSheetTerrain.Yellow_Harbor_Lower:
                case SpriteSheetTerrain.Yellow_Radar_Lower:
                case SpriteSheetTerrain.Yellow_SupplyBase_Lower:

                case SpriteSheetTerrain.Red_Headquarter_Lower:
                case SpriteSheetTerrain.Blue_Headquarter_Lower:
                case SpriteSheetTerrain.Green_Headquarter_Lower:
                case SpriteSheetTerrain.Yellow_Headquarter_Lower:

                case SpriteSheetTerrain.Missile_Silo_Lower:
                    if (p.Y.Between(map.Height, 0))
                    {
                        map[p.GetNearbyPoint(Direction.North)].terrainUpper = t.Previous();
                    }
                    break;

                default:
                    break;
            }
        }

        #region RotateThroughTerrain
        private void RotateThroughTerrain(KeyboardState keyboardInputState)
        {
            if (HelperFunction.IsKeyPress(Keys.P))
            {
                isQuickRotate = !isQuickRotate;
            }
            //rotate through terrain sprite
            if ((HelperFunction.IsKeyPress(Keys.E) && !isQuickRotate)
              || (keyboardInputState.IsKeyDown(Keys.E) && isQuickRotate))
            {
                currentlySelectedTerrain = GetNextTerrain(currentlySelectedTerrain);
            }
            if ((HelperFunction.IsKeyPress(Keys.Q) && !isQuickRotate)
              || (keyboardInputState.IsKeyDown(Keys.Q) && isQuickRotate))
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
            camera.Location = new Vector2(camera.Location.X.Clamp(1680, 0), camera.Location.Y.Clamp(960, 0));
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
            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, map[selectedMapCell].terrainbase.ToString(), GuiSide == Side.Left ? new Vector2(0, 360) : new Vector2(650, 360), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, currentlySelectedTerrain.ToString(), GuiSide == Side.Left ? new Vector2(0, 430) : new Vector2(650, 430), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            CONTENT_MANAGER.spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, GuiSide == Side.Left ? new Vector2(10, 380) : new Vector2(660, 380), SpriteSheetSourceRectangle.GetSpriteRectangle(map[selectedMapCell].terrainbase), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
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
                    if (tempmapcell.terrainbase != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * mapcellsize.X, j * mapcellsize.Y), SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainbase), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainBase);
                    }
                    if (tempmapcell.terrainLower != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * mapcellsize.X, j * mapcellsize.Y), SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainLower), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainLower);
                    }
                    if (tempmapcell.terrainUpper != SpriteSheetTerrain.None)
                    {
                        spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(i * mapcellsize.X, j * mapcellsize.Y), SpriteSheetSourceRectangle.GetSpriteRectangle(tempmapcell.terrainUpper), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.TerrainUpper);
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
