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

namespace Wartorn.Screens.MainGameScreen {
	enum GameState {
		None,
		TurnEnd,
		TurnStart,
		WaitForTurn,
		UnitSelected,
		UnitMove,
		UnitCommand,
		BuildingSelected,
		BuildingBuildUnit,
		CameraPan
	}

	class GameScreen : Screen {
		#region private field
		//information of this game session
		Session session;
		Map map;

		#region ui canvas
		Canvas canvas;
		Canvas canvas_SelectedMapCell;
		Canvas canvas_TargetedMapCell;
		Canvas canvas_action;
		Canvas canvas_action_Factory;
		Canvas canvas_action_Airport;
		Canvas canvas_action_Harbor;
		Canvas canvas_action_Unit;
		Canvas canvas_action_Building;
		#endregion

		//debug console
		//UIClass.Console console;

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
		bool isEnableEdgeScrolling = false;
		long timeout = 0;

		#region player information
		PlayerInfo[] playerInfos;
		int currentPlayer = 0;
		int otherPlayer = 1;
		int incomeThisTurn = 0;
		bool isGoingToEndTurn = false;
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
		GameState currentGameState = GameState.TurnStart;
		bool isDrawTargetRange = false;
		#endregion

		public GameScreen(GraphicsDevice device) : base(device, "GameScreen") {
			LoadContent();
			minimapgen = new MiniMapGenerator(device, CONTENT_MANAGER.spriteBatch);
		}

		#region Init
		public void InitSession(SessionData sessiondata) {
			session = new Session(sessiondata);
			map = session.map;
			minimap = minimapgen.GenerateMapTexture(map);
			playerInfos = sessiondata.playerInfos;

			mapcellVisibility = new bool[map.Width, map.Height];

			//init visibility table
			for (int x = 0; x < map.Width; x++) {
				for (int y = 0; y < map.Height; y++) {
					mapcellVisibility[x, y] = false;
				}
			}

			cursorOffset = selectCursorOffset;
		}

		private void LoadContent() {
			cursor = CONTENT_MANAGER.selectCursor;
		}

		public override bool Init() {
			//TODO: multiplayer : do something to handshake player

			camera = new Camera(_device.Viewport);
			canvas = new Canvas();

			InitUI();

			foreach (MapCell mapcell in map) {
				if (mapcell.unit != null) {
					for (int i = 0; i < playerInfos.GetLength(0); i++) {
						if (mapcell.unit.Owner == playerInfos[i].owner)
							playerInfos[i].ownedUnit.Add(mapcell.unit.guid);
					}
				}
			}
			return base.Init();
		}

		#region init ui
		private void InitUI() {
			PictureBox picturebox_tutorial = new PictureBox(CONTENT_MANAGER.gametutorial, Point.Zero, null, null) {
				IsVisible = CONTENT_MANAGER.IsTutorial
			};

			//declare ui elements
			canvas_SelectedMapCell = new Canvas();
			InitCanvas_SelectedMapCellInfo();

			canvas_TargetedMapCell = new Canvas();
			canvas_TargetedMapCell.IsVisible = false;
			InitCanvas_TargetedMapCellInfo();

			canvas_action = new Canvas();
			InitCanvas_action();

			canvas_action_Unit = new Canvas();
			canvas_action_Unit.IsVisible = false;
			InitCanvas_Unit();

			Label label_mousepos = new Label(" ", new Point(0, 0), new Vector2(80, 20), CONTENT_MANAGER.defaultfont);

			Button button_endTurn = new Button("End Turn", new Point(630, 10), new Vector2(100, 30), CONTENT_MANAGER.hackfont);
			button_endTurn.Origin = new Vector2(20, 0);

			button_endTurn.MouseClick += (sender, e) => {
				isGoingToEndTurn = true;

			};

			Label label_money = new Label(" ", new Point(550, 15), new Vector2(80, 40), CONTENT_MANAGER.hackfont);
			label_money.Origin = new Vector2(1, 1);
			label_money.backgroundColor = Color.White;

			Label label_whoseturn = new Label(" ", new Point(470, 15), new Vector2(50, 30), CONTENT_MANAGER.hackfont);
			label_whoseturn.Origin = new Vector2(1, 1);
			label_whoseturn.backgroundColor = Color.White;

			//console = new UIClass.Console(new Point(0, 0), new Vector2(720, 200), CONTENT_MANAGER.hackfont);
			//console.IsVisible = false;
			//console.SetVariable("player", currentPlayer);
			//console.SetVariable("changeTurn", new Action(this.ChangeTurn));

			//bind event

			//add to canvas
			canvas.AddElement("picturebox_tutorial", picturebox_tutorial);
			canvas.AddElement("SelectedMapCell", canvas_SelectedMapCell);
			canvas.AddElement("TargetedMapCell", canvas_TargetedMapCell);
			canvas.AddElement("action", canvas_action);
			canvas.AddElement("unit", canvas_action_Unit);
			//canvas.AddElement("label_mousepos", label_mousepos);
			//canvas.AddElement("console", console);
			canvas.AddElement("button_endTurn", button_endTurn);
			canvas.AddElement("label_money", label_money);
			canvas.AddElement("label_whoseturn", label_whoseturn);
		}

		private void InitCanvas_Unit() {
			//declare ui elements
			PictureBox commandslot = new PictureBox(CONTENT_MANAGER.commandspritesheet, Point.Zero, CommandSpriteSourceRectangle.GetSprite(playerInfos[currentPlayer].owner == GameData.Owner.Red ? SpriteSheetCommandSlot.oneslotred : SpriteSheetCommandSlot.oneslotblue), null, depth: LayerDepth.GuiBackground);

			Button firstslot = new Button(CONTENT_MANAGER.commandspritesheet, Rectangle.Empty, Point.Zero);
			Button secondslot = new Button(CONTENT_MANAGER.commandspritesheet, Rectangle.Empty, Point.Zero);
			Button thirdslot = new Button(CONTENT_MANAGER.commandspritesheet, Rectangle.Empty, Point.Zero);

			firstslot.ButtonColorPressed = Color.White;
			secondslot.ButtonColorPressed = Color.White;
			thirdslot.ButtonColorPressed = Color.White;

			//bind event
			firstslot.MouseClick += (sender, e) => {
				selectedCmd = CommandSpriteSourceRectangle.GetCommand(firstslot.spriteSourceRectangle);
				//CONTENT_MANAGER.ShowMessageBox(selectedCmd);
			};
			secondslot.MouseClick += (sender, e) => {
				selectedCmd = CommandSpriteSourceRectangle.GetCommand(secondslot.spriteSourceRectangle);
				//CONTENT_MANAGER.ShowMessageBox(selectedCmd);
			};
			thirdslot.MouseClick += (sender, e) => {
				selectedCmd = CommandSpriteSourceRectangle.GetCommand(thirdslot.spriteSourceRectangle);
				//CONTENT_MANAGER.ShowMessageBox(selectedCmd);
			};

			//add to canvas
			canvas_action_Unit.AddElement("commandslot", commandslot);
			canvas_action_Unit.AddElement("firstslot", firstslot);
			canvas_action_Unit.AddElement("secondslot", secondslot);
			canvas_action_Unit.AddElement("thirdslot", thirdslot);
		}

		private void InitCanvas_SelectedMapCellInfo() {
			//declare ui elements
			PictureBox picbox_SelectedMapCellBorder = new PictureBox(CONTENT_MANAGER.SelectedMapCell_border, new Point(175, 364), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiBackground);

			PictureBox picbox_terrainType = new PictureBox(CONTENT_MANAGER.background_terrain, new Point(183, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiBackground + 0.01f);
			Label label_terrainType = new Label(" ", new Point(186, 370), new Vector2(50, 20), CONTENT_MANAGER.hackfont);
			label_terrainType.Origin = new Vector2(-3, 2);
			label_terrainType.Scale = 0.75f;
			PictureBox picbox_unitType = new PictureBox(CONTENT_MANAGER.background_unit, new Point(183, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiBackground + 0.02f);
			Label label_unitType = new Label(" ", new Point(183, 464), new Vector2(80, 20), CONTENT_MANAGER.hackfont);
			label_unitType.Origin = new Vector2(-3, 2);
			label_unitType.Scale = 0.75f;

			PictureBox picbox_SelectedMapCellCapturePoint = new PictureBox(CONTENT_MANAGER.SelectedMapCell_capturePoint, new Point(262, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);
			Label label_SelectedMapCellCapturePoint = new Label(" ", new Point(262, 385), new Vector2(50, 20), CONTENT_MANAGER.hackfont);
			label_SelectedMapCellCapturePoint.Origin = new Vector2(-20, 0);

			PictureBox picbox_SelectedMapCellDefenseStar = new PictureBox(CONTENT_MANAGER.SelectedMapCell_defenseStar, new Point(265, 371), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);

			PictureBox picbox_SelectedMapCellUnitInfo = new PictureBox(CONTENT_MANAGER.SelectedMapCell_unitInfo, new Point(181, 453), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower + 0.01f);
			PictureBox picbox_SelectedMapCellLoadedUnit = new PictureBox(CONTENT_MANAGER.SelectedMapCell_loadedUnit, new Point(181, 435), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);

			PictureBox picbox_SelectedMapCellHPbar = new PictureBox(CONTENT_MANAGER.SelectedMapCell_HPbar, new Point(198, 458), null, Vector2.Zero, depth: LayerDepth.GuiUpper);

			Label label_SelectedMapCellHP = new Label(" ", new Point(301, 455), new Vector2(20, 20), CONTENT_MANAGER.hackfont);
			label_SelectedMapCellHP.Origin = new Vector2(1, 2);
			label_SelectedMapCellHP.Scale = 0.6f;
			Label label_SelectedMapCellUnitInfo_fuel = new Label(" ", new Point(263, 465), new Vector2(20, 20), CONTENT_MANAGER.hackfont);
			label_SelectedMapCellUnitInfo_fuel.Origin = new Vector2(-3, 2);
			label_SelectedMapCellUnitInfo_fuel.Scale = 0.75f;
			Label label_SelectedMapCellUnitInfo_ammo = new Label(" ", new Point(294, 465), new Vector2(20, 20), CONTENT_MANAGER.hackfont);
			label_SelectedMapCellUnitInfo_ammo.Origin = new Vector2(-3, 2);
			label_SelectedMapCellUnitInfo_ammo.Scale = 0.75f;

			//bind event

			//add to canvas
			canvas_SelectedMapCell.AddElement("picbox_SelectedMapCellBorder", picbox_SelectedMapCellBorder);

			canvas_SelectedMapCell.AddElement("picbox_SelectedMapCellCapturePoint", picbox_SelectedMapCellCapturePoint);
			canvas_SelectedMapCell.AddElement("label_SelectedMapCellCapturePoint", label_SelectedMapCellCapturePoint);

			canvas_SelectedMapCell.AddElement("picbox_SelectedMapCellDefenseStar", picbox_SelectedMapCellDefenseStar);

			canvas_SelectedMapCell.AddElement("picbox_SelectedMapCellUnitInfo", picbox_SelectedMapCellUnitInfo);
			canvas_SelectedMapCell.AddElement("picbox_SelectedMapCellLoadedUnit", picbox_SelectedMapCellLoadedUnit);

			canvas_SelectedMapCell.AddElement("picbox_SelectedMapCellHPbar", picbox_SelectedMapCellHPbar);

			canvas_SelectedMapCell.AddElement("label_SelectedMapCellHP", label_SelectedMapCellHP);
			canvas_SelectedMapCell.AddElement("label_SelectedMapCellUnitInfo_fuel", label_SelectedMapCellUnitInfo_fuel);
			canvas_SelectedMapCell.AddElement("label_SelectedMapCellUnitInfo_ammo", label_SelectedMapCellUnitInfo_ammo);


			canvas_SelectedMapCell.AddElement("picbox_terrainType", picbox_terrainType);
			canvas_SelectedMapCell.AddElement("label_terrainType", label_terrainType);

			canvas_SelectedMapCell.AddElement("picbox_unitType", picbox_unitType);
			canvas_SelectedMapCell.AddElement("label_unitType", label_unitType);
		}

		private void InitCanvas_TargetedMapCellInfo() {
			//declare ui elements
			PictureBox picbox_TargetedMapCellBorder = new PictureBox(CONTENT_MANAGER.SelectedMapCell_border, new Point(319, 364), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiBackground);

			PictureBox picbox_terrainType = new PictureBox(CONTENT_MANAGER.background_terrain, new Point(327, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiBackground + 0.01f);
			Label label_terrainType = new Label(" ", new Point(330, 370), new Vector2(50, 20), CONTENT_MANAGER.hackfont);
			label_terrainType.Origin = new Vector2(-3, 2);
			label_terrainType.Scale = 0.75f;
			PictureBox picbox_unitType = new PictureBox(CONTENT_MANAGER.background_unit, new Point(327, 385), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiBackground + 0.02f);
			Label label_unitType = new Label(" ", new Point(330, 464), new Vector2(80, 20), CONTENT_MANAGER.hackfont);
			label_unitType.Origin = new Vector2(-3, 2);
			label_unitType.Scale = 0.75f;

			PictureBox picbox_TargetedMapCellDefenseStar = new PictureBox(CONTENT_MANAGER.SelectedMapCell_defenseStar, new Point(409, 371), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower);

			PictureBox picbox_TargetedMapCellUnitInfo = new PictureBox(CONTENT_MANAGER.SelectedMapCell_unitInfo, new Point(325, 453), Rectangle.Empty, Vector2.Zero, depth: LayerDepth.GuiLower + 0.01f);

			PictureBox picbox_TargetedMapCellHPbar = new PictureBox(CONTENT_MANAGER.SelectedMapCell_HPbar, new Point(342, 458), null, Vector2.Zero, depth: LayerDepth.GuiUpper);

			Label label_TargetedMapCellHP = new Label(" ", new Point(445, 455), new Vector2(20, 20), CONTENT_MANAGER.hackfont);
			label_TargetedMapCellHP.Origin = new Vector2(1, 2);
			label_TargetedMapCellHP.Scale = 0.6f;

			//bind event

			//add to canvas
			canvas_TargetedMapCell.AddElement("picbox_TargetedMapCellBorder", picbox_TargetedMapCellBorder);
			canvas_TargetedMapCell.AddElement("picbox_TargetedMapCellDefenseStar", picbox_TargetedMapCellDefenseStar);
			canvas_TargetedMapCell.AddElement("picbox_TargetedMapCellUnitInfo", picbox_TargetedMapCellUnitInfo);
			canvas_TargetedMapCell.AddElement("picbox_TargetedMapCellHPbar", picbox_TargetedMapCellHPbar);
			canvas_TargetedMapCell.AddElement("label_TargetedMapCellHP", label_TargetedMapCellHP);

			canvas_TargetedMapCell.AddElement("picbox_terrainType", picbox_terrainType);
			canvas_TargetedMapCell.AddElement("label_terrainType", label_terrainType);

			canvas_TargetedMapCell.AddElement("picbox_unitType", picbox_unitType);
			canvas_TargetedMapCell.AddElement("label_unitType", label_unitType);
		}

		#region buy menu
		/*  action button layout
         *  574,300
         *  583,309
         *  583,343
         *  
         *      584 618 652 686
         *  378
         *  412
         *  446
         *  
         */
		private void InitCanvas_action() {
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

		private void InitCanvas_Factory() {
			canvas_action_Factory = new Canvas();
			canvas_action_Factory.IsVisible = false;

			PictureBox picturebox_buymenu = new PictureBox(CONTENT_MANAGER.buymenu_factory, new Point(574, 300), BuyMenuFactorySpriteSourceRectangle.GetSpriteRectangle(playerInfos[currentPlayer].owner), Vector2.Zero, depth: LayerDepth.GuiBackground);

			Label label_unitname = new Label(" ", new Point(583, 309), new Vector2(100, 30), CONTENT_MANAGER.hackfont);
			label_unitname.Origin = new Vector2(1, 1);
			Label label_unitcost = new Label(" ", new Point(583, 343), new Vector2(100, 30), CONTENT_MANAGER.hackfont);
			label_unitcost.Origin = new Vector2(1, 1);

			//hàng 1
			Button button_Soldier = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Soldier, playerInfos[currentPlayer].owner), new Point(584, 378), 0.5f);
			Button button_Mech = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Mech, playerInfos[currentPlayer].owner), new Point(618, 378), 0.5f);
			Button button_Recon = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Recon, playerInfos[currentPlayer].owner), new Point(652, 378), 0.5f);
			Button button_APC = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.APC, playerInfos[currentPlayer].owner), new Point(686, 378), 0.5f);

			//hàng 2
			Button button_Tank = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Tank, playerInfos[currentPlayer].owner), new Point(584, 412), 0.5f);
			Button button_H_Tank = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.HeavyTank, playerInfos[currentPlayer].owner), new Point(618, 412), 0.5f);
			Button button_Artillery = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Artillery, playerInfos[currentPlayer].owner), new Point(652, 412), 0.5f);
			Button button_Rocket = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Rocket, playerInfos[currentPlayer].owner), new Point(686, 412), 0.5f);

			//hàng 3
			Button button_AntiAir = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.AntiAir, playerInfos[currentPlayer].owner), new Point(584, 446), 0.5f);
			Button button_Missile = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Missile, playerInfos[currentPlayer].owner), new Point(618, 446), 0.5f);

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
			foreach (Button button in tempbuttonlist) {
				button.isDrawRect = true;
				button.ButtonColorPressed = Color.White;
				button.MouseClick += (sender, e) => {
					selectedUnitTypeToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
				};
				button.MouseEnter += (sender, e) => {
					var temp = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
					label_unitname.Text = temp.GetName();
					label_unitcost.Text = Unit._UnitStat[temp].Cost.ToString();
				};
				button.MouseLeave += (sender, e) => {
					label_unitname.Text = " ";
					label_unitcost.Text = " ";
				};
			}
			#endregion

			canvas_action_Factory.AddElement("picturebox_buymenu", picturebox_buymenu);
			canvas_action_Factory.AddElement("label_unitname", label_unitname);
			canvas_action_Factory.AddElement("label_unitcost", label_unitcost);
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

		private void InitCanvas_Airport() {
			canvas_action_Airport = new Canvas();
			canvas_action_Airport.IsVisible = false;

			PictureBox picturebox_buymenu = new PictureBox(CONTENT_MANAGER.buymenu_airport_harbor, new Point(574, 300), BuyMenuAirportHarborSpriteSourceRectangle.GetSpriteRectangle(playerInfos[currentPlayer].owner), Vector2.Zero, depth: LayerDepth.GuiBackground);

			Label label_unitname = new Label(" ", new Point(583, 309), new Vector2(100, 30), CONTENT_MANAGER.hackfont);
			label_unitname.Origin = new Vector2(1, 1);
			Label label_unitcost = new Label(" ", new Point(583, 343), new Vector2(100, 30), CONTENT_MANAGER.hackfont);
			label_unitcost.Origin = new Vector2(1, 1);

			//hàng 1
			Button button_transportcopter = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.TransportCopter, playerInfos[currentPlayer].owner), new Point(584, 378), 0.5f);
			Button button_battlecopter = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.BattleCopter, playerInfos[currentPlayer].owner), new Point(618, 378), 0.5f);
			Button button_fighter = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Fighter, playerInfos[currentPlayer].owner), new Point(652, 378), 0.5f);
			Button button_bomber = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Bomber, playerInfos[currentPlayer].owner), new Point(686, 378), 0.5f);

			List<Button> tempbuttonlist = new List<Button>();
			tempbuttonlist.Add(button_transportcopter);
			tempbuttonlist.Add(button_battlecopter);
			tempbuttonlist.Add(button_fighter);
			tempbuttonlist.Add(button_bomber);


			#region bind event
			foreach (Button button in tempbuttonlist) {
				button.ButtonColorPressed = Color.White;
				button.MouseClick += (sender, e) => {
					selectedUnitTypeToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
				};
				button.MouseEnter += (sender, e) => {
					var temp = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
					label_unitname.Text = temp.GetName();
					label_unitcost.Text = Unit._UnitStat[temp].Cost.ToString();
				};
				button.MouseLeave += (sender, e) => {
					label_unitname.Text = " ";
					label_unitcost.Text = " ";
				};
			}
			#endregion

			canvas_action_Airport.AddElement("picturebox_buymenu", picturebox_buymenu);
			canvas_action_Airport.AddElement("label_unitname", label_unitname);
			canvas_action_Airport.AddElement("label_unitcost", label_unitcost);
			canvas_action_Airport.AddElement("button_transportcopter", button_transportcopter);
			canvas_action_Airport.AddElement("button_battlecopter", button_battlecopter);
			canvas_action_Airport.AddElement("button_fighter", button_fighter);
			canvas_action_Airport.AddElement("button_bomber", button_bomber);
		}

		private void InitCanvas_Harbor() {
			canvas_action_Harbor = new Canvas();
			canvas_action_Harbor.IsVisible = false;

			PictureBox picturebox_buymenu = new PictureBox(CONTENT_MANAGER.buymenu_airport_harbor, new Point(574, 300), BuyMenuAirportHarborSpriteSourceRectangle.GetSpriteRectangle(playerInfos[currentPlayer].owner), Vector2.Zero, depth: LayerDepth.GuiBackground);

			Label label_unitname = new Label(" ", new Point(583, 309), new Vector2(100, 30), CONTENT_MANAGER.hackfont);
			label_unitname.Origin = new Vector2(1, 1);
			Label label_unitcost = new Label(" ", new Point(583, 343), new Vector2(100, 30), CONTENT_MANAGER.hackfont);
			label_unitcost.Origin = new Vector2(1, 1);

			//hàng 1
			Button button_lander = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Lander, playerInfos[currentPlayer].owner), new Point(584, 378), 0.5f);
			Button button_cruiser = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Cruiser, playerInfos[currentPlayer].owner), new Point(618, 378), 0.5f);
			Button button_submarine = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Submarine, playerInfos[currentPlayer].owner), new Point(652, 378), 0.5f);
			Button button_battleship = new Button(CONTENT_MANAGER.unitSpriteSheet, UnitSpriteSheetRectangle.GetSpriteRectangle(UnitType.Battleship, playerInfos[currentPlayer].owner), new Point(686, 378), 0.5f);

			List<Button> tempbuttonlist = new List<Button>();
			tempbuttonlist.Add(button_lander);
			tempbuttonlist.Add(button_cruiser);
			tempbuttonlist.Add(button_submarine);
			tempbuttonlist.Add(button_battleship);


			#region bind event
			foreach (Button button in tempbuttonlist) {
				button.ButtonColorPressed = Color.White;
				button.MouseClick += (sender, e) => {
					selectedUnitTypeToBuild = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
				};
				button.MouseEnter += (sender, e) => {
					var temp = UnitSpriteSheetRectangle.GetUnitType(button.spriteSourceRectangle);
					label_unitname.Text = temp.GetName();
					label_unitcost.Text = Unit._UnitStat[temp].Cost.ToString();
				};
				button.MouseLeave += (sender, e) => {
					label_unitname.Text = " ";
					label_unitcost.Text = " ";
				};
			}
			#endregion

			canvas_action_Harbor.AddElement("picturebox_buymenu", picturebox_buymenu);
			canvas_action_Harbor.AddElement("label_unitname", label_unitname);
			canvas_action_Harbor.AddElement("label_unitcost", label_unitcost);
			canvas_action_Harbor.AddElement("button_lander", button_lander);
			canvas_action_Harbor.AddElement("button_cruiser", button_cruiser);
			canvas_action_Harbor.AddElement("button_submarine", button_submarine);
			canvas_action_Harbor.AddElement("button_battleship", button_battleship);
		}
		#endregion

		#endregion

		#endregion

		public override void Shutdown() {
			map = null;
			minimap?.Dispose();
			minimap = null;
		}

		public override void Update(GameTime gameTime) {
			mouseInputState = CONTENT_MANAGER.inputState.mouseState;
			lastMouseInputState = CONTENT_MANAGER.lastInputState.mouseState;
			keyboardInputState = CONTENT_MANAGER.inputState.keyboardState;
			lastKeyboardInputState = CONTENT_MANAGER.lastInputState.keyboardState;

			//if (HelperFunction.IsKeyPress(Keys.OemTilde))
			//{
			//    console.IsVisible = !console.IsVisible;
			//}

			if (HelperFunction.IsKeyPress(Keys.F1)) {
				canvas.GetElementAs<PictureBox>("picturebox_tutorial").IsVisible = !canvas.GetElementAs<PictureBox>("picturebox_tutorial").IsVisible;
			}

			//update canvas
			canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
			//((Label)canvas["label_mousepos"]).Text = mouseInputState.Position.ToString();
			canvas.GetElementAs<Label>("label_money").Text = " " + playerInfos[currentPlayer].money + Environment.NewLine + "+" + incomeThisTurn;
			UpdateCanvas_SelectedMapCell();

			if (canvas_TargetedMapCell.IsVisible) {
				UpdateCanvas_TargetedMapCell();
			}

			//camera control
			if (currentGameState == GameState.None || currentGameState == GameState.UnitSelected)// || currentGameState == GameState.BuildingSelected )
			{
				MoveCamera(keyboardInputState, mouseInputState);
			}
			selectedMapCell = HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, map.Width, map.Height);

			//update minimap
			if (!map.IsProcessed) {
				minimap = minimapgen.GenerateMapTexture(map);
			}

			//update game logic
			switch (currentGameState) {
				#region GameState.TurnStart
				//previous state: TurnEnd
				//bắt đầu lượt chơi
				//next state: None
				case GameState.TurnStart:
					var label_whoseturn = canvas.GetElementAs<Label>("label_whoseturn");
					label_whoseturn.Text = playerInfos[currentPlayer].owner.ToString();
					switch (playerInfos[currentPlayer].owner) {
						case Owner.Red:
							label_whoseturn.foregroundColor = Color.IndianRed;
							break;
						case Owner.Blue:
							label_whoseturn.foregroundColor = Color.CadetBlue;
							break;
						case Owner.Green:
							break;
						case Owner.Yellow:
							break;
					}

					#region trừ fuel cho các unit air và naval
					foreach (var guid in playerInfos[currentPlayer].ownedUnit) {
						int lostfuel = 0;
						var tempoint = map.FindUnit(guid);
						if (tempoint.X == -1) {
							continue;
						}
						var tempu = map[tempoint].unit;
						if (tempu.UnitType.IsAirUnit()) {
							switch (tempu.UnitType) {
								case UnitType.BattleCopter:
									lostfuel = 2;
									break;
								case UnitType.TransportCopter:
									lostfuel = 2;
									break;
								case UnitType.Fighter:
									lostfuel = 4;
									break;
								case UnitType.Bomber:
									lostfuel = 5;
									break;
								default:
									break;
							}
							tempu.Gas -= lostfuel;
						}
						else {
							if (tempu.UnitType.IsNavalUnit()) {
								switch (tempu.UnitType) {
									case UnitType.Lander:
										lostfuel = 1;
										break;
									case UnitType.Cruiser:
										lostfuel = 1;
										break;
									case UnitType.Submarine:
										if (tempu.isDiving)
											lostfuel = 5;
										else
											lostfuel = 1;
										break;
									case UnitType.Battleship:
										lostfuel = 1;
										break;
									default:
										break;
								}
								tempu.Gas -= lostfuel;
							}
						}
						if (tempu.Gas <= 0) {
							playerInfos[currentPlayer].ownedUnit.Remove(tempu.guid);
							map.RemoveUnit(tempoint);
						}
					}
					#endregion

					//get bulding thuộc về current player
					var buildings = map.GetOwnedBuilding(playerInfos[currentPlayer].owner).ToList();

					//update tiền
					incomeThisTurn = (buildings.Count - 1) * 1000 + 3000;
					playerInfos[currentPlayer].money += incomeThisTurn;

					#region supply và repair cho các unit đang ở trong factory, airport, harbor, supplybase
					int supplyandrepaircost = 0;
					foreach (var p in buildings) {
						MapCell tempmapcell = map[p];
						if (tempmapcell.unit != null) {
							var tempu = tempmapcell.unit;
							switch (tempmapcell.terrain) {
								case TerrainType.Factory:
									if (tempu.UnitType.IsLandUnit()) {
										if (tempu.Gas < Unit._UnitStat[tempu.UnitType].Gas
										|| tempu.Ammo < Unit._UnitStat[tempu.UnitType].Ammo) {
											tempu.Resupply();
											supplyandrepaircost += 300;
										}
										if (tempu.HitPoint < 10) {
											tempu.Repair();
											supplyandrepaircost += Unit._UnitStat[tempu.UnitType].Cost / 10;
										}
									}
									break;
								case TerrainType.AirPort:
									if (tempu.UnitType.IsAirUnit()) {
										if (tempu.Gas < Unit._UnitStat[tempu.UnitType].Gas
										|| tempu.Ammo < Unit._UnitStat[tempu.UnitType].Ammo) {
											tempu.Resupply();
											supplyandrepaircost += 300;
										}
										if (tempu.HitPoint < 10) {
											tempu.Repair();
											supplyandrepaircost += Unit._UnitStat[tempu.UnitType].Cost / 10;
										}
									}
									break;
								case TerrainType.Harbor:
									if (tempu.UnitType.IsNavalUnit()) {
										if (tempu.Gas < Unit._UnitStat[tempu.UnitType].Gas
										|| tempu.Ammo < Unit._UnitStat[tempu.UnitType].Ammo) {
											tempu.Resupply();
											supplyandrepaircost += 300;
										}
										if (tempu.HitPoint < 10) {
											tempu.Repair();
											supplyandrepaircost += Unit._UnitStat[tempu.UnitType].Cost / 10;
										}
									}
									break;
								case TerrainType.SupplyBase:
									tempu.Resupply();
									break;
								default:
									break;
							}
						}
					}
					playerInfos[currentPlayer].money -= supplyandrepaircost;
					#endregion

					//reset actionpoint cho tất cả các unit phe mình
					foreach (var guid in playerInfos[currentPlayer].ownedUnit) {
						var temp = map.FindUnit(guid);
						if (temp.X == -1) {
							continue;
						}
						map[temp].unit.UpdateActionPoint(Command.None);
					}
					//set animation cho tất cả các unit thành idle
					foreach (var p in map.mapcellthathaveunit) {
						map[p].unit.Animation.PlayAnimation(AnimationName.idle.ToString());
					}

					//todo update super weapon

					//goto None
					currentGameState = GameState.None;
					break;
				#endregion

				#region GameState.TurnEnd
				//previous state: None
				//kết thúc lượt
				//next state: TurnStart
				case GameState.TurnEnd:
					//kiểm tra thắng thua
					if (map[playerInfos[currentPlayer].HQlocation].owner != playerInfos[currentPlayer].owner
					 || map[playerInfos[otherPlayer].HQlocation].owner != playerInfos[otherPlayer].owner) {
						//thua cmnr
						//nhảy qua scene end game
						var sessiondata = new SessionData();
						sessiondata.map = map;
						sessiondata.playerInfos = playerInfos;
						((EndGameScreen)SCREEN_MANAGER.get_screen("EndGameScreen")).InitEndGameScreen(sessiondata);
						SCREEN_MANAGER.goto_screen("EndGameScreen");
					}

					//kết thúc lượt và đổi lượt
					ChangeTurn();

					//goto TurnStart
					currentGameState = GameState.TurnStart;
					break;
				#endregion

				#region GameState.None
				//the normal state of the game where nothing is selected
				//next state: UnitSelected
				//            BuildingSelected
				//            TurnEnd
				case GameState.None:
					if (isGoingToEndTurn) {
						isGoingToEndTurn = false;
						currentGameState = GameState.TurnEnd;
						break;
					}

					if (HelperFunction.IsLeftMousePressed()) {
						if (SelectUnit()) {
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

						if (SelectBuilding()) {
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
					if (HelperFunction.IsRightMousePressed()) {
						//clear selectedUnit's information
						currentGameState = GameState.None;
						break;
					}

					//check if movement path calculated
					if (!isMovePathCalculated) {
						//calculate move path
						if (movementRange.Contains(selectedMapCell) && selectedMapCell != lastSelectedMapCell && map[selectedMapCell].unit == null) {
							//update movement path
							movementPath = DijkstraHelper.FindPath(dijkstraGraph, selectedMapCell);
							lastSelectedMapCell = selectedMapCell;
							//isMovePathCalculated = true;
						}
					}

					//check if a tile is selected is in the movementRange
					if (HelperFunction.IsLeftMousePressed() && movementPath != null && movementRange != null && movementRange.Contains(selectedMapCell) && map[selectedMapCell].unit == null) {
						StartMovingUnitAnimation();
						currentGameState = GameState.UnitMove;
						break;
					}

					//check if the currently selected unit is selected again
					if (HelperFunction.IsLeftMousePressed() && selectedMapCell == selectedUnit) {
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
					if (HelperFunction.IsRightMousePressed() && movingAnim.IsArrived) {
						//gobackto unitselected
						RevertMovingUnitAnimation();
						currentGameState = GameState.UnitSelected;
						break;
					}

					//check if unit has arrived and not already teleported
					if (movingAnim.IsArrived && map[origin].unit != null) {
						//teleport stuff
						map.TeleportUnit(origin, destination);

						//save selectedUnit
						lastSelectedUnit = selectedUnit;
						//move selectedUnit to destination;
						selectedUnit = destination;

						//check if the unit's action point is above zero
						//TODO make sure that the unit can only move once
						//if (map[selectedUnit].unit.PeakUpdateActionPoint(Command.Move) > 0)
						//{
						map[selectedUnit].unit.Animation.ContinueAnimation();
						//show command menu
						ShowCommandMenu();
						//goto unitcommand
						currentGameState = GameState.UnitCommand;
						//}
						//else
						//{
						//    //Execute Command.Wait for this unit
						//    selectedCmd = Command.Wait;

						//    //goto Command
						//    currentGameState = GameState.UnitCommand;
						//}
						break;
					}
					else {
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
					if (HelperFunction.IsRightMousePressed()) {
						//change cursor back
						cursor = CONTENT_MANAGER.selectCursor;
						cursorOffset = selectCursorOffset;

						RevertMovingUnitAnimation();

						canvas_action_Unit.IsVisible = false;

						currentGameState = GameState.UnitSelected;
						selectedCmd = Command.None;

						isDrawTargetRange = false;
						break;
					}


					Unit tempunit = map[selectedUnit].unit;
					Point selectedTarget;
					Func<Point, bool> filter;

					switch (selectedCmd) {
						case Command.Wait:
							tempunit.UpdateActionPoint(Command.Wait);
							goto finalise_command_execution;

						#region Command.Attack
						case Command.Attack:
							//show target canvas
							canvas_TargetedMapCell.IsVisible = true;

							//change cursor to attack cursor
							cursor = CONTENT_MANAGER.attackCursor;
							cursorOffset = attackCursorOffset;

							isDrawTargetRange = true;

							if (HelperFunction.IsLeftMousePressed()
							 && attackRange.Contains(selectedMapCell)
							 && map[selectedMapCell].unit != null
							 && map[selectedMapCell].unit.Owner != playerInfos[currentPlayer].owner) {
								tempunit.UpdateActionPoint(Command.Attack);

								//do attack stuff
								selectedTarget = selectedMapCell;
								Unit otherunit = map[selectedTarget].unit;
								var result = Unit.GetCalculatedDamage(map[selectedUnit], map[selectedTarget]);
								//CONTENT_MANAGER.ShowMessageBox(string.Format("{0} attack {1}: {2}", tempunit.UnitType, otherunit.UnitType, result));
								tempunit.HitPoint = result.attackerHP;
								otherunit.HitPoint = result.defenderHP;

								if (tempunit.HitPoint <= 0) {
									playerInfos[currentPlayer].ownedUnit.Remove(map[selectedUnit].unit.guid);
									map.RemoveUnit(selectedUnit);
								}
								if (otherunit.HitPoint <= 0) {
									playerInfos[otherPlayer].ownedUnit.Remove(map[selectedTarget].unit.guid);
									map.RemoveUnit(selectedTarget);
								}

								//hide target canvas
								canvas_TargetedMapCell.IsVisible = false;

								//end command
								goto finalise_command_execution;
							}
							break;
						#endregion

						case Command.Capture:

							if (map[selectedUnit].terrain.IsBuildingThatIsCapturable()
							 && map[selectedUnit].owner != playerInfos[currentPlayer].owner
							 && tempunit.CapturePoint == 0) {
								map[selectedUnit].unit.CapturePoint = 20;
							}

							tempunit.CapturePoint -= tempunit.HitPoint;
							tempunit.UpdateActionPoint(Command.Capture);

							if (tempunit.CapturePoint <= 0) {
								map[selectedUnit].owner = playerInfos[currentPlayer].owner;
								map.IsProcessed = false;
								tempunit.CapturePoint = 0;
							}
							goto finalise_command_execution;

						case Command.Load:
							//show load range like attack range
							//select unit to load in
							//if not select, return to previous command
							//if selected a unit, move unit from mapcell into the carrier
							//end command

							isDrawTargetRange = false;

							if (tempunit.UnitType == UnitType.APC || tempunit.UnitType == UnitType.TransportCopter) {
								filter = (Point p) => (map[p].unit != null && map[p].unit.UnitType.IsInfantryUnit());
							}
							else {
								filter = (Point p) => (map[p].unit != null && map[p].unit.UnitType.IsLandUnit());
							}

							//filter unit that the carrier can take
							attackRange = selectedUnit.GetNearbyPoints(Direction.North, Direction.East, Direction.South, Direction.West).Where(filter).ToList();

							isDrawTargetRange = true;

							if (HelperFunction.IsLeftMousePressed()
							 && attackRange.Contains(selectedMapCell)
							 && map[selectedMapCell].unit != null
							 && map[selectedMapCell].unit.Owner == playerInfos[currentPlayer].owner) {
								tempunit.UpdateActionPoint(Command.Load);

								//do load unit stuff
								selectedTarget = selectedMapCell;

								//reset action point for the unit
								map[selectedTarget].unit.UpdateActionPoint(Command.None);

								//move the unit into the carrier
								tempunit.carryingUnit = map[selectedTarget].unit;

								//remove the unit from the map
								map.RemoveUnit(selectedTarget);

								//end command
								goto finalise_command_execution;
							}

							break;

						case Command.Drop: {
								//show load range like attack range
								//load range include free mapcell north,south,west,east to the carrier
								//select mapcell to drop out
								//if not select, return to previous command
								//if selected a unit, move unit from the carrier into the selected mapcell
								//end command

								isDrawTargetRange = false;

								//create filter
								//free mapcell north,south,west,east to the carrier
								//the carried unit can stand on
								if (tempunit.UnitType == UnitType.APC || tempunit.UnitType == UnitType.TransportCopter) {
									filter = (Point p) => (map[p].unit == null && tempunit.UnitType.GetMovementType().IsTraversable(map[p].terrain));
								}
								else {
									filter = (Point p) => (map[p].terrain == TerrainType.Coast || map[p].terrain == TerrainType.Harbor);
								}

								//filter unit that the carrier can take
								attackRange = selectedUnit.GetNearbyPoints(Direction.North, Direction.East, Direction.South, Direction.West).Where(filter).ToList();

								isDrawTargetRange = true;

								if (HelperFunction.IsLeftMousePressed()
								 && attackRange.Contains(selectedMapCell)
								 && map[selectedMapCell].unit == null) {
									tempunit.UpdateActionPoint(Command.Drop);

									//do drop unit stuff
									selectedTarget = selectedMapCell;

									//add the unit into the map
									map.RegisterUnit(selectedTarget, tempunit.carryingUnit);

									//remove the unit from the carrier
									tempunit.carryingUnit = null;

									//end command
									goto finalise_command_execution;
								}
							}
							break;

						case Command.Rise:
							break;

						case Command.Dive:
							break;

						case Command.Supply:
							tempunit.UpdateActionPoint(Command.Supply);
							Point north = selectedUnit.GetNearbyPoint(Direction.North);
							Point south = selectedUnit.GetNearbyPoint(Direction.South);
							Point east = selectedUnit.GetNearbyPoint(Direction.East);
							Point west = selectedUnit.GetNearbyPoint(Direction.West);
							bool isSupplied = false;
							try { map[north].unit.Resupply(); isSupplied = true; } catch (Exception) { }
							try { map[south].unit.Resupply(); isSupplied = true; } catch (Exception) { }
							try { map[east].unit.Resupply(); isSupplied = true; } catch (Exception) { }
							try { map[west].unit.Resupply(); isSupplied = true; } catch (Exception) { }
							if (isSupplied == true) {
								tempunit.Ammo = (tempunit.Ammo - 1).Clamp(10, 0);
							}
							//end command
							goto finalise_command_execution;

						case Command.Move:
							//substract actionpoint
							map[selectedUnit].unit.UpdateActionPoint(Command.Move);

							//substract fuel
							map[selectedUnit].unit.Gas -= movementPath != null ? movementPath.Count : 0;
							break;

						case Command.Operate:
							//show target canvas
							canvas_TargetedMapCell.IsVisible = true;

							//change cursor to attack cursor
							cursor = CONTENT_MANAGER.attackCursor;
							cursorOffset = attackCursorOffset;

							//cause the missilesilo have a range of the whole map
							attackRange = new List<Point>();
							//only target the mapcellthathaveunit
							foreach (var guid in playerInfos[otherPlayer].ownedUnit) {
								attackRange.Add(map.FindUnit(guid));
							}
							isDrawTargetRange = true;
							if (HelperFunction.IsLeftMousePressed()
							 && attackRange.Contains(selectedMapCell)
							 && map[selectedMapCell].unit != null
							 && map[selectedMapCell].unit.Owner != playerInfos[currentPlayer].owner) {
								tempunit.UpdateActionPoint(Command.Operate);

								//do attack stuff
								selectedTarget = selectedMapCell;
								Unit otherunit = map[selectedTarget].unit;

								otherunit.HitPoint = 0;

								if (otherunit.HitPoint <= 0) {
									playerInfos[otherPlayer].ownedUnit.Remove(map[selectedTarget].unit.guid);
									map.RemoveUnit(selectedTarget);
								}

								//hide target canvas
								canvas_TargetedMapCell.IsVisible = false;

								//end command
								goto finalise_command_execution;
							}
							break;

						default:
							break;
					}
					break;

finalise_command_execution:
					{
						//change cursor back
						cursor = CONTENT_MANAGER.selectCursor;
						cursorOffset = selectCursorOffset;

						if (tempunit.ActionPoint == 0) {
							tempunit.Animation.PlayAnimation(AnimationName.done.ToString());
						}

						selectedCmd = Command.None;
						isDrawTargetRange = false;

						//update fuel
						int gas = movementPath != null ? movementPath.Count : 0;

						tempunit.Gas = (tempunit.Gas - gas).Clamp(Unit._UnitStat[tempunit.UnitType].Gas, 0);

						cursor = CONTENT_MANAGER.selectCursor;
						cursorOffset = selectCursorOffset;

						attackRange = null;
						movementPath = null;
						movingAnim = null;

						//hide command menu
						HideCommandMenu();
						canvas_action_Unit.IsVisible = false;

						//Send Command to the other player for rendering

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
					if (HelperFunction.IsRightMousePressed()) {
						//hide building menu
						canvas_action_Factory.IsVisible = false;
						canvas_action_Airport.IsVisible = false;
						canvas_action_Harbor.IsVisible = false;

						//goto none
						currentGameState = GameState.None;
						break;
					}

					//check if there is a unit selected to build
					if (selectedUnitTypeToBuild != UnitType.None) {
						//check if current player have enough fund
						if (playerInfos[currentPlayer].money >= Unit._UnitStat[selectedUnitTypeToBuild].Cost) {
							playerInfos[currentPlayer].money -= Unit._UnitStat[selectedUnitTypeToBuild].Cost;
							currentGameState = GameState.BuildingBuildUnit;
						}
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
					SpawnUnit(selectedUnitTypeToBuild, playerInfos[currentPlayer], selectedBuilding);

					//hide all the building menu
					canvas_action_Factory.IsVisible = false;
					canvas_action_Airport.IsVisible = false;
					canvas_action_Harbor.IsVisible = false;

					//goto none
					currentGameState = GameState.None;
					break;
					#endregion
			}

			UpdateAnimation(gameTime);
		}


		#region Update game logic

		#region calculate vision
		private void CalculateVision() {
			foreach (string id in playerInfos[currentPlayer].ownedUnit) {

			}
		}
		#endregion

		#region Command unit
		private int GetCommands() {
			MapCell tempmapcell = map[selectedUnit];
			Unit tempunit = map[selectedUnit].unit;

			//có wait nè
			int temp = (int)Command.Wait;

			//có attack nếu có Unit địch trong tầm tấn công và tầm nhìn nè
			//check xem có phải là ranged unit? nêu có thì
			//  check xem unit có di chuyển chưa? nếu có thì
			//      không thể attack
			//  nếu không thì
			//      attack
			//nếu không thì
			//  attack
			if (tempunit.UnitType.IsRangedUnit()) {
				if (movingAnim != null) {
					//không attack
				}
				else {
					//attack
					foreach (Point p in attackRange) {
						if (map[p].unit != null
					   //the below check is only for debug
					   //&& map[p].unit.Owner != map[selectedUnit].unit.Owner) 
					   //the below check is used in final game
					   && !playerInfos[currentPlayer].ownedUnit.Contains(map[p].unit.guid)) {
							temp += (int)Command.Attack;
							break;
						}
					}
				}
			}
			else {
				//attack
				//todo làm tầm nhìn
				foreach (Point p in attackRange) {
					if (map[p].unit != null
				   //the below check is only for debug
				   //&& map[p].unit.Owner != map[selectedUnit].unit.Owner) 
				   //the below check is used in final game
				   && !playerInfos[currentPlayer].ownedUnit.Contains(map[p].unit.guid)) {
						temp += (int)Command.Attack;
						break;
					}
				}
			}

			//có capture nếu là lính và đang đứng trên building khác màu nè
			if (tempunit.UnitType.IsInfantryUnit()
			 && map[selectedUnit].terrain.IsBuildingThatIsCapturable()
			 && tempmapcell.owner != playerInfos[currentPlayer].owner) {
				temp += (int)Command.Capture;
			}

			//có supply nếu là apc và đang đứng cạnh 1 unit bạn nè
			bool isFriendlyUnitNearby = false;
			if (tempunit.UnitType == UnitType.APC) {
				if (tempunit.Ammo > 0) {
					List<Point> templistpoint = new List<Point>();
					templistpoint.Add(selectedUnit.GetNearbyPoint(Direction.North));
					templistpoint.Add(selectedUnit.GetNearbyPoint(Direction.South));
					templistpoint.Add(selectedUnit.GetNearbyPoint(Direction.East));
					templistpoint.Add(selectedUnit.GetNearbyPoint(Direction.West));
					foreach (Point p in templistpoint) {
						if (map[p] != null
						 && map[p].unit != null
						 && map[p].unit.Owner == playerInfos[currentPlayer].owner) {
							isFriendlyUnitNearby = true;
						}
					}
				}
			}
			if (isFriendlyUnitNearby) {
				temp += (int)Command.Supply;
			}

			//có operate nếu là lính và đang đứng trên missile silo hay radar
			if (tempunit.UnitType.IsInfantryUnit()
			 && tempmapcell.terrain == TerrainType.MissileSilo
				//|| tempmapcell.terrain == TerrainType.Radar
				) {
				temp += (int)Command.Operate;
			}

			//check for carrying unit
			//get surrounding cell
			var surroundingMapCell = map.GetMapCells(selectedUnit.GetNearbyPoints(Direction.North, Direction.South, Direction.East, Direction.West));

			//add loading command if the carrier
			//    doesn't carry any unit,
			//    the surrounding mapcell have atleast a unit
			if (tempunit.UnitType.IsCarrierUnit()
				&& tempunit.carryingUnit == null
				&& surroundingMapCell
				   .Any(x => x.unit != null)) {
				switch (tempunit.UnitType) {
					//apc and t-copter can only carry infantry unit
					case UnitType.APC:
					case UnitType.TransportCopter:
						surroundingMapCell.Any(x => x.unit != null && x.unit.UnitType.IsInfantryUnit());
						break;
					//lander can only carry land unit
					case UnitType.Lander:
						surroundingMapCell.Any(x => x.unit != null && x.unit.UnitType.IsLandUnit());
						break;
				}
				temp += (int)Command.Load;
			}

			if (tempunit.UnitType.IsCarrierUnit()
				&& tempunit.carryingUnit != null
				&& surroundingMapCell
				   .Any(x => x.unit == null)) {
				temp += (int)Command.Drop;
			}

			return temp;
		}

		private void ShowCommandMenu() {
			HideCommandMenu();

			canvas_action_Unit.IsVisible = true;

			CalculateAttackRange(map[selectedUnit].unit, selectedUnit);
			int comds = GetCommands();

			var cmds = comds.GetContainCommand();

			string ttemp = string.Empty;
			foreach (Command cmd in cmds) {
				ttemp += cmd.ToString() + '\n';
			}
			//CONTENT_MANAGER.ShowMessageBox(ttemp);

			Rectangle cmdslot = CommandSpriteSourceRectangle.GetSprite(cmds.Count, playerInfos[currentPlayer].owner);

			canvas_action_Unit.GetElementAs<PictureBox>("commandslot").SourceRectangle = cmdslot;
			//Point cmdslotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 50, selectedUnit.Y * Constants.MapCellHeight);
			Point cmdslotPosition = new Point(selectedUnit.X * Constants.MapCellWidth - 2, selectedUnit.Y * Constants.MapCellHeight);
			canvas_action_Unit.GetElementAs<PictureBox>("commandslot").Position = camera.TranslateFromWorldToScreen(cmdslotPosition.ToVector2()).ToPoint();

			Button firstslot = canvas_action_Unit.GetElementAs<Button>("firstslot");
			Button secondslot = canvas_action_Unit.GetElementAs<Button>("secondslot");
			Button thirdslot = canvas_action_Unit.GetElementAs<Button>("thirdslot");

			Point slotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 6, selectedUnit.Y * Constants.MapCellHeight + 8);
			slotPosition = camera.TranslateFromWorldToScreen(slotPosition.ToVector2()).ToPoint();

			firstslot.Position = slotPosition;
			firstslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[0]);
			firstslot.rect = new Rectangle(firstslot.Position, firstslot.spriteSourceRectangle.Size);

			if (cmds.Count > 1) {
				slotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 6, selectedUnit.Y * Constants.MapCellHeight + 16 + 8);
				slotPosition = camera.TranslateFromWorldToScreen(slotPosition.ToVector2()).ToPoint();
				secondslot.Position = slotPosition;
				secondslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[1]);
				secondslot.rect = new Rectangle(secondslot.Position, secondslot.spriteSourceRectangle.Size);
			}
			if (cmds.Count > 2) {
				slotPosition = new Point(selectedUnit.X * Constants.MapCellWidth + 6, selectedUnit.Y * Constants.MapCellHeight + 32 + 8);
				slotPosition = camera.TranslateFromWorldToScreen(slotPosition.ToVector2()).ToPoint();
				thirdslot.Position = slotPosition;
				thirdslot.spriteSourceRectangle = CommandSpriteSourceRectangle.GetSprite(cmds[2]);
				thirdslot.rect = new Rectangle(thirdslot.Position, thirdslot.spriteSourceRectangle.Size);
			}
		}

		private void HideCommandMenu() {
			canvas_action_Unit.IsVisible = false;

			canvas_action_Unit.GetElementAs<Button>("firstslot").rect = Rectangle.Empty;
			canvas_action_Unit.GetElementAs<Button>("secondslot").rect = Rectangle.Empty;
			canvas_action_Unit.GetElementAs<Button>("thirdslot").rect = Rectangle.Empty;

			canvas_action_Unit.GetElementAs<Button>("firstslot").spriteSourceRectangle = Rectangle.Empty;
			canvas_action_Unit.GetElementAs<Button>("secondslot").spriteSourceRectangle = Rectangle.Empty;
			canvas_action_Unit.GetElementAs<Button>("thirdslot").spriteSourceRectangle = Rectangle.Empty;
		}
		#endregion

		#region Unit handler

		private bool SelectUnit() {
			MapCell temp = map[selectedMapCell];
			return
			 (
				//check if there is a unit to select
				temp.unit != null
				//check if 
				&& temp.unit.ActionPoint > 0
				//check if this unit is the current player's unit
				&& temp.unit.Owner == playerInfos[currentPlayer].owner
			 );
		}

		private void StartMovingUnitAnimation() {
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

		private void RevertMovingUnitAnimation() {
			if (map[origin].unit == null) {
				movingAnim = null;
				map.TeleportUnit(selectedUnit, origin);
				selectedUnit = origin;
			}
		}

		//deperecated do not use
		private void DeselectUnit() {
			canvas_SelectedMapCell.GetElementAs<Label>("label_unittype").Text = " ";
			movementRange = null;
			movementPath = null;
			isMovePathCalculated = false;
			lastSelectedUnit = selectedUnit;
			selectedUnit = destination;
			destination = default(Point);
			if (currentGameState == GameState.UnitSelected) {
				currentGameState = GameState.UnitCommand;
			}
		}

		private void CalculateMovementRange(Unit unit, Point position) {
			dijkstraGraph = DijkstraHelper.CalculateGraph(map, unit, position);
			movementRange = DijkstraHelper.FindRange(dijkstraGraph);
		}

		private void CalculateAttackRange(Unit unit, Point position) {
			Range atkrange = unit.GetAttackkRange();
			attackRange = new List<Point>();

			//use to clamp the attack range inside the map
			int minx = (position.X - atkrange.Max).Clamp(position.X, 0);
			int maxx = (position.X + atkrange.Max).Clamp(map.Width - 1, position.X);
			int miny = (position.Y - atkrange.Max).Clamp(position.Y, 0);
			int maxy = (position.Y + atkrange.Max).Clamp(map.Height - 1, position.Y);

			for (int x = minx; x <= maxx; x++) {
				for (int y = miny; y <= maxy; y++) {
					Point temp = new Point(x, y);
					int dist = (int)temp.DistanceToOther(position, true);
					if (dist >= atkrange.Min && dist <= atkrange.Max) {
						attackRange.Add(temp);
					}
				}
			}
		}
		#endregion

		#region only use to demo gameplay these will not be used in game

		private void ChangeTurn() {
			//save the camera location to player
			playerInfos[currentPlayer].lastCameraLocation = camera.Location;

			if (currentPlayer == 1) {
				currentPlayer = 0;
				otherPlayer = 1;
			}
			else {
				currentPlayer = 1;
				otherPlayer = 0;
			}

			//set camera location to the current player
			camera.Location = playerInfos[currentPlayer].lastCameraLocation;

			//change buymenu color to current player color
			ChangeBuyUnitCanvasColor(playerInfos[currentPlayer].owner);
		}

		private void ChangeBuyUnitCanvasColor(GameData.Owner owner) {
			foreach (string uiname in canvas_action_Factory.UInames) {
				if (uiname.Contains("picturebox")) {
					canvas_action_Factory.GetElementAs<PictureBox>(uiname).SourceRectangle = BuyMenuFactorySpriteSourceRectangle.GetSpriteRectangle(owner);
				}
				else {
					if (!uiname.Contains("label")) {
						Rectangle temp = canvas_action_Factory.GetElementAs<Button>(uiname).spriteSourceRectangle;
						UnitType tempunittype = UnitSpriteSheetRectangle.GetUnitType(temp);
						canvas_action_Factory.GetElementAs<Button>(uiname).spriteSourceRectangle = UnitSpriteSheetRectangle.GetSpriteRectangle(tempunittype, owner);
					}
				}
			}
			foreach (string uiname in canvas_action_Airport.UInames) {
				if (uiname.Contains("picturebox")) {
					canvas_action_Airport.GetElementAs<PictureBox>(uiname).SourceRectangle = BuyMenuAirportHarborSpriteSourceRectangle.GetSpriteRectangle(owner);
				}
				else {
					if (!uiname.Contains("label")) {
						Rectangle temp = canvas_action_Airport.GetElementAs<Button>(uiname).spriteSourceRectangle;
						UnitType tempunittype = UnitSpriteSheetRectangle.GetUnitType(temp);
						canvas_action_Airport.GetElementAs<Button>(uiname).spriteSourceRectangle = UnitSpriteSheetRectangle.GetSpriteRectangle(tempunittype, owner);
					}
				}
			}
			foreach (string uiname in canvas_action_Harbor.UInames) {
				if (uiname.Contains("picturebox")) {
					canvas_action_Harbor.GetElementAs<PictureBox>(uiname).SourceRectangle = BuyMenuAirportHarborSpriteSourceRectangle.GetSpriteRectangle(owner);
				}
				else {
					if (!uiname.Contains("label")) {
						Rectangle temp = canvas_action_Harbor.GetElementAs<Button>(uiname).spriteSourceRectangle;
						UnitType tempunittype = UnitSpriteSheetRectangle.GetUnitType(temp);
						canvas_action_Harbor.GetElementAs<Button>(uiname).spriteSourceRectangle = UnitSpriteSheetRectangle.GetSpriteRectangle(tempunittype, owner);
					}
				}
			}
		}

		#endregion

		private bool SelectBuilding() {
			MapCell temp = map[selectedMapCell];
			return (
				//check if the currently selected have 
				//a building that can produce unit
				temp.terrain.IsBuildingThatProduceUnit()
			 //check if there is no unit currently standing on said building
			 && temp.unit == null
			 //check if this building is belong tu the current player
			 && temp.owner == playerInfos[currentPlayer].owner);
		}

		private void ShowBuildingMenu() {
			HideBuildingMenu();

			MapCell temp = map[selectedBuilding];

			switch (temp.terrain) {
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
		}

		private void HideBuildingMenu() {
			canvas_action_Factory.IsVisible = false;
			canvas_action_Airport.IsVisible = false;
			canvas_action_Harbor.IsVisible = false;
		}

		private bool SpawnUnit(UnitType unittype, PlayerInfo owner, Point location) {
			MapCell spawnlocation = map[location];
			if (spawnlocation != null
			  && spawnlocation.unit == null) {
				Unit temp = UnitCreationHelper.Instantiate(unittype, owner.owner);
				playerInfos[currentPlayer].ownedUnit.Add(temp.guid);
				map.RegisterUnit(location, temp);
				return true;
			}
			return false;
		}


		#endregion

		#region Update function helper

		private void UpdateCanvas_TargetedMapCell() {
			if (attackRange == null) {
				return;
			}

			if (!attackRange.Contains(selectedMapCell)) {
				return;
			}

			MapCell tempmapcell = map[selectedMapCell];

			//update the SelectedMapCell border
			canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_TargetedMapCellBorder").SourceRectangle = SelectedMapCellBorderSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.owner);

			canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_TargetedMapCellDefenseStar").SourceRectangle = SelectedMapCellDefenseStarSpriteSourceRectangle.GetSpriteRectangle(Unit._DefenseStar[tempmapcell.terrain]);

			if (tempmapcell.unit != null) {
				canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_TargetedMapCellUnitInfo").SourceRectangle = SelectedMapCellUnitInfoSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.Owner);

				canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_unitType").SourceRectangle = BackgroundUnitSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.UnitType, tempmapcell.unit.Owner, tempmapcell.terrain);
				canvas_TargetedMapCell.GetElementAs<Label>("label_unitType").Text = tempmapcell.unit.GetUnitName();

				int hp = tempmapcell.unit.HitPoint;
				canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_TargetedMapCellHPbar").SourceRectangle = new Rectangle(0, 0, hp * 10, 3);
				canvas_TargetedMapCell.GetElementAs<Label>("label_TargetedMapCellHP").Text = hp.ToString();
			}
			else {
				canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_TargetedMapCellUnitInfo").SourceRectangle = Rectangle.Empty;

				canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_unitType").SourceRectangle = Rectangle.Empty;
				canvas_TargetedMapCell.GetElementAs<Label>("label_unitType").Text = " ";

				canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_TargetedMapCellHPbar").SourceRectangle = Rectangle.Empty;
				canvas_TargetedMapCell.GetElementAs<Label>("label_TargetedMapCellHP").Text = " ";
			}

			//update the terrantype of the mapcell which is hovered on
			TerrainType tempterraintype = tempmapcell.terrain;
			if (tempterraintype == TerrainType.Coast ||
				tempterraintype == TerrainType.Cliff) {
				tempterraintype = TerrainType.Sea;
			}
			canvas_TargetedMapCell.GetElementAs<PictureBox>("picbox_terrainType").SourceRectangle = BackgroundTerrainSpriteSourceRectangle.GetSpriteRectangle(tempterraintype, map.weather, map.theme, tempmapcell.unit != null ? tempmapcell.unit.UnitType : UnitType.None, tempmapcell.owner);
			canvas_TargetedMapCell.GetElementAs<Label>("label_terrainType").Text = tempmapcell.terrain.ToString();
		}

		private void UpdateCanvas_SelectedMapCell() {
			MapCell tempmapcell = null;

			switch (currentGameState) {
				case GameState.None:
				case GameState.TurnEnd:
				case GameState.TurnStart:
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

			//update the SelectedMapCell border
			canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellBorder").SourceRectangle = SelectedMapCellBorderSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.owner);

			if (tempmapcell.terrain.IsBuildingThatIsCapturable()
			 && tempmapcell.owner != playerInfos[currentPlayer].owner
			 && tempmapcell.unit != null
			 && (tempmapcell.unit.UnitType == UnitType.Soldier
				  || tempmapcell.unit.UnitType == UnitType.Mech
					)
				) {
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellCapturePoint").SourceRectangle = SelectedMapCellCapturePointSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.Owner);

				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellCapturePoint").Text = tempmapcell.unit.CapturePoint.ToString();

				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellCapturePoint").IsVisible = true;
				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellCapturePoint").IsVisible = true;
			}
			else {
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellCapturePoint").SourceRectangle = Rectangle.Empty;
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellCapturePoint").IsVisible = false;
				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellCapturePoint").IsVisible = false;
			}

			if (tempmapcell.unit == null
			 || (tempmapcell.unit != null
				  && (tempmapcell.unit.UnitType != UnitType.TransportCopter
					  && tempmapcell.unit.UnitType != UnitType.BattleCopter
					  && tempmapcell.unit.UnitType != UnitType.Bomber
					  && tempmapcell.unit.UnitType != UnitType.Fighter
						)
					)
				) {
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellDefenseStar").SourceRectangle = SelectedMapCellDefenseStarSpriteSourceRectangle.GetSpriteRectangle(Unit._DefenseStar[tempmapcell.terrain]);
			}
			else {
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellDefenseStar").SourceRectangle = Rectangle.Empty;
			}

			if (tempmapcell.unit != null) {
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellUnitInfo").SourceRectangle = SelectedMapCellUnitInfoSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.Owner);

				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_unitType").SourceRectangle = BackgroundUnitSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.UnitType, tempmapcell.unit.Owner, tempmapcell.terrain);
				canvas_SelectedMapCell.GetElementAs<Label>("label_unitType").Text = tempmapcell.unit.GetUnitName();

				if (tempmapcell.unit.UnitType == UnitType.APC
				 || tempmapcell.unit.UnitType == UnitType.TransportCopter
				 || tempmapcell.unit.UnitType == UnitType.Lander
				 || tempmapcell.unit.UnitType == UnitType.Cruiser) {
					canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellLoadedUnit").SourceRectangle = SelectedMapCellLoadedUnitSpriteSourceRectangle.GetSpriteRectangle(tempmapcell.unit.Owner);
					canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellLoadedUnit").IsVisible = true;
					//update loaded unit here
				}
				else {
					canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellLoadedUnit").SourceRectangle = Rectangle.Empty;
					canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellLoadedUnit").IsVisible = false;
				}

				int hp = tempmapcell.unit.HitPoint;
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellHPbar").SourceRectangle = new Rectangle(0, 0, hp * 10, 3);
				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellHP").Text = hp.ToString();

				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellUnitInfo_fuel").Text = tempmapcell.unit.Gas.ToString();
				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellUnitInfo_ammo").Text = tempmapcell.unit.Ammo.ToString();
			}
			else {
				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellUnitInfo").SourceRectangle = Rectangle.Empty;

				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_unitType").SourceRectangle = Rectangle.Empty;
				canvas_SelectedMapCell.GetElementAs<Label>("label_unitType").Text = " ";

				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellLoadedUnit").SourceRectangle = Rectangle.Empty;

				canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_SelectedMapCellHPbar").SourceRectangle = Rectangle.Empty;
				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellHP").Text = " ";

				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellUnitInfo_fuel").Text = " ";
				canvas_SelectedMapCell.GetElementAs<Label>("label_SelectedMapCellUnitInfo_ammo").Text = " ";
			}

			//update the terrantype of the mapcell which is hovered on
			TerrainType tempterraintype = tempmapcell.terrain;
			if (tempterraintype == TerrainType.Coast ||
				tempterraintype == TerrainType.Cliff) {
				tempterraintype = TerrainType.Sea;
			}
			canvas_SelectedMapCell.GetElementAs<PictureBox>("picbox_terrainType").SourceRectangle = BackgroundTerrainSpriteSourceRectangle.GetSpriteRectangle(tempterraintype, map.weather, map.theme, tempmapcell.unit != null ? tempmapcell.unit.UnitType : UnitType.None, tempmapcell.owner);
			canvas_SelectedMapCell.GetElementAs<Label>("label_terrainType").Text = tempmapcell.terrain.ToString();
		}

		private void UpdateAnimation(GameTime gameTime) {
			if (map == null) {
				return;
			}

			foreach (Point p in map.mapcellthathaveunit) {
				map[p].unit.Animation.Update(gameTime);
			}
		}

		private void MoveCamera(KeyboardState keyboardInputState, MouseState mouseInputState) {
			int speed = 10;

			if (CONTENT_MANAGER.lastInputState.keyboardState.IsKeyDown(Keys.LeftShift) || CONTENT_MANAGER.lastInputState.keyboardState.IsKeyDown(Keys.RightShift)) {
				speed = 50;
			}

			//simulate scrolling
			if (keyboardInputState.IsKeyDown(Keys.Left)
				 || keyboardInputState.IsKeyDown(Keys.A)
				 || (mouseInputState.Position.X.Between(50, 10) && isEnableEdgeScrolling)) {
				camera.Location += new Vector2(-1, 0) * speed;
			}
			if (keyboardInputState.IsKeyDown(Keys.Right)
				|| keyboardInputState.IsKeyDown(Keys.D)
				|| (mouseInputState.Position.X.Between(710, 670) && isEnableEdgeScrolling)) {
				camera.Location += new Vector2(1, 0) * speed;
			}
			if (keyboardInputState.IsKeyDown(Keys.Up)
				|| keyboardInputState.IsKeyDown(Keys.W)
				|| (mouseInputState.Position.Y.Between(50, 10) && isEnableEdgeScrolling)) {
				camera.Location += new Vector2(0, -1) * speed;
			}
			if (keyboardInputState.IsKeyDown(Keys.Down)
				|| keyboardInputState.IsKeyDown(Keys.S)
				|| (mouseInputState.Position.Y.Between(470, 430) && isEnableEdgeScrolling)) {
				camera.Location += new Vector2(0, 1) * speed;
			}

			Point clampMax = new Point(map.Width * Constants.MapCellWidth - 720 + 96, map.Height * Constants.MapCellHeight - 480 + 96);

			camera.Location = new Vector2(camera.Location.X.Clamp(clampMax.X, -96), camera.Location.Y.Clamp(clampMax.Y, -96));
		}

		#endregion

		#region Draw
		public override void Draw(GameTime gameTime) {
			DrawMap(CONTENT_MANAGER.spriteBatch, gameTime);

			//draw the guibackground
			//CONTENT_MANAGER.spriteBatch.Draw(guibackground, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
			canvas.Draw(CONTENT_MANAGER.spriteBatch);

			//CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, currentGameState.ToString(), new Vector2(100, 100), Color.Red);
			if (currentGameState == GameState.BuildingSelected) {
				//CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, canvas_action_Factory.GetElementAs<Label>("label_unitname").Position.toString(), new Vector2(100, 140), Color.Red);
			}
			CONTENT_MANAGER.spriteBatch.DrawString(CONTENT_MANAGER.defaultfont, currentGameState.ToString() + Environment.NewLine + selectedCmd.ToString(), new Vector2(100, 140), Color.Red);

			//draw canvas_SelectedMapCell
			//DrawCanvas_SelectedMapCell();


			//draw the minimap
			//CONTENT_MANAGER.spriteBatch.Draw(minimap, minimapbound, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, LayerDepth.GuiBackground);
			//CONTENT_MANAGER.spriteBatch.Draw(CONTENT_MANAGER.SelectedMapCell_HPbar, Vector2.Zero, new Rectangle(0, 0, 100, 3), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);
		}

		private void DrawMap(SpriteBatch spriteBatch, GameTime gameTime) {
			spriteBatch.End();

			spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);

			//render the map
			MapRenderer.Render(map, spriteBatch, gameTime);

			//draw moving animation
			if (currentGameState == GameState.UnitMove) {
				movingAnim.Draw(spriteBatch, gameTime);
			}

			//draw selected unit's movement range
			if (currentGameState == GameState.UnitSelected) {
				DrawMovementRange(spriteBatch);
			}

			//draw movementpath direction arrow if exist
			if (currentGameState == GameState.UnitSelected && movementPath != null) {
				dirarrowRenderer.UpdatePath(movementPath);
				dirarrowRenderer.Draw(spriteBatch);
			}

			//draw attackrange
			//if (currentGameState == GameState.UnitCommand && (selectedCmd == Command.Attack || selectedCmd == Command.Operate)) {
			if (isDrawTargetRange) {
				DrawAttackRange(spriteBatch);
			}

			//draw the cursor
			spriteBatch.Draw(cursor, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), null, Color.White, 0f, cursorOffset, 1f, SpriteEffects.None, LayerDepth.GuiUpper);

			spriteBatch.End();

			spriteBatch.Begin(SpriteSortMode.FrontToBack);
		}

		private void DrawMovementRange(SpriteBatch spriteBatch) {
			if (movementRange != null) {
				foreach (Point dest in movementRange) {
					spriteBatch.Draw(CONTENT_MANAGER.moveOverlay, new Vector2(dest.X * Constants.MapCellWidth, dest.Y * Constants.MapCellHeight), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
				}
			}
		}

		private void DrawAttackRange(SpriteBatch spriteBatch) {
			if (attackRange != null) {
				foreach (Point p in attackRange) {
					spriteBatch.Draw(CONTENT_MANAGER.attackOverlay, new Vector2(p.X * Constants.MapCellWidth, p.Y * Constants.MapCellHeight), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
				}
			}
		}
		#endregion
	}
}
