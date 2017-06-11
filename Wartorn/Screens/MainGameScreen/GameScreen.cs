using System;
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
        TurnEnd,
        WaitForTurn,
        UnitSelected,
        UnitMove,
        UnitCommand,
        BuildingSelected,
        BuildingBuildUnit,
    }

    class GameScreen : Screen
    {
        #region private field
        //information of this game session
        Session session;
        Map map;

        #region ui canvas
        Canvas canvas;
        Canvas canvas_generalInfo;
        Canvas canvas_action;
        Canvas canvas_action_Factory;
        Canvas canvas_action_Airport;
        Canvas canvas_action_Harbor;
        Canvas canvas_action_Unit;
        Canvas canvas_action_Building;
        #endregion

        //debug console
        UIClass.Console console;

        //cursor
        Texture2D cursor;
        readonly Vector2 selectCursorOffset = new Vector2(6, 6);
        readonly Vector2 attackCursorOffset = new Vector2(14, 14);
        Vector2 cursorOffset;


        //minimap
        Texture2D minimap;
        MiniMapGenerator minimapgen;

        //camera ?
        Camera camera;

        #region input information
        MouseState mouseInputState;
        MouseState lastMouseInputState;
        KeyboardState keyboardInputState;
        KeyboardState lastKeyboardInputState;
        Point selectedMapCell;
        Point lastSelectedMapCell;
        #endregion

        //constants
        readonly Rectangle minimapbound = new Rectangle(2, 312, 234, 166);
        readonly Rectangle actionbound = new Rectangle(536, 340, 182, 138);

        //gui variable
        bool isHideGUI = false;

        #region player information
        PlayerInfo[] playerInfos;
        int currentPlayer = 0;
        int localPlayer = 0;
        List<Guid> ownedUnit;
        #endregion

        //build unit information
        UnitType selectedUnitTypeToBuild = UnitType.None;
        Point selectedBuilding = default(Point);

        //current unit selection
        Point selectedUnit = default(Point);
        Point lastSelectedUnit = default(Point);
        List<Point> movementRange = null;
        List<Point> attackRange = null;

        #region moving unit animation
        Graph dijkstraGraph;
        List<Point> movementPath;
        Point origin;
        Point destination;
        bool isMovePathCalculated = false;
        MovingUnitAnimation movingAnim;
        DirectionArrowRenderer dirarrowRenderer = new DirectionArrowRenderer();
        #endregion

        //fog of war
        bool[,] mapcellVisibility;

        //selected command
        Command selectedCmd = Command.None;

        //game state
        GameState currentGameState = GameState.None;
        #endregion

        public GameScreen(GraphicsDevice device) : base(device, "GameScreen")
        {
            LoadContent();
            minimapgen = new MiniMapGenerator(device, CONTENT_MANAGER.spriteBatch);
        }

        #region Init
        public void InitSession(SessionData sessiondata)
        {
            session = new Session(sessiondata);
            map = session.map;
            minimap = minimapgen.GenerateMapTexture(map);
            playerInfos = sessiondata.playerInfos;
            ownedUnit = new List<Guid>();
            mapcellVisibility = new bool[map.Width, map.Height];

            //init visibility table
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    mapcellVisibility[x, y] = false;
                }
            }

            cursorOffset = selectCursorOffset;
        }

        private void LoadContent()
        {
            cursor = CONTENT_MANAGER.selectCursor;
        }

        public override bool Init()
        {
            //TODO
            //do something to handshake player

            camera = new Camera(_device.Viewport);
            canvas = new Canvas();

            InitUI();

            foreach (MapCell mapcell in map)
            {
                if (mapcell.unit!=null && mapcell.unit.Owner == playerInfos[localPlayer].owner)
                {
                    ownedUnit.Add(mapcell.unit.guid);
                }
            }
            return base.Init();
        }

        #region init ui
        private void InitUI()
        {
            //declare ui elements
            canvas_generalInfo = new Canvas();
            InitCanvas_generalInfo();

            canvas_action = new Canvas();
            InitCanvas_action();

            canvas_action_Unit = new Canvas();
            canvas_action_Unit.IsVisible = false;
            InitCanvas_Unit();

            Label label_mousepos = new Label(" ", new Point(0, 0), new Vector2(80, 20), CONTENT_MANAGER.defaultfont);

            console = new UIClass.Console(new Point(0, 0), new Vector2(720, 200), CONTENT_MANAGER.hackfont);
            console.IsVisible = false;
            console.SetVariable("player", localPlayer);
            console.SetVariable("changeTurn", new Action(this.ChangeTurn));
            console.SetVariable("round", new Action(this.RoundTurn));

            //bind event

            //add to canvas
            canvas.AddElement("generalInfo", canvas_generalInfo);
            canvas.AddElement("action", canvas_action);
            canvas.AddElement("unit", canvas_action_Unit);
            //canvas.AddElement("label_mousepos", label_mousepos);
            canvas.AddElement("console", console);
        }


        private void InitCanvas_Unit()
        {
            //declare ui elements
            PictureBox commandslot = new PictureBox(CONTENT_MANAGER.commandspritesheet, Point.Zero, CommandSpriteSourceRectangle.GetSprite(playerInfos[localPlayer].owner == GameData.Owner.Red ? SpriteSheetCommandSlot.oneslotred : SpriteSheetCommandSlot.oneslotblue), null, depth: LayerDepth.GuiBackground);

            Button firstslot = new Button(CONTENT_MANAGER.commandspritesheet, Rectangle.Empty, Point.Zero);
            Button secondslot = new Button(CONTENT_MANAGER.commandspritesheet, Rectangle.Empty, Point.Zero);
            Button thirdslot = new Button(CONTENT_MANAGER.commandspritesheet, Rectangle.Empty, Point.Zero);

            //firstslot.isDrawRect = true;
            //thirdslot.isDrawRect = true;
            //secondslot.isDrawRect = true;

            //bind event
            firstslot.MouseClick += (sender, e) =>
            {
                selectedCmd = CommandSpriteSourceRectangle.GetCommand(firstslot.spriteSourceRectangle);
            };
            secondslot.MouseClick += (sender, e) =>
            {
                selectedCmd = CommandSpriteSourceRectangle.GetCommand(secondslot.spriteSourceRectangle);
            };
            thirdslot.MouseClick += (sender, e) =>
            {
                selectedCmd = CommandSpriteSourceRectangle.GetCommand(thirdslot.spriteSourceRectangle);
            };

            //add to canvas
            canvas_action_Unit.AddElement("commandslot", commandslot);
            canvas_action_Unit.AddElement("firstslot", firstslot);
            canvas_action_Unit.AddElement("secondslot", secondslot);
            canvas_action_Unit.AddElement("thirdslot", thirdslot);
        }

        private void InitCanvas_generalInfo()
        {
            //declare ui elements
            PictureBox picbox_generalInfoBorder = new PictureBox(CONTENT_MANAGER.generalInfo_border, new Point(175, 364), Rectangle.Empty, Vector2.Zero,depth: LayerDepth.GuiBackground);

            PictureBox picbox_terrainType = new PictureBox(CONTENT_MANAGER.background_terrain, new Point(183, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower - 0.05f);
            Label label_terrainType = new Label(" ", new Point(186, 370), new Vector2(50, 20), CONTENT_MANAGER.hackfont);
            label_terrainType.Origin = new Vector2(-3, 2);
            label_terrainType.Scale = 0.75f;
            PictureBox picbox_unitType = new PictureBox(CONTENT_MANAGER.background_unit, new Point(183, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);
            Label label_unitType = new Label(" ", new Point(183, 464), new Vector2(80, 20), CONTENT_MANAGER.hackfont);
            label_unitType.Origin = new Vector2(-3, 2);
            label_unitType.Scale = 0.75f;

            PictureBox picbox_generalInfoCapturePoint = new PictureBox(CONTENT_MANAGER.generalInfo_capturePoint, new Point(262, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);
            Label label_generalInfoCapturePoint = new Label(" ", new Point(), new Vector2(50, 20), CONTENT_MANAGER.arcadefont);
            label_generalInfoCapturePoint.Origin = new Vector2(-3, 2);

            PictureBox picbox_generalInfoDefenseStar = new PictureBox(CONTENT_MANAGER.generalInfo_defenseStar, new Point(265, 371), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);

            PictureBox picbox_generalInfoUnitInfo = new PictureBox(CONTENT_MANAGER.generalInfo_unitInfo, new Point(181, 453), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower + 0.05f);
            PictureBox picbox_generalInfoLoadedUnit = new PictureBox(CONTENT_MANAGER.generalInfo_loadedUnit, new Point(181, 435), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);

            PictureBox picbox_generalInfoHPbar = new PictureBox(CONTENT_MANAGER.generalInfo_HPbar, new Point(198, 458), null, Vector2.Zero, depth: LayerDepth.GuiUpper);

            Label label_generalInfoHP = new Label(" ", new Point(301, 455), new Vector2(20, 20), CONTENT_MANAGER.arcadefont);
            label_generalInfoHP.Origin = new Vector2(1, 2);
            label_generalInfoHP.Scale = 0.75f;
            Label label_generalInfoUnitInfo_fuel = new Label(" ", new Point(263, 465), new Vector2(20, 20), CONTENT_MANAGER.arcadefont);
            label_generalInfoUnitInfo_fuel.Origin = new Vector2(-3, 2);
            label_generalInfoUnitInfo_fuel.Scale = 0.75f;
            Label label_generalInfoUnitInfo_ammo = new Label(" ", new Point(294, 465), new Vector2(20, 20), CONTENT_MANAGER.arcadefont);
            label_generalInfoUnitInfo_ammo.Origin = new Vector2(-3, 2);
            label_generalInfoUnitInfo_ammo.Scale = 0.75f;

            //bind event

            //add to canvas
            canvas_generalInfo.AddElement("picbox_generalInfoBorder", picbox_generalInfoBorder);

            canvas_generalInfo.AddElement("picbox_generalInfoCapturePoint", picbox_generalInfoCapturePoint);
            canvas_generalInfo.AddElement("label_generalInfoCapturePoint", label_generalInfoCapturePoint);

            canvas_generalInfo.AddElement("picbox_generalInfoDefenseStar", picbox_generalInfoDefenseStar);

            canvas_generalInfo.AddElement("picbox_generalInfoUnitInfo", picbox_generalInfoUnitInfo);
            canvas_generalInfo.AddElement("picbox_generalInfoLoadedUnit", picbox_generalInfoLoadedUnit);

            canvas_generalInfo.AddElement("picbox_generalInfoHPbar", picbox_generalInfoHPbar);

            canvas_generalInfo.AddElement("label_generalInfoHP", label_generalInfoHP);
            canvas_generalInfo.AddElement("label_generalInfoUnitInfo_fuel", label_generalInfoUnitInfo_fuel);
            canvas_generalInfo.AddElement("label_generalInfoUnitInfo_ammo", label_generalInfoUnitInfo_ammo);


            canvas_generalInfo.AddElement("picbox_terrainType", picbox_terrainType);
            canvas_generalInfo.AddElement("label_terrainType", label_terrainType);

            canvas_generalInfo.AddElement("picbox_unitType", picbox_unitType);
            canvas_generalInfo.AddElement("label_unitType", label_unitType);
        }

        #region legacy
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
                    selectedUnitTypeToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
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
            canvas_action_Airport.IsVisible = false;

            //hàng 1
            Button button_transportcopter = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.TransportCopter, playerInfos[localPlayer].owner), new Point(540, 346), 0.5f);
            Button button_battlecopter = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.BattleCopter, playerInfos[localPlayer].owner), new Point(570, 346), 0.5f);
            Button button_fighter = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Fighter, playerInfos[localPlayer].owner), new Point(600, 346), 0.5f);
            Button button_bomber = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Bomber, playerInfos[localPlayer].owner), new Point(630, 346), 0.5f);
            
            List<Button> tempbuttonlist = new List<Button>();
            tempbuttonlist.Add(button_transportcopter);
            tempbuttonlist.Add(button_battlecopter);
            tempbuttonlist.Add(button_fighter);
            tempbuttonlist.Add(button_bomber);
            

            #region bind event
            foreach (Button button in tempbuttonlist)
            {
                button.MouseClick += (sender, e) =>
                {
                    selectedUnitTypeToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
                };
            }
            #endregion

            canvas_action_Airport.AddElement("button_transportcopter", button_transportcopter);
            canvas_action_Airport.AddElement("button_battlecopter", button_battlecopter);
            canvas_action_Airport.AddElement("button_fighter", button_fighter);
            canvas_action_Airport.AddElement("button_bomber", button_bomber);
        }

        private void InitCanvas_Harbor()
        {
            canvas_action_Harbor = new Canvas();
            canvas_action_Harbor.IsVisible = false;

            //hàng 1
            Button button_lander = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Lander, playerInfos[localPlayer].owner), new Point(540, 346), 0.5f);
            Button button_cruiser = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Cruiser, playerInfos[localPlayer].owner), new Point(570, 346), 0.5f);
            Button button_submarine = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Submarine, playerInfos[localPlayer].owner), new Point(600, 346), 0.5f);
            Button button_battleship = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Battleship,playerInfos[localPlayer].owner), new Point(630, 346), 0.5f);

            List<Button> tempbuttonlist = new List<Button>();
            tempbuttonlist.Add(button_lander);
            tempbuttonlist.Add(button_cruiser);
            tempbuttonlist.Add(button_submarine);
            tempbuttonlist.Add(button_battleship);


            #region bind event
            foreach (Button button in tempbuttonlist)
            {
                button.MouseClick += (sender, e) =>
                {
                    selectedUnitTypeToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
                };
            }
            #endregion

            canvas_action_Harbor.AddElement("button_lander", button_lander);
            canvas_action_Harbor.AddElement("button_cruiser", button_cruiser);
            canvas_action_Harbor.AddElement("button_submarine", button_submarine);
            canvas_action_Harbor.AddElement("button_battleship", button_battleship);
        }
        #endregion

        #endregion

        #endregion

        public override void Shutdown()
        {
            map = null;
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

            if (HelperFunction.IsKeyPress(Keys.OemTilde))
            {
                console.IsVisible = !console.IsVisible;
            }

            //update canvas
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
            //((Label)canvas["label_mousepos"]).Text = mouseInputState.Position.ToString();
            UpdateCanvas_generalInfo();

            //camera control
            if (currentGameState == GameState.None || currentGameState == GameState.UnitSelected || currentGameState == GameState.BuildingSelected )
            {
                MoveCamera(keyboardInputState, mouseInputState);
            }
            selectedMapCell = HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, map.Width, map.Height);

            //update minimap
            if (!map.IsProcessed)
            {
                minimap = minimapgen.GenerateMapTexture(map);
            }

            //update game logic
            switch (currentGameState)
            {
                #region GameState.None
                //the normal state of the game where nothing is selected
                case GameState.None:
                    if (HelperFunction.IsLeftMousePressed())
                    {
                        if (SelectUnit())
                        {
                            //get information of currently selected unit...
                            CONTENT_MANAGER.yes1.Play();
                            selectedUnit = selectedMapCell;
                            origin = selectedMapCell;
                            MapCell temp = map[selectedUnit];
                            CalculateMovementRange(temp.unit, selectedUnit);
                            movementPath = null;

                            currentGameState = GameState.UnitSelected;
                            break;
                        }

                        if (SelectBuilding())
                        {
                            selectedBuilding = selectedMapCell;
                            selectedUnitTypeToBuild = UnitType.None;

                            ShowBuildingMenu();

                            currentGameState = GameState.BuildingSelected;
                            break;
                        }
                    }
                    break;
                #endregion

                #region GameState.UnitSelected
                //previous state: None
                //show movement range
                //show movement path planning
                //next state: UnitMove    : if a tile inside movement range is selected
                //            UnitCommand : if the tile of the selected unit is selected again
                //            None        : if a tile outside movement range is selected or <Cancel> is pressed
                case GameState.UnitSelected:
                    //check if <cancel> is pressed or move outside range
                    if (HelperFunction.IsRightMousePressed())
                    {
                        //clear selectedUnit's information
                        currentGameState = GameState.None;
                        break;
                    }

                    //check if movement path calculated
                    if (!isMovePathCalculated)
                    {
                        //calculate move path
                        if (movementRange.Contains(selectedMapCell) && selectedMapCell != lastSelectedMapCell && map[selectedMapCell].unit == null)
                        {
                            //update movement path
                            movementPath = DijkstraHelper.FindPath(dijkstraGraph, selectedMapCell);
                            lastSelectedMapCell = selectedMapCell;
                        }
                    }

                    //check if a tile is selected is in the movementRange
                    if (HelperFunction.IsLeftMousePressed() && movementRange.Contains(selectedMapCell) && map[selectedMapCell].unit == null)
                    {
                        StartMovingUnitAnimation();
                        currentGameState = GameState.UnitMove;
                        break;
                    }

                    //check if the currently selected unit is selected again
                    if (HelperFunction.IsLeftMousePressed() && selectedMapCell == selectedUnit)
                    {
                        //show command menu
                        ShowCommandMenu();
                        currentGameState = GameState.UnitCommand;
                        break;
                    }
                    break;
                #endregion

                #region GameState.UnitMove
                //previous state: UnitSelected
                //update and draw unit move animation
                //next state: UnitCommand  : end moving animation
                //            UnitSelected : if <Cancel> is pressed
                //            None         : if the unit is out of action point
                case GameState.UnitMove:
                    //check if <cancel> is pressed
                    if (HelperFunction.IsRightMousePressed() && movingAnim.IsArrived)
                    {
                        //gobackto unitselected
                        RevertMovingUnitAnimation();
                        currentGameState = GameState.UnitSelected;
                        break;
                    }

                    //check if unit has arrived and not already teleported
                    if (movingAnim.IsArrived && map[origin].unit!=null)
                    {
                        //teleport stuff
                        map.TeleportUnit(origin, destination);

                        //save selectedUnit
                        lastSelectedUnit = selectedUnit;
                        //move selectedUnit to destination;
                        selectedUnit = destination;

                        //check if the unit's action point is above zero
                        //TODO make sure that the unit can only move once
                        if (map[selectedUnit].unit.PeakUpdateActionPoint(Command.Move) > 0)
                        {
                            map[selectedUnit].unit.Animation.ContinueAnimation();
                            //show command menu
                            ShowCommandMenu();
                            //goto unitcommand
                            currentGameState = GameState.UnitCommand;
                        }
                        else
                        {
                            //Execute Command.Wait for this unit
                            selectedCmd = Command.Wait;

                            //goto Command
                            currentGameState = GameState.UnitCommand;
                        }
                        break;
                    }
                    else
                    {
                        //update moving animation
                        movingAnim.Update(gameTime);
                    }
                    break;
                #endregion

                #region GameState.UnitCommand
                //previous state: UnitSelected,UnitMove
                //select and execute unit command
                //next state: None         : if a command is selected
                //            UnitSelected : if <cancel> is pressed
                case GameState.UnitCommand:
                    //check if <cancel> is pressed
                    if (HelperFunction.IsRightMousePressed())
                    {
                        RevertMovingUnitAnimation();

                        canvas_action_Unit.IsVisible = false;
                        //revert any movement if exist
                        //RevertMovingUnitAnimation();
                        currentGameState = GameState.UnitSelected;
                        break;
                    }

                    Unit tempunit = map[selectedUnit].unit;
                    switch (selectedCmd)
                    {
                        case Command.Wait:
                            tempunit.UpdateActionPoint(Command.Wait);
                            goto finalise_command_execution;

                        #region Command.Attack
                        case Command.Attack:
                            //change cursor to attack cursor
                            cursor = CONTENT_MANAGER.attackCursor;
                            cursorOffset = attackCursorOffset;

                            if (HelperFunction.IsLeftMousePressed() 
                             && attackRange.Contains(selectedMapCell) 
                             && map[selectedMapCell].unit!=null
                             && map[selectedMapCell].unit.Owner!=playerInfos[localPlayer].owner)
                            {
                                //change cursor back
                                cursor = CONTENT_MANAGER.selectCursor;
                                cursorOffset = selectCursorOffset;

                                tempunit.UpdateActionPoint(Command.Attack);

                                //do attack stuff
                                Unit otherunit = map[selectedMapCell].unit;
                                var result = Unit.GetCalculatedDamage(map[selectedUnit], map[selectedMapCell]);
                                CONTENT_MANAGER.ShowMessageBox(string.Format("{0} attack {1}: {2}", tempunit.UnitType, otherunit.UnitType, result));
                                tempunit.HitPoint = result.attackerHP;
                                otherunit.HitPoint = result.defenderHP;

                                if (tempunit.HitPoint==0)
                                {
                                    map.RemoveUnit(selectedUnit);
                                }
                                //end command
                                goto finalise_command_execution;
                            }
                            break;
                        #endregion

                        case Command.Capture:

                            if (isBuilding(map[selectedUnit].terrain)
                             && map[selectedUnit].owner != playerInfos[localPlayer].owner
                             && tempunit.CapturePoint == 0)
                            {
                                map[selectedUnit].unit.CapturePoint = 20;
                            }

                            tempunit.CapturePoint -= tempunit.HitPoint;
                            tempunit.UpdateActionPoint(Command.Capture);

                            if (tempunit.CapturePoint<=0)
                            {
                                map[selectedUnit].owner = playerInfos[localPlayer].owner;
                                map.IsProcessed = false;
                                tempunit.CapturePoint = 0;
                            }

                            goto finalise_command_execution;

                        case Command.Load:
                            break;

                        case Command.Drop:
                            break;

                        case Command.Rise:
                            break;

                        case Command.Dive:
                            break;

                        case Command.Supply:
                            break;

                        case Command.Move:
                            //substract actionpoint
                            map[selectedUnit].unit.UpdateActionPoint(Command.Move);

                            //substract fuel
                            map[selectedUnit].unit.Fuel -= movementPath != null ? movementPath.Count : 0;
                            break;

                        default:
                            break;
                    }
                    break;

                    finalise_command_execution:
                    {
                        if (tempunit.ActionPoint == 0)
                        {
                            tempunit.Animation.PlayAnimation(AnimationName.done.ToString());
                        }
                        selectedCmd = Command.None;

                        //update fuel
                        int fuel = movementPath.Count;

                        tempunit.Fuel = (tempunit.Fuel - fuel).Clamp(Unit._Gas[tempunit.UnitType], 0);

                        cursor = CONTENT_MANAGER.selectCursor;
                        cursorOffset = selectCursorOffset;

                        //hide command menu
                        HideCommandMenu();
                        canvas_action_Unit.IsVisible = false;

                        //goto none
                        currentGameState = GameState.None;
                    }
                    break;
                #endregion

                #region GameState.BuildingSelected
                //previous state: None
                //show building's canvas
                //next state: None              : if <Cancel> is pressed
                //            BuildingBuildUnit : if a Unit is selected to build
                case GameState.BuildingSelected:
                    //check if <cancel> is pressed or move outside range
                    if (HelperFunction.IsRightMousePressed())
                    {
                        //hide building menu
                        canvas_action_Factory.IsVisible = false;
                        canvas_action_Airport.IsVisible = false;
                        canvas_action_Harbor.IsVisible = false;

                        //goto none
                        currentGameState = GameState.None;
                        break;
                    }

                    //check if there is a unit selected to build
                    if (selectedUnitTypeToBuild != UnitType.None)
                    {
                        currentGameState = GameState.BuildingBuildUnit;
                        break;
                    }

                    break;
                #endregion

                #region GameState.BuildingBuildUnit
                //previous state: BuildingSelected:
                //spawn the unit which was selected to build
                //next state: None
                case GameState.BuildingBuildUnit:

                    //spawn the selected unit
                    SpawnUnit(selectedUnitTypeToBuild, playerInfos[localPlayer], selectedBuilding);

                    //hide all the building menu
                    canvas_action_Factory.IsVisible = false;
                    canvas_action_Airport.IsVisible = false;
                    canvas_action_Harbor.IsVisible = false;

                    //goto none
                    currentGameState = GameState.None;
                    break;
                #endregion

                default:
                    break;
            }

            UpdateAnimation(gameTime);
        }


        #region Update game logic

        #region Execute command
        private void ExecuteCommand(Command cmd)
        {
            //CONTENT_MANAGER.ShowMessageBox(cmd.ToString());
            Unit tempunit = map[selectedUnit].unit;

            //substract action point
            

            switch (cmd)
            {
                case Command.Wait:
                    break;
                case Command.Attack:
                    break;
                case Command.Capture:
                    break;
                case Command.Load:
                    break;
                case Command.Drop:
                    break;
                case Command.Rise:
                    break;
                case Command.Dive:
                    break;
                case Command.Supply:
                    break;
                case Command.Move:
                    break;
                default:
                    break;
            }

            if (tempunit.ActionPoint == 0)
            {
                tempunit.Animation.PlayAnimation(AnimationName.done.ToString());
            }
        }
        #endregion

        #region calculate vision
        private void CalculateVision()
        {
            foreach (Guid id in ownedUnit)
            {
                
            }
        }
        #endregion

        private int GetCommands()
        {
            //có wait nè
            int temp = (int)Command.Wait;

            //có attack nếu có Unit địch trong tầm tấn công và tầm nhìn nè
            //todo làm tầm nhìn
            foreach (Point p in attackRange)
            {
                if (map[p].unit!=null 
                    //the below check is only for debug
                 //&& map[p].unit.Owner != map[selectedUnit].unit.Owner) 
                    //the below check is used in final game
               && !ownedUnit.Contains(map[p].unit.guid))
                {
                    temp += (int)Command.Attack;
                    break;
                }
            }

            //có load nếu bên cạnh transport unit nè
            //target select the transport unit
            //like goto the mapcell that is next to the transporter
            //then select command load and then select said transporter
            //then you are loaded in said transporter

            //có drop unit đang chở unit khác và bên cạnh là ô đi được cho unit đó.

            //có capture nếu là lính và đang đứng trên building khác màu nè
            if ((map[selectedUnit].unit.UnitType == UnitType.Soldier
             || map[selectedUnit].unit.UnitType == UnitType.Mech)
             && isBuilding(map[selectedUnit].terrain)
             && map[selectedUnit].owner!= playerInfos[localPlayer].owner)
            {
                temp += (int)Command.Capture;
            }

            //có supply nếu là apc và đang đứng cạnh 1 unit bạn nè

            
            return temp;
        }

        private void ShowCommandMenu()
        {
            canvas_action_Unit.IsVisible = true;

            CalculateAttackRange(map[selectedUnit].unit, selectedUnit);
            int comds = GetCommands();

            var cmds = comds.GetContainCommand();

            string ttemp = string.Empty;
            foreach (Command cmd in cmds)
            {
                ttemp += cmd.ToString() + '\n';
            }
            //CONTENT_MANAGER.ShowMessageBox(ttemp);

            Rectangle cmdslot = CommandSpriteSourceRectangle.GetSprite(cmds.Count, playerInfos[localPlayer].owner);

            canvas_action_Unit.GetElementAs<PictureBox>("commandslot").SourceRectangle = cmdslot;
            Point cmdslotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 50, selectedUnit.Y * Constants.MapCellHeight);
            canvas_action_Unit.GetElementAs<PictureBox>("commandslot").Position = camera.TranslateFromWorldToScreen(cmdslotPosition.ToVector2()).ToPoint();

            Button firstslot = canvas_action_Unit.GetElementAs<Button>("firstslot");
            Button secondslot = canvas_action_Unit.GetElementAs<Button>("secondslot");
            Button thirdslot = canvas_action_Unit.GetElementAs<Button>("thirdslot");

            Point slotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 50 + 6, selectedUnit.Y * Constants.MapCellHeight + 8);
            slotPosition = camera.TranslateFromWorldToScreen(slotPosition.ToVector2()).ToPoint();

            firstslot.Position = slotPosition;
            firstslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[0]);
            firstslot.rect = new Rectangle(firstslot.Position, firstslot.spriteSourceRectangle.Size);

            if (cmds.Count>1)
            {
                slotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 50 + 6, selectedUnit.Y * Constants.MapCellHeight + 16 + 8);
                slotPosition = camera.TranslateFromWorldToScreen(slotPosition.ToVector2()).ToPoint();
                secondslot.Position = slotPosition;
                secondslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[1]);
                secondslot.rect = new Rectangle(secondslot.Position, secondslot.spriteSourceRectangle.Size);
            }
            if (cmds.Count > 2)
            {
                slotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 50 + 6, selectedUnit.Y * Constants.MapCellHeight + 32 + 8);
                slotPosition = camera.TranslateFromWorldToScreen(slotPosition.ToVector2()).ToPoint();
                thirdslot.Position = slotPosition;
                thirdslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[2]);
                thirdslot.rect = new Rectangle(thirdslot.Position, thirdslot.spriteSourceRectangle.Size);
            }
        }

        private void HideCommandMenu()
        {
            canvas_action_Unit.IsVisible = false;

            canvas_action_Unit.GetElementAs<Button>("firstslot").rect = Rectangle.Empty;
            canvas_action_Unit.GetElementAs<Button>("secondslot").rect = Rectangle.Empty;
            canvas_action_Unit.GetElementAs<Button>("thirdslot").rect = Rectangle.Empty;

            canvas_action_Unit.GetElementAs<Button>("firstslot").spriteSourceRectangle = Rectangle.Empty;
            canvas_action_Unit.GetElementAs<Button>("secondslot").spriteSourceRectangle = Rectangle.Empty;
            canvas_action_Unit.GetElementAs<Button>("thirdslot").spriteSourceRectangle = Rectangle.Empty;
        }

        #region Unit handler

        private bool SelectUnit()
        {
            MapCell temp = map[selectedMapCell];
            return
             (
                //check if there is a unit to select
                temp.unit != null
                //check if 
                && temp.unit.ActionPoint > 0
                //check if this is the local player's turn
                && currentPlayer == localPlayer
                //check if this unit is the local player's unit
                && temp.unit.Owner == playerInfos[localPlayer].owner
             );
        }

        private void StartMovingUnitAnimation()
        {
            //destination confirmed, moving to destination
            Unit tempunit = map[selectedUnit].unit;

            //play sfx
            CONTENT_MANAGER.moving_out.Play();

            //we gonna move unit by moving a clone of it then teleport it to the destination
            destination = selectedMapCell;

            //create a new animation object
            movingAnim = new MovingUnitAnimation(map[selectedUnit].unit, movementPath, new Point(selectedUnit.X * Constants.MapCellWidth, selectedUnit.Y * Constants.MapCellHeight));

            //ngung vẽ path
            isMovePathCalculated = false;

            //ngưng update animation cho unit gốc                        
            tempunit.Animation.StopAnimation();
        }

        private void RevertMovingUnitAnimation()
        {
            if (map[origin].unit == null)
            {
                map.TeleportUnit(selectedUnit, origin);
                selectedUnit = origin;
            }
        }

        //deperecated do not use
        private void DeselectUnit()
        {
            canvas_generalInfo.GetElementAs<Label>("label_unittype").Text = " ";
            movementRange = null;
            movementPath = null;
            isMovePathCalculated = false;
            lastSelectedUnit = selectedUnit;
            selectedUnit = destination;
            destination = default(Point);
            if (currentGameState == GameState.UnitSelected)
            {
                currentGameState = GameState.UnitCommand;
            }
        }
        
        private void CalculateMovementRange(Unit unit,Point position)
        {
            dijkstraGraph = DijkstraHelper.CalculateGraph(map, unit, position);
            movementRange = DijkstraHelper.FindRange(dijkstraGraph);
        }

        private void CalculateAttackRange(Unit unit,Point position)
        {
            Range atkrange = unit.GetAttackkRange();
            attackRange = new List<Point>();

            //use to clamp the attack range inside the map
            int minx = (position.X - atkrange.Max).Clamp(position.X, 0);
            int maxx = (position.X + atkrange.Max).Clamp(map.Width-1, position.X);
            int miny = (position.Y - atkrange.Max).Clamp(position.Y, 0);
            int maxy = (position.Y + atkrange.Max).Clamp(map.Height-1, position.Y);

            for (int x = minx; x <= maxx; x++)
            {
                for (int y = miny; y <= maxy; y++)
                {
                    Point temp = new Point(x, y);
                    int dist = (int)temp.DistanceToOther(position, true);
                    if (dist>=atkrange.Min && dist<=atkrange.Max)
                    {
                        attackRange.Add(temp);
                    }
                }
            }

            StringBuilder attackrange = new StringBuilder();
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Point temp = new Point(x, y);
                    if (attackRange.Contains(temp))
                    {
                        attackrange.Append("*");
                    }
                    else
                    {
                        attackrange.Append(" ");
                    }
                }
                attackrange.AppendLine();
            }
            File.WriteAllText("attackrange.txt",attackrange.ToString());
        }
        #endregion

        #region only use to demo gameplay these will not be used in game

        private void RoundTurn()
        {
            ChangeTurn();
            ChangeTurn();
        }

        private void ChangeTurn()
        {
            foreach (Point p in map.mapcellthathaveunit)
            {
                map[p].unit.UpdateActionPoint(Command.None);
                map[p].unit.Animation.PlayAnimation(AnimationName.idle.ToString());
            }

            if (currentPlayer == 1)
            {
                currentPlayer = 0;
            }
            else
            {
                currentPlayer = 1;
            }

            if (localPlayer == 1)
            {
                localPlayer = 0;
            }
            else
            {
                localPlayer = 1;
            }

            ChangeUnitCanvasColor(playerInfos[currentPlayer].owner);
        }

        private void ChangeUnitCanvasColor(GameData.Owner owner)
        {
            foreach (string uiname in canvas_action_Factory.UInames)
            {
                Rectangle temp = canvas_action_Factory.GetElementAs<Button>(uiname).spriteSourceRectangle;
                UnitType tempunittype = UnitSpriteSheetRectangle.GetUnitType(temp);
                canvas_action_Factory.GetElementAs<Button>(uiname).spriteSourceRectangle = UnitSpriteSheetRectangle.GetSpriteRectangle(tempunittype, owner);
            }
            foreach (string uiname in canvas_action_Airport.UInames)
            {
                Rectangle temp = canvas_action_Airport.GetElementAs<Button>(uiname).spriteSourceRectangle;
                UnitType tempunittype = UnitSpriteSheetRectangle.GetUnitType(temp);
                canvas_action_Airport.GetElementAs<Button>(uiname).spriteSourceRectangle = UnitSpriteSheetRectangle.GetSpriteRectangle(tempunittype, owner);
            }
            foreach (string uiname in canvas_action_Harbor.UInames)
            {
                Rectangle temp = canvas_action_Harbor.GetElementAs<Button>(uiname).spriteSourceRectangle;
                UnitType tempunittype = UnitSpriteSheetRectangle.GetUnitType(temp);
                canvas_action_Harbor.GetElementAs<Button>(uiname).spriteSourceRectangle = UnitSpriteSheetRectangle.GetSpriteRectangle(tempunittype, owner);
            }
        }

        #endregion

        private bool SelectBuilding()
        {
            MapCell temp = map[selectedMapCell];
            return (
                //check if the currently selected have 
                //a building that can produce unit
                isBuildingThatProduceUnit(temp.terrain)
                //check if there is no unit currently standing on said building
             && temp.unit == null
                //check if 
             && currentPlayer == localPlayer
             && temp.owner == playerInfos[localPlayer].owner);
        }

        private void ShowBuildingMenu()
        {
            MapCell temp = map[selectedBuilding];

            switch (temp.terrain)
            {
                case TerrainType.Factory:
                    canvas_action_Factory.IsVisible = true;
                    currentGameState = GameState.BuildingSelected;
                    break;
                case TerrainType.AirPort:
                    canvas_action_Airport.IsVisible = true;
                    currentGameState = GameState.BuildingSelected;
                    break;
                case TerrainType.Harbor:
                    canvas_action_Harbor.IsVisible = true;
                    currentGameState = GameState.BuildingSelected;
                    break;
                default:
                    break;
            }

            //legacy example to spawn an unit
            //if (selectedUnitToBuild != UnitType.None)
            //{
            //    SpawnUnit(selectedUnitToBuild, playerInfos[localPlayer], selectedBuilding);
            //    selectedUnitToBuild = UnitType.None;
            //    DeselectBuilding();
            //}
        }

        private void HideBuildingMenu()
        {
            canvas_action_Factory.IsVisible = false;
            canvas_action_Airport.IsVisible = false;
            canvas_action_Harbor.IsVisible = false;
        }

        private bool SpawnUnit(UnitType unittype,PlayerInfo owner,Point location)
        {
            MapCell spawnlocation = map[location];
            if (spawnlocation != null
              &&spawnlocation.unit == null)
            {
                Unit temp = UnitCreationHelper.Create(unittype, owner.owner);
                ownedUnit.Add(temp.guid);
                map.RegisterUnit(location, temp);
                return true;
            }
            return false;
        }

        private bool isBuilding(TerrainType t)
        {
            switch (t)
            {
                case TerrainType.MissileSilo:
                case TerrainType.MissileSiloLaunched:
                case TerrainType.City:
                case TerrainType.Factory:
                case TerrainType.AirPort:
                case TerrainType.Harbor:
                case TerrainType.Radar:
                case TerrainType.SupplyBase:
                case TerrainType.HQ:
                    return true;
                default:
                    return false;
            }
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
            MapCell tempmapcell = null;

            switch (currentGameState)
            {
                case GameState.None:
                case GameState.BuildingSelected:
                case GameState.BuildingBuildUnit:
                     tempmapcell = map[selectedMapCell];
                    break;

                case GameState.UnitSelected:
                case GameState.UnitMove:
                case GameState.UnitCommand:
                    tempmapcell = map[selectedUnit];
                    break;

                default:
                    break;
            }

            //update the generalinfo border
            canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoBorder").SourceRectangle = GeneralInfoBorderSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.owner);

            if (isBuilding(tempmapcell.terrain) && tempmapcell.terrain != TerrainType.MissileSiloLaunched && tempmapcell.terrain != TerrainType.MissileSilo && tempmapcell.unit!=null)
            {
                canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoCapturePoint").SourceRectangle = GeneralInfoCapturePointSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.owner);
                canvas_generalInfo.GetElementAs<Label>("label_generalInfoCapturePoint").Text = tempmapcell.unit.CapturePoint.ToString();
            }

            canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoDefenseStar").SourceRectangle = GeneralInfoDefenseStarSpriteSourceRectangle.GetSpriteRectangle(Unit._DefenseStar[tempmapcell.terrain]);

            if (tempmapcell.unit != null)
            {
                canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoUnitInfo").SourceRectangle = GeneralInfoUnitInfoSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.Owner);

                canvas_generalInfo.GetElementAs<PictureBox>("picbox_unitType").SourceRectangle = BackgroundUnitSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.UnitType, tempmapcell.unit.Owner, tempmapcell.terrain);
                canvas_generalInfo.GetElementAs<Label>("label_unitType").Text = tempmapcell.unit.GetUnitName();

                if (tempmapcell.unit.UnitType == UnitType.APC
                 || tempmapcell.unit.UnitType == UnitType.TransportCopter
                 || tempmapcell.unit.UnitType == UnitType.Lander
                 || tempmapcell.unit.UnitType == UnitType.Cruiser)
                {
                    canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoLoadedUnit").SourceRectangle = GeneralInfoLoadedUnitSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.Owner);
                    //update loaded unit here
                }

                int hp = tempmapcell.unit.HitPoint;
                canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoHPbar").SourceRectangle = new Rectangle(0, 0, hp*10, 3);
                canvas_generalInfo.GetElementAs<Label>("label_generalInfoHP").Text = hp.ToString();

                canvas_generalInfo.GetElementAs<Label>("label_generalInfoUnitInfo_fuel").Text = tempmapcell.unit.Fuel.ToString();
                canvas_generalInfo.GetElementAs<Label>("label_generalInfoUnitInfo_ammo").Text = tempmapcell.unit.Ammo.ToString();
            }
            else
            {
                canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoUnitInfo").SourceRectangle = Rectangle.Empty;

                canvas_generalInfo.GetElementAs<PictureBox>("picbox_unitType").SourceRectangle = Rectangle.Empty;
                canvas_generalInfo.GetElementAs<Label>("label_unitType").Text = " ";

                canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoLoadedUnit").SourceRectangle = Rectangle.Empty;

                canvas_generalInfo.GetElementAs<PictureBox>("picbox_generalInfoHPbar").SourceRectangle = Rectangle.Empty;
                canvas_generalInfo.GetElementAs<Label>("label_generalInfoHP").Text = " ";

                canvas_generalInfo.GetElementAs<Label>("label_generalInfoUnitInfo_fuel").Text = " ";
                canvas_generalInfo.GetElementAs<Label>("label_generalInfoUnitInfo_ammo").Text = " ";
            }

            //update the terrantype of the mapcell which is hovered on
            canvas_generalInfo.GetElementAs<PictureBox>("picbox_terrainType").SourceRectangle = BackgroundTerrainSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.terrain, map.weather, map.theme, tempmapcell.unit != null ? tempmapcell.unit.UnitType : UnitType.None, tempmapcell.owner);
            canvas_generalInfo.GetElementAs<Label>("label_terrainType").Text = tempmapcell.terrain.ToString();
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            foreach (Point p in map.mapcellthathaveunit)
            {
                map[p].unit.Animation.Update(gameTime);
            }
        }

        private void MoveCamera(KeyboardState keyboardInputState, MouseState mouseInputState)
        {
            int speed = 10;

            if (CONTENT_MANAGER.lastInputState.keyboardState.IsKeyDown(Keys.LeftShift) || CONTENT_MANAGER.lastInputState.keyboardState.IsKeyDown(Keys.RightShift))
            {
                speed = 50;
            }

            //simulate scrolling
            if (keyboardInputState.IsKeyDown(Keys.Left)
             || keyboardInputState.IsKeyDown(Keys.A)
             || mouseInputState.Position.X.Between(50, 10))
            {
                camera.Location += new Vector2(-1, 0) * speed;
            }
            if (keyboardInputState.IsKeyDown(Keys.Right)
                || keyboardInputState.IsKeyDown(Keys.D)
                || mouseInputState.Position.X.Between(710, 670))
            {
                camera.Location += new Vector2(1, 0) * speed;
            }
            if (keyboardInputState.IsKeyDown(Keys.Up)
                || keyboardInputState.IsKeyDown(Keys.W)
                || mouseInputState.Position.Y.Between(50, 10))
            {
                camera.Location += new Vector2(0, -1) * speed;
            }
            if (keyboardInputState.IsKeyDown(Keys.Down)
                || keyboardInputState.IsKeyDown(Keys.S)
                || mouseInputState.Position.Y.Between(470, 430))
            {
                camera.Location += new Vector2(0, 1) * speed;
            }

            Point clampMax = new Point(map.Width * Constants.MapCellWidth - 720, map.Height * Constants.MapCellHeight - 480);

            camera.Location = new Vector2(camera.Location.X.Clamp(clampMax.X, 0), camera.Location.Y.Clamp(clampMax.Y, 0));
        }

        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            DrawMap(CONTENT_MANAGER.spriteBatch, gameTime);
            
            //draw the guibackground
            //CONTENT_MANAGER.spriteBatch.Draw(guibackground, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
            canvas.Draw(CONTENT_MANAGER.spriteBatch);

            CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, currentGameState.ToString(), new Vector2(100, 100), Color.Red);
            if (movingAnim != null)
            {
                CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont,movingAnim.IsArrived.ToString(), new Vector2(100, 140), Color.Red);
            }

            //draw canvas_generalInfo
            //DrawCanvas_generalInfo();


            //draw the minimap
            //CONTENT_MANAGER.spriteBatch.Draw(minimap, minimapbound, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, LayerDepth.GuiBackground);
            //CONTENT_MANAGER.spriteBatch.Draw(CONTENT_MANAGER.generalInfo_HPbar, Vector2.Zero, new Rectangle(0, 0, 100, 3), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
        }

        private void DrawMap(SpriteBatch spriteBatch,GameTime gameTime)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);

            //render the map
            MapRenderer.Render(map, spriteBatch, gameTime);

            //draw moving animation
            if (currentGameState == GameState.UnitMove)
            {
                movingAnim.Draw(spriteBatch, gameTime);
            }

            //draw selected unit's movement range
            if (currentGameState == GameState.UnitSelected)
            {
                DrawMovementRange(spriteBatch);
            }

            //draw movementpath direction arrow if exist
            if (currentGameState == GameState.UnitSelected && movementPath != null)
            {
                dirarrowRenderer.UpdatePath(movementPath);
                dirarrowRenderer.Draw(spriteBatch);
            }

            //draw attackrange
            if (currentGameState == GameState.UnitCommand)
            {
                DrawAttackRange(spriteBatch);
            }

            //draw the cursor
            spriteBatch.Draw(cursor, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), null, Color.White, 0f, cursorOffset, 1f, SpriteEffects.None, LayerDepth.GuiUpper);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }

        private void DrawMovementRange(SpriteBatch spriteBatch)
        {
            if (movementRange != null)
            {
                foreach (Point dest in movementRange)
                {
                    spriteBatch.Draw(CONTENT_MANAGER.moveOverlay, new Vector2(dest.X * Constants.MapCellWidth, dest.Y * Constants.MapCellHeight), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
                }
            }
        }

        private void DrawAttackRange(SpriteBatch spriteBatch)
        {
            if (attackRange != null)
            {
                foreach (Point p in attackRange)
                {
                    spriteBatch.Draw(CONTENT_MANAGER.attackOverlay, new Vector2(p.X * Constants.MapCellWidth, p.Y * Constants.MapCellHeight), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
                }
            }
        }
        #endregion
    }
}
