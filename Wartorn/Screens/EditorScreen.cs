using System;
using System.IO;
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
using Wartorn.Drawing.Animation;

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
        private MiniMapGenerator minimapgen;

        private Texture2D showtile;
        private Texture2D minimap;

        private Point selectedMapCell = new Point(0, 0);
        private Vector2 offset = new Vector2(70, 70);
        private Rectangle mapArea;

        //private SpriteSheetTerrain currentlySelectedTerrain = SpriteSheetTerrain.Tree_up_left;
        private TerrainType currentlySelectedTerrain = TerrainType.Plain;
        private Owner currentlySelectedOwner = Owner.None;

        private Side GuiSide = Side.Left;
        private bool isMenuOpen = true;
        private bool isQuickRotate = true;

        //represent a changing map cell action
        struct Action
        {
            public Point selectedMapCell;
            public TerrainType selectedMapCellTerrain;
            public Owner selectedMapCellOwner;

            public Action(Point p, MapCell mc)
            {
                selectedMapCell = p;
                selectedMapCellTerrain = mc.terrain;
                selectedMapCellOwner = mc.owner;
            }
        }
        Stack<Action> undostack;

        public EditorScreen(GraphicsDevice device) : base(device, "EditorScreen")
        {
            LoadContent();
        }

        private void LoadContent()
        {
            showtile = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\showtile");
        }

        public override bool Init()
        {
            map = new Map(50, 30);
            mapArea = new Rectangle(0, 0, map.Width * Constants.MapCellWidth, map.Height * Constants.MapCellHeight);
            canvas = new Canvas();
            camera = new Camera(_device.Viewport);
            minimapgen = new MiniMapGenerator(_device, CONTENT_MANAGER.spriteBatch);

            undostack = new Stack<Action>();

            InitUI();
            InitMap(TerrainType.Plain);

            minimap = minimapgen.GenerateMapTexture(map);
            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        #region InitUI

        private void InitEscapeMenu()
        {
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

            //bind action to ui event
            button_Undo.MouseClick += (sender, e) =>
            {
                if (undostack.Count > 0)
                {
                    var lastaction = undostack.Pop();
                    map[lastaction.selectedMapCell].terrain = lastaction.selectedMapCellTerrain;
                    map[lastaction.selectedMapCell].owner = lastaction.selectedMapCellOwner;
                    map.IsProcessed = false;
                }
            };

            button_Save.MouseClick += (sender, e) =>
            {
                var savedata = MapData.SaveMap(map);
                try
                {
                    Directory.CreateDirectory(@"map/");
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
                SCREEN_MANAGER.go_back();
            };

            canvas.AddElement("canvas_Menu", canvas_Menu);
        }

        private void InitTileSelectMenu()
        {
            //terrain selection menu
            Canvas canvas_terrain_selection = new Canvas();

            Button button_changeTerrainTheme = new Button("Normal", new Point(10, 50), new Vector2(80, 20), CONTENT_MANAGER.arcadefont);
            button_changeTerrainTheme.Origin = new Vector2(10, 0);
            button_changeTerrainTheme.backgroundColor = Color.White;
            button_changeTerrainTheme.foregroundColor = Color.Black;
            Button button_changeWeather = new Button("Sunny", new Point(100, 50), new Vector2(80, 20), CONTENT_MANAGER.arcadefont);
            button_changeWeather.backgroundColor = Color.White;
            button_changeWeather.foregroundColor = Color.Black;

            List<Button> tempbuttonlist = new List<Button>();

            //terrain button
            Button button_bridge = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Bridge_hor), new Point(10, 80), 0.75f, false);
            Button button_road = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Road_hor), new Point(50, 80), 0.75f, false);
            Button button_wood = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tree), new Point(90, 80), 0.75f, false);
            Button button_mountain = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Mountain_Low), new Point(130, 80), 0.75f, false);
            Button button_plain = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Plain), new Point(170, 80), 0.75f, false);

            Button button_reef = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Reef), new Point(210, 80), 0.75f, false);
            Button button_shoal = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Coast_up), new Point(250, 80), 0.75f, false);
            Button button_river = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.River_hor), new Point(290, 80), 0.75f, false);
            Button button_sea = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Sea), new Point(330, 80), 0.75f, false);

            tempbuttonlist.Add(button_bridge);
            tempbuttonlist.Add(button_road);
            tempbuttonlist.Add(button_wood);
            tempbuttonlist.Add(button_mountain);
            tempbuttonlist.Add(button_plain);
            tempbuttonlist.Add(button_reef);
            tempbuttonlist.Add(button_shoal);
            tempbuttonlist.Add(button_river);
            tempbuttonlist.Add(button_sea);

            //bind event
            button_changeTerrainTheme.MouseClick += (sender, e) =>
            {
                //normal -> tropical -> desert -> normal ...
                switch (button_changeTerrainTheme.Text)
                {
                    case "Normal":
                        button_changeTerrainTheme.Text = "Tropical";
                        button_changeWeather.Text = "Sunny";
                        button_bridge.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tropical_Bridge_hor);
                        button_road.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tropical_Road_hor);
                        button_wood.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tropical_Tree);
                        button_mountain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tropical_Mountain_Low);
                        button_plain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tropical_Plain);
                        map.theme = Theme.Tropical;
                        map.weather = Weather.Sunny;
                        break;
                    case "Tropical":
                        button_changeTerrainTheme.Text = "Desert";
                        button_changeWeather.Text = "Sunny";
                        button_bridge.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Bridge_hor);
                        button_road.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Road_hor);
                        button_wood.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Tree);
                        button_mountain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Mountain_High_Lower);
                        button_plain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Plain);

                        button_reef.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Reef);
                        button_shoal.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Coast_up);
                        button_river.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_River_hor);
                        button_sea.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Sea);
                        map.theme = Theme.Desert;
                        map.weather = Weather.Sunny;
                        break;
                    case "Desert":
                        button_changeTerrainTheme.Text = "Normal";
                        button_changeWeather.Text = "Sunny";
                        button_bridge.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Bridge_hor);
                        button_road.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Road_hor);
                        button_wood.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tree);
                        button_mountain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Mountain_Low);
                        button_plain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Plain);

                        button_reef.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Reef);
                        button_shoal.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Coast_up);
                        button_river.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.River_hor);
                        button_sea.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Sea);
                        map.theme = Theme.Normal;
                        map.weather = Weather.Sunny;
                        break;
                    default:
                        break;
                }
                map.IsProcessed = false;
            };

            button_changeWeather.MouseClick += (sender, e) =>
            {
                //Sunny -> rain -> snow
                switch (button_changeWeather.Text)
                {
                    case "Sunny":
                        button_changeWeather.Text = "Rain";
                        button_changeTerrainTheme.Text = "Normal";
                        button_bridge.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Bridge_hor);
                        button_road.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Road_hor);
                        button_wood.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Tree);
                        button_mountain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Mountain_Low);
                        button_plain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Plain);

                        button_reef.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Reef);
                        button_shoal.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Coast_up);
                        button_river.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_River_hor);
                        button_sea.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Sea);
                        map.weather = Weather.Rain;
                        break;
                    case "Rain":
                        button_changeWeather.Text = "Snow";
                        button_changeTerrainTheme.Text = "Normal";
                        button_bridge.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Bridge_hor);
                        button_road.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Road_hor);
                        button_wood.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Tree);
                        button_mountain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Mountain_Low);
                        button_plain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Plain);

                        button_reef.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Reef);
                        button_shoal.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Coast_up);
                        button_river.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_River_hor);
                        button_sea.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Sea);
                        map.weather = Weather.Snow;
                        break;
                    case "Snow":
                        button_changeWeather.Text = "Sunny";
                        button_changeTerrainTheme.Text = "Normal";
                        button_bridge.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Bridge_hor);
                        button_road.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Road_hor);
                        button_wood.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tree);
                        button_mountain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Mountain_Low);
                        button_plain.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Plain);

                        button_reef.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Reef);
                        button_shoal.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Coast_up);
                        button_river.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.River_hor);
                        button_sea.spriteSourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Sea);
                        map.weather = Weather.Sunny;
                        break;
                    default:
                        break;
                }
                map.IsProcessed = false;
            };

            foreach (var button in tempbuttonlist)
            {
                button.MouseClick += (sender, e) =>
                {
                    currentlySelectedTerrain = SpriteSheetSourceRectangle.GetTerrain(button.spriteSourceRectangle).ToTerrainType();
                };
            }

            canvas_terrain_selection.AddElement("button_changeTerrainTheme", button_changeTerrainTheme);
            canvas_terrain_selection.AddElement("button_changeWeather", button_changeWeather);
            canvas_terrain_selection.AddElement("button_bridge", button_bridge);
            canvas_terrain_selection.AddElement("button_road", button_road);
            canvas_terrain_selection.AddElement("button_reef", button_reef);
            canvas_terrain_selection.AddElement("button_shoal", button_shoal);
            canvas_terrain_selection.AddElement("button_river", button_river);
            canvas_terrain_selection.AddElement("button_wood", button_wood);
            canvas_terrain_selection.AddElement("button_mountain", button_mountain);
            canvas_terrain_selection.AddElement("button_sea", button_sea);
            canvas_terrain_selection.AddElement("button_plain", button_plain);

            //building button
            Button button_changeOwner = new Button("None", new Point(10, 120), new Vector2(80, 20), CONTENT_MANAGER.arcadefont);
            button_changeOwner.Origin = new Vector2(10, 0);
            button_changeOwner.backgroundColor = Color.White;
            button_changeOwner.foregroundColor = Color.Black;

            tempbuttonlist = new List<Button>();

            Button button_city = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(0, 0, 48, 96), new Point(10, 140), 0.75f);
            Button button_factory = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(48, 0, 48, 96), new Point(50, 140), 0.75f);
            Button button_airport = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(96, 0, 48, 96), new Point(90, 140), 0.75f);
            Button button_harbor = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(144, 0, 48, 96), new Point(130, 140), 0.75f);
            Button button_radar = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(192, 0, 48, 96), new Point(170, 140), 0.75f);
            Button button_supplybase = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(240, 0, 48, 96), new Point(210, 140), 0.75f);
            Button button_headquarter = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(288, 0, 48, 96), new Point(250, 140), 0.75f);
            Button button_missilesilo = new Button(CONTENT_MANAGER.buildingSpriteSheet, new Rectangle(336, 0, 48, 96), new Point(290, 140), 0.75f);

            tempbuttonlist.Add(button_city);
            tempbuttonlist.Add(button_factory);
            tempbuttonlist.Add(button_airport);
            tempbuttonlist.Add(button_harbor);
            tempbuttonlist.Add(button_radar);
            tempbuttonlist.Add(button_supplybase);
            tempbuttonlist.Add(button_headquarter);
            tempbuttonlist.Add(button_missilesilo);

            //bind event
            button_changeOwner.MouseClick += (sender, e) =>
            {
                int nextowner = 0;
                switch (button_changeOwner.Text)
                {
                    case "None":
                        button_changeOwner.Text = "Red";
                        currentlySelectedOwner = Owner.Red;
                        nextowner = 96;
                        break;
                    case "Red":
                        button_changeOwner.Text = "Blue";
                        currentlySelectedOwner = Owner.Blue;
                        nextowner = 192;
                        break;
                    case "Blue":
                        button_changeOwner.Text = "Green";
                        currentlySelectedOwner = Owner.Green;
                        nextowner = 288;
                        break;
                    case "Green":
                        button_changeOwner.Text = "Yellow";
                        currentlySelectedOwner = Owner.Yellow;
                        nextowner = 384;
                        break;
                    case "Yellow":
                        button_changeOwner.Text = "None";
                        currentlySelectedOwner = Owner.None;
                        nextowner = 0;
                        break;
                    default:
                        break;
                }
                Rectangle temp;
                for (int i = 0; i < tempbuttonlist.Count; i++)
                {
                    temp = tempbuttonlist[i].spriteSourceRectangle;
                    tempbuttonlist[i].spriteSourceRectangle = new Rectangle(temp.X, nextowner, temp.Width, temp.Height);
                }
            };

            button_city.MouseClick += (sender, e) =>
            {
                currentlySelectedTerrain = TerrainType.City;
            };

            button_factory.MouseClick += (sender, e) =>
            {
                currentlySelectedTerrain = TerrainType.Factory;
            };

            button_airport.MouseClick += (sender, e) =>
            {
                currentlySelectedTerrain = TerrainType.AirPort;
            };

            button_harbor.MouseClick += (sender, e) =>
            {
                currentlySelectedTerrain = TerrainType.Harbor;
            };

            button_radar.MouseClick += (sender, e) =>
            {
                currentlySelectedTerrain = TerrainType.Radar;
            };

            button_supplybase.MouseClick += (sender, e) =>
            {
                currentlySelectedTerrain = TerrainType.SupplyBase;
            };

            button_headquarter.MouseClick += (sender, e) =>
            {
                if (button_headquarter.spriteSourceRectangle.Y != 0)
                {
                    currentlySelectedTerrain = TerrainType.HQ;
                }
            };

            button_missilesilo.MouseClick += (sender, e) =>
            {
                if (button_missilesilo.spriteSourceRectangle.Y == 0)
                {
                    currentlySelectedTerrain = TerrainType.MissileSilo;
                }
            };

            canvas_terrain_selection.AddElement("button_changeOwner", button_changeOwner);
            canvas_terrain_selection.AddElement("button_city", button_city);
            canvas_terrain_selection.AddElement("button_factory", button_factory);
            canvas_terrain_selection.AddElement("button_airport", button_airport);
            canvas_terrain_selection.AddElement("button_harbor", button_harbor);
            canvas_terrain_selection.AddElement("button_radar", button_radar);
            canvas_terrain_selection.AddElement("button_supplybase", button_supplybase);
            canvas_terrain_selection.AddElement("button_headquarter", button_headquarter);
            canvas_terrain_selection.AddElement("button_missilesilo", button_missilesilo);

            canvas.AddElement("canvas_terrain_selection", canvas_terrain_selection);
        }

        private void InitUI()
        {
            //declare ui element
            InitEscapeMenu();
            InitTileSelectMenu();            

            //side menu
            Label label1 = new Label("Hor" + Environment.NewLine + "Ver", new Point(0, 0), new Vector2(30, 20), CONTENT_MANAGER.defaultfont);
            label1.Scale = 1.2f;
            Label label_Horizontal = new Label("1", new Point(40, 0), new Vector2(20, 20), CONTENT_MANAGER.defaultfont);
            label_Horizontal.Scale = 1.2f;
            Label label_Vertical = new Label("1", new Point(40, 20), new Vector2(20, 20), CONTENT_MANAGER.defaultfont);
            label_Vertical.Scale = 1.2f;

            

            //add ui element to canvas
            canvas.AddElement("label1", label1);
            canvas.AddElement("label_Horizontal", label_Horizontal);
            canvas.AddElement("label_Vertical", label_Vertical);
        }
        #endregion

        private void InitMap(TerrainType terraintype)
        {
            int count = 1;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    map[i, j] = new MapCell(terraintype, null);
                }
            }

            map[10, 10] = new MapCell(TerrainType.City);
            map[10, 10].owner = Owner.Red;

            Unit soldier1 = UnitCreationHelper.Create(UnitType.Artillery, Owner.Red);
            soldier1.Animation.Depth = LayerDepth.Unit;
            soldier1.Animation.PlayAnimation(AnimationName.right.ToString());
            soldier1.Animation.FlipEffect = SpriteEffects.FlipHorizontally;

            map[5, 5].unit = soldier1;
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
            //MoveGuiToAvoidMouse(mouseInputState);

            if (!isMenuOpen)
            {
                PlaceTile(mouseInputState);
                RotateThroughTerrain(keyboardInputState);
            }
            MoveCamera(keyboardInputState, mouseInputState);

            UpdateAnimation(gameTime);
            //ZoomCamera();

            if (!map.IsProcessed)
            {
                minimap = minimapgen.GenerateMapTexture(map);
            }
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            foreach (MapCell mapcell in map)
            {
                if (mapcell.unit!=null)
                {
                    mapcell.unit.Animation.Update(gameTime);
                }
            }
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
                        if (map[selectedMapCell].terrain != currentlySelectedTerrain || map[selectedMapCell].owner != currentlySelectedOwner)
                        {
                            undostack.Push(new Action(selectedMapCell, map[selectedMapCell]));
                            map[selectedMapCell].terrain = currentlySelectedTerrain;
                            map[selectedMapCell].owner = currentlySelectedOwner;
                            map.IsProcessed = false;
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
                currentlySelectedTerrain = currentlySelectedTerrain.Next();
            }
            if ((HelperFunction.IsKeyPress(Keys.Q) && !isQuickRotate)
              || (keyboardInputState.IsKeyDown(Keys.Q) && isQuickRotate))
            {
                currentlySelectedTerrain = currentlySelectedTerrain.Previous();
            }
        }
        #endregion

        private void MoveCamera(KeyboardState keyboardInputState, MouseState mouseInputState)
        {
            if (isMenuOpen)
            {
                return;
            }

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
            temp.X = (int)(temp.X / Constants.MapCellWidth);       //mapcell size
            temp.Y = (int)(temp.Y / Constants.MapCellHeight);

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
            DrawMap(CONTENT_MANAGER.spriteBatch,gameTime);
            canvas.Draw(CONTENT_MANAGER.spriteBatch);

            //draw side menu
            CONTENT_MANAGER.spriteBatch.Draw(showtile, GuiSide == Side.Left ? new Vector2(0, 350) : new Vector2(630, 350), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiLower);
            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, map[selectedMapCell].terrainbase.ToTerrainType().ToString(), GuiSide == Side.Left ? new Vector2(0, 360) : new Vector2(650, 360), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, currentlySelectedTerrain.ToString(), GuiSide == Side.Left ? new Vector2(0, 430) : new Vector2(650, 430), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            CONTENT_MANAGER.spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, GuiSide == Side.Left ? new Vector2(10, 380) : new Vector2(660, 380), SpriteSheetSourceRectangle.GetSpriteRectangle(map[selectedMapCell].terrainbase), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);

            //draw minimap
            if (isMenuOpen)
            {
                CONTENT_MANAGER.spriteBatch.Draw(minimap, new Vector2(400, 10), null, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, LayerDepth.GuiLower);
            }
        }

        private void DrawMap(SpriteBatch spriteBatch,GameTime gameTime)
        {
            //end that batch since the map will be render diffrently
            spriteBatch.End();

            //begin a new batch with translated matrix to simulate scrolling
            //aka make a camera
            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);
            
            //render the map
            MapRenderer.Render(map, spriteBatch, gameTime);

            #region draw cursor
            //spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), SpriteSheetSourceRectangle.GetSpriteRectangle(currentlySelectedTerrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiLower);
            //draw the tile that is about to be placed
            if (currentlySelectedTerrain.Is2Tile())
            {
                //ve 2 tile tren duoi
                Rectangle uppertile = Rectangle.Empty;
                Rectangle lowertile = Rectangle.Empty;
                int nextowner = 0;
                switch (currentlySelectedOwner)
                {
                    case Owner.None:
                        nextowner = 0;
                        break;
                    case Owner.Red:
                        nextowner = SpriteSheetTerrain.Red_City_Lower - SpriteSheetTerrain.City_Lower;
                        break;
                    case Owner.Blue:
                        nextowner = SpriteSheetTerrain.Blue_City_Lower - SpriteSheetTerrain.City_Lower;
                        break;
                    case Owner.Green:
                        nextowner = SpriteSheetTerrain.Green_City_Lower - SpriteSheetTerrain.City_Lower;
                        break;
                    case Owner.Yellow:
                        nextowner = SpriteSheetTerrain.Yellow_City_Lower - SpriteSheetTerrain.City_Lower;
                        break;
                    default:
                        break;
                }

                switch (currentlySelectedTerrain)
                {
                    case TerrainType.Mountain:
                        switch (map.weather)
                        {
                            case Weather.Sunny:
                                switch (map.theme)
                                {
                                    case Theme.Normal:
                                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Mountain_High_Lower);
                                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Mountain_High_Upper);
                                        break;
                                    case Theme.Tropical:
                                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tropical_Mountain_High_Lower);
                                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Tropical_Mountain_High_Upper);
                                        break;
                                    case Theme.Desert:
                                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Mountain_High_Lower);
                                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Desert_Mountain_High_Upper);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case Weather.Rain:
                                lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Mountain_High_Lower);
                                uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Rain_Mountain_High_Upper);
                                break;
                            case Weather.Snow:
                                lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Mountain_High_Lower);
                                uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Snow_Mountain_High_Upper);
                                break;
                            default:
                                break;
                        }
                        break;


                    case TerrainType.MissileSilo:
                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Missile_Silo_Lower);
                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Missile_Silo_Upper);
                        break;
                    case TerrainType.City:
                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.City_Lower.Next(nextowner));
                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.City_Upper.Next(nextowner));
                        break;
                    case TerrainType.AirPort:
                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.AirPort_Lower.Next(nextowner));
                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.AirPort_Upper.Next(nextowner));
                        break;
                    case TerrainType.Harbor:
                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Harbor_Lower.Next(nextowner));
                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Harbor_Upper.Next(nextowner));
                        break;
                    case TerrainType.Radar:
                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Radar_Lower.Next(nextowner));
                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Radar_Upper.Next(nextowner));
                        break;
                    case TerrainType.SupplyBase:
                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.SupplyBase_Lower.Next(nextowner));
                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.SupplyBase_Upper.Next(nextowner));
                        break;
                    case TerrainType.HQ:
                        switch (currentlySelectedOwner)
                        {
                            case Owner.Red:
                                nextowner = 0;
                                break;
                            case Owner.Blue:
                                nextowner = SpriteSheetTerrain.Blue_Headquarter_Lower - SpriteSheetTerrain.Red_Headquarter_Lower;
                                break;
                            case Owner.Green:
                                nextowner = SpriteSheetTerrain.Green_Headquarter_Lower - SpriteSheetTerrain.Red_Headquarter_Lower;
                                break;
                            case Owner.Yellow:
                                nextowner = SpriteSheetTerrain.Yellow_Headquarter_Lower - SpriteSheetTerrain.Red_Headquarter_Lower;
                                break;
                            default:
                                break;
                        }
                        lowertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Red_Headquarter_Lower.Next(nextowner));
                        uppertile = SpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetTerrain.Red_Headquarter_Upper.Next(nextowner));
                        break;
                    default:
                        break;
                }
                spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), lowertile, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
                spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, (selectedMapCell.Y-1) * Constants.MapCellHeight), uppertile, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
            }
            else
            {
                //ve 1 tile
                spriteBatch.Draw(CONTENT_MANAGER.spriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), SpriteSheetSourceRectangle.GetSpriteRectangle(currentlySelectedTerrain), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
            }

            //draw the cursor
            //todo change cursor
            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), new Rectangle(0, 0, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
            #endregion

            //end this batch
            spriteBatch.End();

            //start a new batch for whatever come after
            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
