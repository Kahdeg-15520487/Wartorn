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

using Wartorn.CustomJsonConverter;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fclp;

namespace Wartorn {
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class GameManager : Game {
		GraphicsDeviceManager graphics;

		InputState inputState, lastInputState;

		public GameManager() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			CONTENT_MANAGER.Content = Content;
			IsMouseVisible = true;
			lastInputState = new InputState();
			graphics.PreferMultiSampling = true;

			JsonConvert.DefaultSettings = () => {
				var settings = new JsonSerializerSettings();
				settings.Converters.Add(new UnitPairJsonConverter());
				settings.Converters.Add(new UnitJsonConverter());
				settings.Converters.Add(new UnitTypeJsonConverter());
				settings.Converters.Add(new MovementTypeJsonConverter());
				settings.Converters.Add(new TerrainTypeJsonConverter());
				settings.Converters.Add(new RangeJsonConverter());
				settings.Converters.Add(new MapJsonConverter());
				settings.Converters.Add(new MapCellJsonConverter());
				settings.Converters.Add(new Dictionary_MovementType_Dictionary_TerrainType_int_JsonConverter());
				settings.Converters.Add(new Dictionary_UnitType_Dictionary_UnitType_int_JsonConverter());
				return settings;
			};
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			TerrainSpriteSourceRectangle.LoadSprite();
			UISpriteSheetSourceRectangle.LoadSprite();
			UnitSpriteSheetRectangle.LoadSprite();
			BuildingSpriteSourceRectangle.LoadSprite();
			DirectionArrowSpriteSourceRectangle.LoadSprite();
			CommandSpriteSourceRectangle.LoadSprite();
			BackgroundTerrainSpriteSourceRectangle.LoadSprite();
			BackgroundUnitSpriteSourceRectangle.LoadSprite();
			SelectedMapCellBorderSpriteSourceRectangle.LoadSprite();
			SelectedMapCellCapturePointSpriteSourceRectangle.LoadSprite();
			SelectedMapCellDefenseStarSpriteSourceRectangle.LoadSprite();
			SelectedMapCellLoadedUnitSpriteSourceRectangle.LoadSprite();
			SelectedMapCellUnitInfoSpriteSourceRectangle.LoadSprite();
			BuyMenuFactorySpriteSourceRectangle.LoadSprite();
			BuyMenuAirportHarborSpriteSourceRectangle.LoadSprite();

			graphics.PreferredBackBufferWidth = Constants.Width;    // set this value to the desired width of your window
			graphics.PreferredBackBufferHeight = Constants.Height;  // set this value to the desired height of your window
			graphics.ApplyChanges();

			
			DrawingHelper.Initialize(GraphicsDevice);

			Unit.Init();
			Unit.Load();

			CONTENT_MANAGER.gameinstance = this;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			// Create a new SpriteBatch, which can be used to draw textures.
			CONTENT_MANAGER.spriteBatch = new SpriteBatch(GraphicsDevice);
			CONTENT_MANAGER.currentInputState = new InputState(Mouse.GetState(), Keyboard.GetState());

			var spritelist = new string[] {
				@"AttackOverlay",
				@"blank8x8",
				@"buildingSpriteSheet",
				@"directionarrow",
				@"MoveOverlay",
				@"showtile",
				@"spriteSheet",
				@"UIspriteSheet",
				@"unitSpriteSheet",
				@"Cursor\Attack_Bubble",
				@"Cursor\Attack_Cursor",
				@"Cursor\Buy_Cursor",
				@"Cursor\Damage_Number",
				@"Cursor\Select_Cursor",
				@"GUI\commandspritesheet",
				@"GUI\Mode_select_dark",
				@"GUI\Mode_select_light",
				@"GUI\placeholdergui",
				@"GUI\Select_screen",
				@"GUI\Startscreen",
				@"GUI\Startscreen_Airstrike",
				@"GUI\Startscreen_Battleship",
				@"GUI\Startscreen_Wartorn",
				@"GUI\Title",
				@"GUI\background\background_terrain",
				@"GUI\background\background_unit",
				@"GUI\background\BG_Desert",
				@"GUI\background\BG_Normal",
				@"GUI\background\BG_Rain",
				@"GUI\background\BG_Snow",
				@"GUI\background\BG_Tropical",
				@"GUI\background\BLUE_Unit",
				@"GUI\background\GREEN_Unit",
				@"GUI\background\RED_Unit",
				@"GUI\background\YELLOW_Unit",
				@"GUI\commandspritesheet\commandspritesheet",
				@"GUI\endgame\Background1",
				@"GUI\endgame\Background2",
				@"GUI\endgame\Background3",
				@"GUI\endgame\Background4",
				@"GUI\endgame\Background5",
				@"GUI\endgame\Background6",
				@"GUI\endgame\Background7",
				@"GUI\endgame\Background8",
				@"GUI\selected_cell_info\SC_Border",
				@"GUI\selected_cell_info\SC_Buy_A_H",
				@"GUI\selected_cell_info\SC_Buy_F",
				@"GUI\selected_cell_info\SC_Cap_Point",
				@"GUI\selected_cell_info\SC_Def_Star",
				@"GUI\selected_cell_info\SC_HP_bar",
				@"GUI\selected_cell_info\SC_Loaded",
				@"GUI\selected_cell_info\SC_Unit_info",
				@"tutorial\gamescreentutorial_en",
				@"tutorial\gamescreentutorial_vi",
				@"tutorial\mapeditortutorial_en",
				@"tutorial\mapeditortutorial_vi",
				@"tutorial\setupscreentutorial_en",
				@"tutorial\setupscreentutorial_vi"
			};

			var spritesheetlist = new string[] {

				@"Animation\Blue\AntiAir",
				@"Animation\Blue\APC",
				@"Animation\Blue\Artillery",
				@"Animation\Blue\BattleCopter",
				@"Animation\Blue\Battleship",
				@"Animation\Blue\Bomber",
				@"Animation\Blue\Cruiser",
				@"Animation\Blue\Fighter",
				@"Animation\Blue\HeavyTank",
				@"Animation\Blue\Lander",
				@"Animation\Blue\Mech",
				@"Animation\Blue\Missile",
				@"Animation\Blue\Recon",
				@"Animation\Blue\Rocket",
				@"Animation\Blue\Soldier",
				@"Animation\Blue\Submarine",
				@"Animation\Blue\Tank",
				@"Animation\Blue\TransportCopter",
				@"Animation\Green\AntiAir",
				@"Animation\Green\APC",
				@"Animation\Green\Artillery",
				@"Animation\Green\BattleCopter",
				@"Animation\Green\Battleship",
				@"Animation\Green\Bomber",
				@"Animation\Green\Cruiser",
				@"Animation\Green\Fighter",
				@"Animation\Green\HeavyTank",
				@"Animation\Green\Lander",
				@"Animation\Green\Mech",
				@"Animation\Green\Missile",
				@"Animation\Green\Recon",
				@"Animation\Green\Rocket",
				@"Animation\Green\Soldier",
				@"Animation\Green\Submarine",
				@"Animation\Green\Tank",
				@"Animation\Green\TransportCopter",
				@"Animation\Red\AntiAir",
				@"Animation\Red\APC",
				@"Animation\Red\Artillery",
				@"Animation\Red\BattleCopter",
				@"Animation\Red\Battleship",
				@"Animation\Red\Bomber",
				@"Animation\Red\Cruiser",
				@"Animation\Red\Fighter",
				@"Animation\Red\HeavyTank",
				@"Animation\Red\Lander",
				@"Animation\Red\Mech",
				@"Animation\Red\Missile",
				@"Animation\Red\Recon",
				@"Animation\Red\Rocket",
				@"Animation\Red\Soldier",
				@"Animation\Red\Submarine",
				@"Animation\Red\Tank",
				@"Animation\Red\TransportCopter",
				@"Animation\Yellow\AntiAir",
				@"Animation\Yellow\APC",
				@"Animation\Yellow\Artillery",
				@"Animation\Yellow\BattleCopter",
				@"Animation\Yellow\Battleship",
				@"Animation\Yellow\Bomber",
				@"Animation\Yellow\Cruiser",
				@"Animation\Yellow\Fighter",
				@"Animation\Yellow\HeavyTank",
				@"Animation\Yellow\Lander",
				@"Animation\Yellow\Mech",
				@"Animation\Yellow\Missile",
				@"Animation\Yellow\Recon",
				@"Animation\Yellow\Rocket",
				@"Animation\Yellow\Soldier",
				@"Animation\Yellow\Submarine",
				@"Animation\Yellow\Tank",
				@"Animation\Yellow\TransportCopter"
			};

			//todo make these spritesheet
			CONTENT_MANAGER.LoadSprites(spritelist);
			CONTENT_MANAGER.LoadSprites(spritesheetlist);
			//todo load endgameBG
			CONTENT_MANAGER.LoadFont("defaultfont", "arcadefont", "hackfont");
			CONTENT_MANAGER.LoadSound(@"yessir\yes1", @"speech\moving_out");

			InitScreen();
		}

		private void InitScreen() {
			SCREEN_MANAGER.add_screen(new EditorScreen(GraphicsDevice));
			SCREEN_MANAGER.add_screen(new MainMenuScreen(GraphicsDevice));
			//SCREEN_MANAGER.add_screen(new TestAnimationScreen(GraphicsDevice));
			SCREEN_MANAGER.add_screen(new Screens.MainGameScreen.SetupScreen(GraphicsDevice));
			SCREEN_MANAGER.add_screen(new Screens.MainGameScreen.GameScreen(GraphicsDevice));
			//SCREEN_MANAGER.add_screen(new TestConsole(GraphicsDevice));
			SCREEN_MANAGER.add_screen(new Screens.MainGameScreen.EndGameScreen(GraphicsDevice));

			//SCREEN_MANAGER.goto_screen("TestAnimationScreen");
			//SCREEN_MANAGER.goto_screen("SetupScreen");
			if (string.IsNullOrEmpty(CONTENT_MANAGER.MapName)) {
				SCREEN_MANAGER.goto_screen("MainMenuScreen");
			}
			else {
				SCREEN_MANAGER.goto_screen("SetupScreen");
				var setupscreen = ((Screens.MainGameScreen.SetupScreen)SCREEN_MANAGER.get_screen("SetupScreen"));
				setupscreen.LoadMap(Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", CONTENT_MANAGER.MapName));
				setupscreen.SetUpSessionDataAndLaunchMainGame();
			}
			//SCREEN_MANAGER.goto_screen("EditorScreen");

			SCREEN_MANAGER.Init();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent() {
			Environment.Exit(0);
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			//if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			//    Exit();

			CONTENT_MANAGER.currentInputState = new InputState(Mouse.GetState(), Keyboard.GetState());

			//if (HelperFunction.IsKeyPress(Keys.F1))
			//{
			//    SCREEN_MANAGER.goto_screen("TestAnimationScreen");
			//}

			if (HelperFunction.IsKeyPress(Keys.F2)) {
				Unit.Load();
				CONTENT_MANAGER.ShowMessageBox("Unit stats reloaded");
			}

			//if (HelperFunction.IsKeyPress(Keys.F3))
			//{
			//    SCREEN_MANAGER.goto_screen("TestConsole");
			//}

			SCREEN_MANAGER.Update(gameTime);

			lastInputState = inputState;
			base.Update(gameTime);
		}


		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);

			CONTENT_MANAGER.BeginSpriteBatch();
			{
				SCREEN_MANAGER.Draw(gameTime);
			}
			CONTENT_MANAGER.EndSpriteBatch();

			base.Draw(gameTime);
		}
	}
}
