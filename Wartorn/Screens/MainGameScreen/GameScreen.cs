﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Wartorn.ScreenManager;
using Wartorn.Storage;
using Wartorn.GameData;
using Wartorn.UIClass;
using Wartorn.Utility;
using Wartorn.Utility.Drawing;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;

namespace Wartorn.Screens.MainGameScreen
{
    enum GameState
    {
        None,
        UnitSelected,
        BuildingSelected
    }

    class GameScreen : Screen
    {
        //information of this game session
        Session session;

        //ui canvas
        Canvas canvas;
        Canvas canvas_generalInfo;
        Canvas canvas_action;
        Canvas canvas_action_Factory;
        Canvas canvas_action_Airport;
        Canvas canvas_action_Harbor;
        Canvas canvas_action_Unit;
        Canvas canvas_action_Building;

        //resources
        Texture2D guibackground;
        Texture2D minimap;
        MiniMapGenerator minimapgen;

        //camera ?
        Camera camera;

        //input information
        MouseState mouseInputState;
        MouseState lastMouseInputState;
        KeyboardState keyboardInputState;
        KeyboardState lastKeyboardInputState;
        Point selectedMapCell;
        Point lastSelectedMapCell;

        //constants
        readonly Rectangle minimapbound = new Rectangle(2, 312, 234, 166);
        readonly Rectangle actionbound = new Rectangle(536, 340, 182, 138);

        //gui variable
        bool isHideGUI = false;

        //player information
        PlayerInfo[] playerInfos;
        int currentPlayer = 0;
        int localPlayer = 0;
        List<Unit> ownedUnit;

        //build unit information
        UnitType selectedUnitToBuild = UnitType.None;
        Point selectedBuilding = default(Point);

        //current unit selection
        Point selectedUnit = default(Point);
        List<Point> movementRange = null;

        //moving unit animation
        Graph dijkstraGraph;
        List<Point> movementPath;
        Point destination;
        bool isMovingUnitAnimPlaying = false;
        bool isMovePathCalculated = false;
        MovingUnitAnimation movingAnim;
        DirectionArrowRenderer dirarrowRenderer = new DirectionArrowRenderer();

        //fog of war
        bool[,] mapcellVisibility;

        //game state
        GameState currentGameState = GameState.None;

        public GameScreen(GraphicsDevice device) : base(device, "GameScreen")
        {
            LoadContent();
            minimapgen = new MiniMapGenerator(device, CONTENT_MANAGER.spriteBatch);
        }

        #region Innit
        public void InitSession(SessionData sessiondata)
        {
            session = new Session(sessiondata);
            minimap = minimapgen.GenerateMapTexture(session.map);
            playerInfos = sessiondata.playerInfos;
            ownedUnit = new List<Unit>();
            mapcellVisibility = new bool[session.map.Width, session.map.Height];

            //init visibility table
            for (int x = 0; x < session.map.Width; x++)
            {
                for (int y = 0; y < session.map.Height; y++)
                {
                    mapcellVisibility[x, y] = false;
                }
            }
        }

        private void LoadContent()
        {
            guibackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\placeholdergui");
        }

        public override bool Init()
        {
            //TODO
            //do something to handshake player

            camera = new Camera(_device.Viewport);
            canvas = new Canvas();

            InitUI();

            foreach (MapCell mapcell in session.map)
            {
                if (mapcell.unit!=null && mapcell.unit.Owner == playerInfos[localPlayer].owner)
                {
                    ownedUnit.Add(mapcell.unit);
                }
            }
            return base.Init();
        }

        private void InitUI()
        {
            //declare ui elements
            canvas_generalInfo = new Canvas();
            InitCanvas_generalInfo();

            canvas_action = new Canvas();
            InitCanvas_action();

            Label label_mousepos = new Label(" ", new Point(0, 0), new Vector2(80, 20), CONTENT_MANAGER.defaultfont);

            //bind event

            //add to canvas
            canvas.AddElement("generalInfo", canvas_generalInfo);
            canvas.AddElement("action", canvas_action);
            canvas.AddElement("label_mousepos", label_mousepos);
        }

        private void InitCanvas_generalInfo()
        {
            //declare ui elements
            Label label_terraintype = new Label(" ", new Point(465, 365), new Vector2(50, 20), CONTENT_MANAGER.arcadefont);
            label_terraintype.Scale = 0.75f;
            Label label_unittype = new Label(" ", new Point(300, 365), new Vector2(80, 20), CONTENT_MANAGER.arcadefont);

            //bind event

            //add to canvas
            canvas_generalInfo.AddElement("label_terraintype", label_terraintype);
            canvas_generalInfo.AddElement("label_unittype", label_unittype);
        }

        /* action button layout
         * 
         *      540  570  600  630  660  690
         * 346
         * 380
         * 414
         * 448
         */
        private void InitCanvas_action()
        {
            //declare ui elements
            InitCanvas_Factory();
            InitCanvas_Airport();
            InitCanvas_Harbor();

            //bind event

            //add to canvas
            canvas_action.AddElement("canvas_Factory", canvas_action_Factory);
            canvas_action.AddElement("canvas_Airport", canvas_action_Airport);
            canvas_action.AddElement("canvas_Harbor", canvas_action_Harbor);
        }

        private void InitCanvas_Factory()
        {
            canvas_action_Factory = new Canvas();
            canvas_action_Factory.IsVisible = false;

            //hàng 1
            Button button_Soldier = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Soldier,playerInfos[localPlayer].owner), new Point(540, 346), 0.5f);
            Button button_Mech = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Mech, playerInfos[localPlayer].owner), new Point(570, 346), 0.5f);
            Button button_Recon = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Recon, playerInfos[localPlayer].owner), new Point(600, 346), 0.5f);
            Button button_APC = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.APC, playerInfos[localPlayer].owner), new Point(630, 346), 0.5f);
            Button button_Tank = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Tank, playerInfos[localPlayer].owner), new Point(660, 346), 0.5f);
            Button button_H_Tank = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.HeavyTank, playerInfos[localPlayer].owner), new Point(690, 346), 0.5f);

            //hàng 2
            Button button_Artillery = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Artillery, playerInfos[localPlayer].owner), new Point(540, 380), 0.5f);
            Button button_Rocket = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Rocket, playerInfos[localPlayer].owner), new Point(570, 380), 0.5f);
            Button button_AntiAir = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.AntiAir, playerInfos[localPlayer].owner), new Point(600, 380), 0.5f);
            Button button_Missile = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Missile, playerInfos[localPlayer].owner), new Point(630, 380), 0.5f);

            List<Button> tempbuttonlist = new List<Button>();
            tempbuttonlist.Add(button_Soldier);
            tempbuttonlist.Add(button_Mech);
            tempbuttonlist.Add(button_Recon);
            tempbuttonlist.Add(button_APC);
            tempbuttonlist.Add(button_Tank);
            tempbuttonlist.Add(button_H_Tank);
            tempbuttonlist.Add(button_Artillery);
            tempbuttonlist.Add(button_Rocket);
            tempbuttonlist.Add(button_AntiAir);
            tempbuttonlist.Add(button_Missile);

            #region bind event
            foreach (Button button in tempbuttonlist)
            {
                button.MouseClick += (sender, e) =>
                {
                    selectedUnitToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
                };
            }
            #endregion

            canvas_action_Factory.AddElement("button_Soldier", button_Soldier);
            canvas_action_Factory.AddElement("button_Mech", button_Mech);
            canvas_action_Factory.AddElement("button_Recon", button_Recon);
            canvas_action_Factory.AddElement("button_APC", button_APC);
            canvas_action_Factory.AddElement("button_Tank", button_Tank);
            canvas_action_Factory.AddElement("button_H_Tank", button_H_Tank);
            canvas_action_Factory.AddElement("button_Artillery", button_Artillery);
            canvas_action_Factory.AddElement("button_Rocket", button_Rocket);
            canvas_action_Factory.AddElement("button_AntiAir", button_AntiAir);
            canvas_action_Factory.AddElement("button_Missile", button_Missile);
        }

        private void InitCanvas_Airport()
        {
            canvas_action_Airport = new Canvas();
        }

        private void InitCanvas_Harbor()
        {
            canvas_action_Harbor = new Canvas();
        }
        #endregion

        public override void Shutdown()
        {
            session.map = null;
            minimap?.Dispose();
            minimap = null;
        }

        public override void Update(GameTime gameTime)
        {
            mouseInputState = CONTENT_MANAGER.inputState.mouseState;
            lastMouseInputState = CONTENT_MANAGER.lastInputState.mouseState;
            keyboardInputState = CONTENT_MANAGER.inputState.keyboardState;
            lastKeyboardInputState = CONTENT_MANAGER.lastInputState.keyboardState;

            if (Utility.HelperFunction.IsKeyPress(Keys.Escape))
            {
                SCREEN_MANAGER.goto_screen("SetupScreen");
                return;
            }

            //update canvas
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
            ((Label)canvas["label_mousepos"]).Text = mouseInputState.Position.ToString();
            UpdateCanvas_generalInfo();

            //camera control
            MoveCamera(keyboardInputState, mouseInputState);
            selectedMapCell = Utility.HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, session.map.Width, session.map.Height);

            //update minimap
            if (!session.map.IsProcessed)
            {
                minimap = minimapgen.GenerateMapTexture(session.map);
            }

            //update game logic
            if (mouseInputState.LeftButton == ButtonState.Released
             && lastMouseInputState.LeftButton == ButtonState.Pressed)
            {
                SelectUnit();
                SelectBuilding();
            }

            if (isMovingUnitAnimPlaying)
            {
                UpdateMovingUnit(gameTime);
            }

            //calculate movepath
            if (isMovePathCalculated)
            {
                if (movementRange.Contains(selectedMapCell) && selectedMapCell != lastSelectedMapCell)
                {
                    //update movement path
                    movementPath = DijkstraHelper.FindPath(dijkstraGraph, selectedMapCell);
                    dirarrowRenderer.UpdatePath(movementPath);
                    lastSelectedMapCell = selectedMapCell;
                }
            }

            UpdateAnimation(gameTime);
        }

        #region Update game logic

        private void CalculateVision()
        {
            foreach (Unit unit in ownedUnit)
            {

            }
        }

        #region Unit handler
        private void SelectUnit()
        {
            MapCell temp = session.map[selectedMapCell];
            if (temp.unit != null && !isMovingUnitAnimPlaying && selectedUnit != selectedMapCell)
            {
                CONTENT_MANAGER.yes1.Play();

                selectedUnit = selectedMapCell;
                canvas_generalInfo.GetElementAs<Label>("label_unittype").Text = temp.unit.UnitType.ToString() + Environment.NewLine + temp.unit.Owner.ToString();
                DisplayMovementRange(temp.unit, selectedUnit);
                isMovePathCalculated = true;
                currentGameState = GameState.UnitSelected;
            }
            else
            {
                if (!isMovingUnitAnimPlaying)
                {
                    if (movementRange != null && movementRange.Contains(selectedMapCell))
                    {

                        //play sfx
                        CONTENT_MANAGER.moving_out.Play();

                        //we gonna move unit by moving a clone of it then teleport it to the destination
                        destination = selectedMapCell;
                        isMovingUnitAnimPlaying = true;

                        //create a new animation object
                        movingAnim = new MovingUnitAnimation(session.map[selectedUnit].unit, movementPath, new Point(selectedUnit.X * Constants.MapCellWidth, selectedUnit.Y * Constants.MapCellHeight));

                        //ngung vẽ path
                        isMovePathCalculated = false;

                        //ngưng update animation cho unit gốc                        
                        session.map[selectedUnit].unit.Animation.StopAnimation();
                    }
                    else
                    {
                        //bỏ lựa chọn unit sau khi đã chọn unit
                        DeselectUnit();
                    }
                }
            }
        }

        private void DeselectUnit()
        {
            canvas_generalInfo.GetElementAs<Label>("label_unittype").Text = " ";
            movementRange = null;
            movementPath = null;
            isMovePathCalculated = false;
            isMovingUnitAnimPlaying = false;
            selectedUnit = default(Point);
            destination = default(Point);
            currentGameState = GameState.None;
        }
        private void UpdateMovingUnit(GameTime gameTime)
        {
            if (movingAnim.IsArrived)
            {
                //normal stuff
                session.map[destination].unit = session.map[selectedUnit].unit;
                session.map[selectedUnit].unit = null;
                session.map[destination].unit.Animation.ContinueAnimation();
                DeselectUnit();
                return;
            }

            movingAnim.Update(gameTime);
        }

        private void DisplayMovementRange(Unit unit,Point position)
        {
            dijkstraGraph = DijkstraHelper.CalculateGraph(session.map, unit, position);
            movementRange = DijkstraHelper.FindRange(dijkstraGraph);
        }
        #endregion

        private void SelectBuilding()
        {
            MapCell temp = session.map[selectedMapCell];
            if (temp.unit == null
             && isBuildingThatProduceUnit(temp.terrain)
             && currentPlayer == localPlayer
             && temp.owner == playerInfos[localPlayer].owner
             && selectedBuilding != selectedMapCell)
            {
                
                switch (temp.terrain)
                {
                    case TerrainType.Factory:
                        canvas_action_Factory.IsVisible = true;
                        selectedBuilding = selectedMapCell;
                        currentGameState = GameState.BuildingSelected;
                        break;
                    case TerrainType.AirPort:
                        canvas_action_Airport.IsVisible = true;
                        selectedBuilding = selectedMapCell;
                        currentGameState = GameState.BuildingSelected;
                        break;
                    case TerrainType.Harbor:
                        canvas_action_Harbor.IsVisible = true;
                        selectedBuilding = selectedMapCell;
                        currentGameState = GameState.BuildingSelected;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (!actionbound.Contains(mouseInputState.Position))
                {
                    DeselectBuilding();
                }
            }

            if (selectedUnitToBuild != UnitType.None)
            {
                SpawnUnit(selectedUnitToBuild, playerInfos[localPlayer], selectedBuilding);
                selectedUnitToBuild = UnitType.None;
            }
        }

        private void DeselectBuilding()
        {
            selectedBuilding = default(Point);
            canvas_action_Factory.IsVisible = false;
            canvas_action_Airport.IsVisible = false;
            canvas_action_Harbor.IsVisible = false;
            currentGameState = GameState.None;
        }

        private bool SpawnUnit(UnitType unittype,PlayerInfo owner,Point location)
        {
            MapCell spawnlocation = session.map[location];
            if (spawnlocation != null
              &&spawnlocation.unit == null)
            {
                Unit temp = UnitCreationHelper.Create(unittype, owner.owner);
                temp.UnitID = 0;
                session.map[location].unit = temp;
                return true;
            }

            return false;
        }

        private bool isBuildingThatProduceUnit(TerrainType t)
        {
            switch (t)
            {
                case TerrainType.Factory:
                case TerrainType.AirPort:
                case TerrainType.Harbor:
                    return true;
                default:
                    break;
            }
            return false;
        }


        #endregion

        #region Update function helper

        private void UpdateCanvas_generalInfo()
        {
            canvas_generalInfo.GetElementAs<Label>("label_terraintype").Text = session.map[selectedMapCell].terrain.ToString() + Environment.NewLine + session.map[selectedMapCell].owner.ToString();
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            foreach (MapCell mapcell in session.map)
            {
                if (mapcell.unit != null)
                {
                    mapcell.unit.Animation.Update(gameTime);
                }
            }
        }

        private void MoveCamera(KeyboardState keyboardInputState, MouseState mouseInputState)
        {
            //simulate scrolling
            if (keyboardInputState.IsKeyDown(Keys.Left) || keyboardInputState.IsKeyDown(Keys.A) || mouseInputState.Position.X.Between(20, 0))
            {
                camera.Location += new Vector2(-1, 0) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Right) || keyboardInputState.IsKeyDown(Keys.D) || mouseInputState.Position.X.Between(720, 700))
            {
                camera.Location += new Vector2(1, 0) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Up) || keyboardInputState.IsKeyDown(Keys.W) || mouseInputState.Position.Y.Between(20, 0))
            {
                camera.Location += new Vector2(0, -1) * 10;
            }
            if (keyboardInputState.IsKeyDown(Keys.Down) || keyboardInputState.IsKeyDown(Keys.S) || mouseInputState.Position.Y.Between(480, 460))
            {
                camera.Location += new Vector2(0, +1) * 10;
            }
            camera.Location = new Vector2(camera.Location.X.Clamp(1680, 0), camera.Location.Y.Clamp(960, 0));
        }

        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            DrawMap(CONTENT_MANAGER.spriteBatch, gameTime);
            
            //draw the guibackground
            CONTENT_MANAGER.spriteBatch.Draw(guibackground, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
            canvas.Draw(CONTENT_MANAGER.spriteBatch);

            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, currentGameState.ToString(), new Vector2(100, 100), Color.Red);

            //draw canvas_generalInfo
            DrawCanvas_generalInfo();


            //draw the minimap
            CONTENT_MANAGER.spriteBatch.Draw(minimap, minimapbound, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, LayerDepth.GuiBackground);
        }

        private void DrawCanvas_generalInfo()
        {
            
        }

        private void DrawMap(SpriteBatch spriteBatch,GameTime gameTime)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);

            //render the map
            MapRenderer.Render(session.map, spriteBatch, gameTime);

            //draw moving animation
            if (isMovingUnitAnimPlaying)
            {
                movingAnim.Draw(spriteBatch, gameTime);
            }

            //draw selected unit's movement range
            if (!isMovingUnitAnimPlaying)
            {
                DrawSelectedUnit(spriteBatch);
            }

            //draw movementpath direction arrow if exist
            if (movementPath != null && isMovePathCalculated)
            {
                dirarrowRenderer.UpdatePath(movementPath);
                dirarrowRenderer.Draw(spriteBatch);
            }

            //draw the cursor
            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), new Rectangle(0, 0, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }

        private void DrawSelectedUnit(SpriteBatch spriteBatch)
        {
            if (movementRange != null)
            {
                foreach (Point dest in movementRange)
                {
                    spriteBatch.Draw(CONTENT_MANAGER.moveOverlay, new Vector2(dest.X * Constants.MapCellWidth, dest.Y * Constants.MapCellHeight), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
                }
            }
        }
        #endregion
    }
}
