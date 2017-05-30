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
        List<Unit> ownedUnit;
        #endregion

        //build unit information
        UnitType selectedUnitToBuild = UnitType.None;
        Point selectedBuilding = default(Point);

        //current unit selection
        Point selectedUnit = default(Point);
        Point lastSelectedUnit = default(Point);
        List<Point> movementRange = null;

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

        //command button list
        List<Button> cmdList;

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
            PictureBox commandslot = new PictureBox(CONTENT_MANAGER.commandspritesheet, Point.Zero, CommandSpriteSourceRectangle.GetSprite(playerInfos[localPlayer].owner == Owner.Red ? SpriteSheetCommandSlot.oneslotred : SpriteSheetCommandSlot.oneslotblue), null, depth: LayerDepth.GuiBackground);

            Button firstslot = new Button(CONTENT_MANAGER.commandspritesheet, CommandSpriteSourceRectangle.GetSprite(SpriteSheetCommand.Wait), new Point(6, 8));

            canvas_action_Unit.AddElement("commandslot", commandslot);
            canvas_action_Unit.AddElement("firstslot", firstslot);
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
                    selectedUnitToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
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
            Button button_cruiser = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Cruise, playerInfos[localPlayer].owner), new Point(570, 346), 0.5f);
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
                    selectedUnitToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
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
            MoveCamera(keyboardInputState, mouseInputState);
            selectedMapCell = Utility.HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, session.map.Width, session.map.Height);

            //update minimap
            if (!session.map.IsProcessed)
            {
                minimap = minimapgen.GenerateMapTexture(session.map);
            }

            //update game logic
            switch (currentGameState)
            {
                //the normal state of the game where nothing is selected
                case GameState.None:
                    if (HelperFunction.IsLeftMousePressed())
                    {
                        if (SelectUnit())
                        {
                            //get information of currently selected unit...
                            CONTENT_MANAGER.yes1.Play();
                            selectedUnit = selectedMapCell;
                            MapCell temp = session.map[selectedUnit];
                            canvas_generalInfo.GetElementAs<Label>("label_unittype").Text = temp.unit.UnitType.ToString() + Environment.NewLine + temp.unit.Owner.ToString();
                            CalculateMovementRange(temp.unit, selectedUnit);
                            isMovePathCalculated = true;

                            currentGameState = GameState.UnitSelected;
                            break;
                        }

                        if (currentGameState != GameState.UnitSelected)
                        {
                            //SelectBuilding();
                            break;
                        }
                    }
                    break;

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

                    //calculate movement path
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

                //previous state: UnitSelected
                //update and draw unit move animation
                //next state: UnitCommand  : end moving animation
                //            UnitSelected : if <Cancel> is pressed
                //            None         : if the unit is out of action point
                case GameState.UnitMove:
                    //check if <cancel> is pressed
                    if (HelperFunction.IsRightMousePressed())
                    {
                        //gobackto unitselected
                        RevertMovingUnitAnimation();
                        currentGameState = GameState.UnitSelected;
                        break;
                    }

                    //check if unit has arrived
                    if (movingAnim.IsArrived)
                    {
                        //teleport stuff
                        //teleport the unit to destination
                        session.map[destination].unit = session.map[selectedUnit].unit;
                        //dereference the unit from the origin 
                        session.map[selectedUnit].unit = null;

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
                            session.map[destination].unit.Animation.PlayAnimation(AnimationName.done.ToString());
                            session.map[destination].unit.Animation.ContinueAnimation();

                            //clear selectedUnit's information
                            //goto None
                            //currentGameState = GameState.None;
                        }
                        break;
                    }
                    else
                    {
                        //update moving animation
                        movingAnim.Update(gameTime);
                    }
                    break;

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
                        currentGameState = GameState.UnitSelected;
                        break;
                    }

                    //Command cmd = GetCommand();
                    //if (cmd != Command.None)
                    //{
                    //    ExecuteCommand(cmd);
                    //    //clear selectedUnit's information
                    //    currentGameState = GameState.None;
                    //}
                    break;

                //previous state: None
                //show building's canvas
                //next state: None              : if <Cancel> is pressed
                //            BuildingBuildUnit : if a Unit is selected to build
                case GameState.BuildingSelected:
                    break;

                //previous state: BuildingSelected:
                //spawn the unit which was selected to build
                //next state: None
                case GameState.BuildingBuildUnit:



                    //currentGameState = GameState.None;
                    break;
                default:
                    break;
            }

            UpdateAnimation(gameTime);
        }

        

        #region Update game logic
        private Command GetCommand()
        {
            return Command.None;
        }

        private void CalculateVision()
        {
            foreach (Unit unit in ownedUnit)
            {
                
            }
        }

        private int GetCommandCount()
        {
            //có wait nè
            //có attack nếu có Unit địch trong tầm tấn công và tầm nhìn nè
            //có load nếu đi vô transport unit nè
            //có drop nếu unit đang chở unit khác nè
            //có capture nếu là lính và đang đứng trên building khác màu nè
            //có supply nếu là apc và đang đứng cạnh 1 unit bạn nè

            return 1;
        }

        private void ShowCommandMenu()
        {
            canvas_action_Unit.IsVisible = true;
            int commandcount = GetCommandCount();
            Rectangle temp = CommandSpriteSourceRectangle.GetSprite(commandcount, playerInfos[localPlayer].owner);

            canvas_action_Unit.GetElementAs<PictureBox>("commandslot").SourceRectangle = temp;
            canvas_action_Unit.GetElementAs<PictureBox>("commandslot").Position = new Point(selectedUnit.X * Constants.MapCellWidth + 50, selectedUnit.Y * Constants.MapCellHeight);
            canvas_action_Unit.GetElementAs<Button>("firstslot").Position = new Point(selectedUnit.X * Constants.MapCellWidth + 50 + 6, selectedUnit.Y * Constants.MapCellHeight + 8);
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

            //substract fuel
            tempunit.Fuel -= movementPath.Count;

            //substract actionpoint
            tempunit.UpdateActionPoint(Command.Move);

            //play sfx
            CONTENT_MANAGER.moving_out.Play();

            //save the origin in case we need to return
            origin = selectedUnit;

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
            session.map[origin].unit = session.map[selectedUnit].unit;
            session.map[selectedUnit].unit = null;
            selectedUnit = origin;
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

        private bool SelectBuilding(int n)
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

        private void SelectBuilding()
        {
            MapCell temp = session.map[selectedMapCell];
            if (temp.unit == null
             && isBuildingThatProduceUnit(temp.terrain)
             && currentPlayer == localPlayer
             && temp.owner == playerInfos[localPlayer].owner
             && selectedBuilding != selectedMapCell)
            {
                DeselectBuilding();
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
                if (!actionbound.Contains(mouseInputState.Position) && currentGameState == GameState.BuildingSelected)
                {
                    DeselectBuilding();
                }
            }

            if (selectedUnitToBuild != UnitType.None)
            {
                SpawnUnit(selectedUnitToBuild, playerInfos[localPlayer], selectedBuilding);
                selectedUnitToBuild = UnitType.None;
                DeselectBuilding();
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
                DrawSelectedUnit(spriteBatch);
            }

            //draw movementpath direction arrow if exist
            if (currentGameState == GameState.UnitSelected && movementPath != null)
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
