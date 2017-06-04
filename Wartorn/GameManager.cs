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
using Wartorn.Utility.Drawing;
using Wartorn.CustomJsonConverter;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wartorn
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameManager : Game
    {
        GraphicsDeviceManager graphics;

        InputState inputState, lastInputState;

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            CONTENT_MANAGER.Content = Content;
            IsMouseVisible = true;
            lastInputState = new InputState();

            graphics.PreferMultiSampling = true;

            JsonConvert.DefaultSettings = () =>
            {
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
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            TerrainSpriteSourceRectangle.LoadSprite();
            UISpriteSheetSourceRectangle.LoadSprite();
            UnitSpriteSheetRectangle.LoadSprite();
            BuildingSpriteSourceRectangle.LoadSprite();
            DirectionArrowSpriteSourceRectangle.LoadSprite();
            CommandSpriteSourceRectangle.LoadSprite();
            BackgroundTerrainSpriteSourceRectangle.LoadSprite();
            BackgroundUnitSpriteSourceRectangle.LoadSprite();
            GeneralInfoBorderSpriteSourceRectangle.LoadSprite();
            GeneralInfoCapturePointSpriteSourceRectangle.LoadSprite();
            GeneralInfoDefenseStarSpriteSourceRectangle.LoadSprite();
            GeneralInfoLoadedUnitSpriteSourceRectangle.LoadSprite();
            GeneralInfoUnitInfoSpriteSourceRectangle.LoadSprite();
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
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            CONTENT_MANAGER.spriteBatch = new SpriteBatch(GraphicsDevice);
            CONTENT_MANAGER.inputState = new InputState(Mouse.GetState(), Keyboard.GetState());

            CONTENT_MANAGER.LoadContent();
            
            InitScreen();
        }        

        private void InitScreen()
        {
            SCREEN_MANAGER.add_screen(new EditorScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new MainMenuScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new TestAnimationScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new Screens.MainGameScreen.SetupScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new Screens.MainGameScreen.GameScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new TestConsole(GraphicsDevice));

            //SCREEN_MANAGER.goto_screen("TestAnimationScreen");
            //SCREEN_MANAGER.goto_screen("SetupScreen");
            SCREEN_MANAGER.goto_screen("MainMenuScreen");
            //SCREEN_MANAGER.goto_screen("EditorScreen");

            SCREEN_MANAGER.Init();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Environment.Exit(0);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            // TODO: Add your update logic here
            CONTENT_MANAGER.inputState = new InputState(Mouse.GetState(), Keyboard.GetState());

            if (HelperFunction.IsKeyPress(Keys.F1))
            {
                SCREEN_MANAGER.goto_screen("TestAnimationScreen");
            }

            if (HelperFunction.IsKeyPress(Keys.F2))
            {
                Unit.Load();
                CONTENT_MANAGER.ShowMessageBox("Unit stats reloaded");
            }

            if (HelperFunction.IsKeyPress(Keys.F3))
            {
                SCREEN_MANAGER.goto_screen("TestConsole");
            }

            SCREEN_MANAGER.Update(gameTime);

            lastInputState = inputState;
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            CONTENT_MANAGER.spriteBatch.Begin(SpriteSortMode.FrontToBack);
            {
                SCREEN_MANAGER.Draw(gameTime);
            }
            CONTENT_MANAGER.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
