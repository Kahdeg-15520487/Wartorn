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

        //resources
        Texture2D guibackground;
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
            minimap = minimapgen.GenerateMapTexture(session.map);
            playerInfos = sessiondata.playerInfos;
            ownedUnit = new List<Guid>();
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
            PictureBox commandslot = new PictureBox(CONTENT_MANAGER.commandspritesheet, Point.Zero, CommandSpriteSourceRectangle.GetSprite(playerInfos[localPlayer].owner == Owner.Red ? SpriteSheetCommandSlot.oneslotred : SpriteSheetCommandSlot.oneslotblue), null, depth: LayerDepth.GuiBackground);

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
                CONTENT_MANAGER.ShowMessageBox(selectedCmd.ToString());
            };
            secondslot.MouseClick += (sender, e) =>
            {
                selectedCmd = CommandSpriteSourceRectangle.GetCommand(secondslot.spriteSourceRectangle);
                CONTENT_MANAGER.ShowMessageBox(selectedCmd.ToString());
            };
            thirdslot.MouseClick += (sender, e) =>
            {
                selectedCmd = CommandSpriteSourceRectangle.GetCommand(thirdslot.spriteSourceRectangle);
                CONTENT_MANAGER.ShowMessageBox(selectedCmd.ToString());
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
            selectedMapCell = Utility.HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, session.map.Width, session.map.Height);

            //update minimap
            if (!session.map.IsProcessed)
            {
                minimap = minimapgen.GenerateMapTexture(session.map);
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
                            MapCell temp = session.map[selectedUnit];
                            canvas_generalInfo.GetElementAs<Label>("label_unittype").Text = temp.unit.UnitType.ToString() + Environment.NewLine + temp.unit.Owner.ToString();
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
                        if (movementRange.Contains(selectedMapCell) && selectedMapCell != lastSelectedMapCell)
                        {
                            //update movement path
                            movementPath = DijkstraHelper.FindPath(dijkstraGraph, selectedMapCell);
                            lastSelectedMapCell = selectedMapCell;
                        }
                    }

                    //check if a tile is selected is in the movementRange
                    if (HelperFunction.IsLeftMousePressed() && movementRange.Contains(selectedMapCell))
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
                    if (movingAnim.IsArrived && session.map[origin].unit!=null)
                    {
                        //teleport stuff
                        //teleport the unit to destination
                        session.map[destination].unit = session.map[origin].unit;
                        //dereference the unit from the origin 
                        session.map[origin].unit = null;

                        //save selectedUnit
                        lastSelectedUnit = selectedUnit;
                        //move selectedUnit to destination;
                        selectedUnit = destination;

                        //check if the unit's action point is above zero
                        //TODO make sure that the unit can only move once
                        if (session.map[destination].unit.ActionPoint > 0)
                        {
                            session.map[destination].unit.Animation.ContinueAnimation();
                            //show command menu
                            ShowCommandMenu();
                            //goto unitcommand
                            currentGameState = GameState.UnitCommand;
                        }
                        else
                        {
                            //all done for this unit, no more select, no more command, just lay there till next turn
                            session.map[destination].unit.Animation.PlayAnimation(AnimationName.done.ToString());
                            session.map[destination].unit.Animation.ContinueAnimation();

                            //clear selectedUnit's information


                            //goto None
                            currentGameState = GameState.None;
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
                        RevertMovingUnitAnimation();
                        currentGameState = GameState.UnitSelected;
                        break;
                    }

                    if (selectedCmd != Command.None)
                    {
                        //substract actionpoint
                        session.map[selectedUnit].unit.UpdateActionPoint(Command.Move);

                        //substract fuel
                        session.map[selectedUnit].unit.Fuel -= movementPath != null ? movementPath.Count : 0;

                        ExecuteCommand(selectedCmd);
                        selectedCmd = Command.None;

                        //clear selectedUnit's information

                        //hide command menu
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
            Unit tempunit = session.map[selectedUnit].unit;

            //substract action point
            tempunit.UpdateActionPoint(cmd);

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
                if (session.map[p].unit!=null 
                    //the below check is only for debug
                 && session.map[p].unit.Owner != session.map[selectedUnit].unit.Owner) 
                    //the below check is used in final game
               //&& !ownedUnit.Contains(session.map[p].unit.guid))
                {
                    temp += (int)Command.Attack;
                    break;
                }
            }

            //có load nếu đi vô transport unit nè

            //có drop nếu unit đang chở unit khác nè

            //có capture nếu là lính và đang đứng trên building khác màu nè
            if ((session.map[selectedUnit].unit.UnitType == UnitType.Soldier
             || session.map[selectedUnit].unit.UnitType == UnitType.Mech)
             && isBuilidng(session.map[selectedUnit].terrain)
             && session.map[selectedUnit].owner!= playerInfos[localPlayer].owner)
            {
                temp += (int)Command.Capture;
            }

            //có supply nếu là apc và đang đứng cạnh 1 unit bạn nè

            
            return temp;
        }

        private void ShowCommandMenu()
        {
            canvas_action_Unit.IsVisible = true;

            CalculateAttackRange(session.map[selectedUnit].unit, selectedUnit);
            int comds = GetCommands();

            var cmds = comds.GetContainCommand();

            string ttemp = string.Empty;
            foreach (Command cmd in cmds)
            {
                ttemp += cmd.ToString() + '\n';
            }
            CONTENT_MANAGER.ShowMessageBox(ttemp);

            Rectangle temp = CommandSpriteSourceRectangle.GetSprite(cmds.Count, playerInfos[localPlayer].owner);

            canvas_action_Unit.GetElementAs<PictureBox>("commandslot").SourceRectangle = temp;
            canvas_action_Unit.GetElementAs<PictureBox>("commandslot").Position = new Point(selectedUnit.X * Constants.MapCellWidth + 50, selectedUnit.Y * Constants.MapCellHeight);

            Button firstslot = canvas_action_Unit.GetElementAs<Button>("firstslot");
            Button secondslot = canvas_action_Unit.GetElementAs<Button>("secondslot");
            Button thirdslot = canvas_action_Unit.GetElementAs<Button>("thirdslot");

            firstslot.Position = new Point(selectedUnit.X * Constants.MapCellWidth + 50 + 6, selectedUnit.Y * Constants.MapCellHeight + 8);
            firstslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[0]);
            firstslot.rect = new Rectangle(firstslot.Position, firstslot.spriteSourceRectangle.Size);

            if (cmds.Count>1)
            {
                secondslot.Position = new Point(selectedUnit.X * Constants.MapCellWidth + 50 + 6, selectedUnit.Y * Constants.MapCellHeight + 16 + 8 + 8);
                secondslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[1]);
                secondslot.rect = new Rectangle(secondslot.Position, secondslot.spriteSourceRectangle.Size);
            }
            if (cmds.Count > 2)
            {
                thirdslot.Position = new Point(selectedUnit.X * Constants.MapCellWidth + 50 + 6, selectedUnit.Y * Constants.MapCellHeight + 32 + 8 + 8);
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
            MapCell temp = session.map[selectedMapCell];
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
            Unit tempunit = session.map[selectedUnit].unit;

            //play sfx
            CONTENT_MANAGER.moving_out.Play();

            //we gonna move unit by moving a clone of it then teleport it to the destination
            destination = selectedMapCell;

            //create a new animation object
            movingAnim = new MovingUnitAnimation(session.map[selectedUnit].unit, movementPath, new Point(selectedUnit.X * Constants.MapCellWidth, selectedUnit.Y * Constants.MapCellHeight));

            //ngung vẽ path
            isMovePathCalculated = false;

            //ngưng update animation cho unit gốc                        
            tempunit.Animation.StopAnimation();
        }

        private void RevertMovingUnitAnimation()
        {
            if (session.map[origin].unit == null)
            {
                session.map[origin].unit = session.map[selectedUnit].unit;
                session.map[selectedUnit].unit = null;
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
            dijkstraGraph = DijkstraHelper.CalculateGraph(session.map, unit, position);
            movementRange = DijkstraHelper.FindRange(dijkstraGraph);
        }

        private void CalculateAttackRange(Unit unit,Point position)
        {
            Range atkrange = unit.GetAttackkRange();
            attackRange = new List<Point>();

            int minx = (position.X - atkrange.Max);//.Clamp(position.X, 0);
            int maxx = (position.X + atkrange.Max);//.Clamp(session.map.Width, position.X);
            int miny = (position.Y - atkrange.Max);//.Clamp(position.Y, 0);
            int maxy = (position.Y + atkrange.Max);//.Clamp(session.map.Height, position.Y);

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
        }
        #endregion

        #region only use to demo gameplay these will not be used in game
        private void ChangeTurn()
        {
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

        private void ChangeUnitCanvasColor(Owner owner)
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
            MapCell temp = session.map[selectedMapCell];
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
            MapCell temp = session.map[selectedBuilding];

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
            MapCell spawnlocation = session.map[location];
            if (spawnlocation != null
              &&spawnlocation.unit == null)
            {
                Unit temp = UnitCreationHelper.Create(unittype, owner.owner);
                session.map[location].unit = temp;
                ownedUnit.Add(temp.guid);
                return true;
            }
            return false;
        }

        private bool isBuilidng(TerrainType t)
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
            if (movingAnim != null)
            {
                CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont,movingAnim.IsArrived.ToString(), new Vector2(100, 140), Color.Red);
            }

            //draw canvas_generalInfo
            //DrawCanvas_generalInfo();


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
            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), new Rectangle(0, 0, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);

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
