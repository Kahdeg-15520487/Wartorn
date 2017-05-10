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
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

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

            JsonConvert.DefaultSettings  = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new UnitPairJsonConverter());
                settings.Converters.Add(new UnitJsonConverter());
                settings.Converters.Add(new UnitTypeJsonConverter());
                settings.Converters.Add(new MapJsonConverter());
                settings.Converters.Add(new MapCellJsonConverter());
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
            TerrainSpriteSheetSourceRectangle.LoadSprite();
            UISpriteSheetSourceRectangle.LoadSprite();
            UnitSpriteSheetRectangle.LoadSprite();
            BuildingSpriteSourceRectangle.LoadSprite();

            graphics.PreferredBackBufferWidth = Constants.Width;    // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = Constants.Height;  // set this value to the desired height of your window
            graphics.ApplyChanges();

            DrawingHelper.Initialize(GraphicsDevice);

            Unit.Init();
            //Unit.Load();

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

            // TODO: use this.Content to load your game content here
            CONTENT_MANAGER.defaultfont = CONTENT_MANAGER.Content.Load<SpriteFont>("defaultfont");
            CONTENT_MANAGER.arcadefont = CONTENT_MANAGER.Content.Load<SpriteFont>(@"sprite\GUI\menufont");
            CONTENT_MANAGER.spriteSheet = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\terrain");
            CONTENT_MANAGER.buildingSpriteSheet = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\building");
            CONTENT_MANAGER.UIspriteSheet = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\ui_sprite_sheet");
            CONTENT_MANAGER.unitSpriteSheet = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\unit");
            CONTENT_MANAGER.blank8x8 = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\blank8x8");

            LoadAnimationContent();
            InitScreen();
        }

        private void LoadAnimationContent()
        {
            string delimit = "Yellow";
            CONTENT_MANAGER.animationEntities = new Dictionary<SpriteSheetUnit, AnimatedEntity>();
            CONTENT_MANAGER.animationSheets = new Dictionary<SpriteSheetUnit, Texture2D>();
            CONTENT_MANAGER.animationTypes = new List<Animation>();

            //list of unit type
            var UnitTypes = new List<SpriteSheetUnit>((IEnumerable<SpriteSheetUnit>)Enum.GetValues(typeof(SpriteSheetUnit)));
            //Artillery
            //load animation sprite sheet for each unit type
            foreach (SpriteSheetUnit unittype in UnitTypes)
            {
                var paths = unittype.ToString().Split('_');
                if (paths[0].CompareTo(delimit) == 0)
                {
                    break;
                }
                CONTENT_MANAGER.animationSheets.Add(unittype, CONTENT_MANAGER.Content.Load<Texture2D>("sprite//Animation//" + paths[0] + "//" + paths[1]));
            }

            //declare animation frame

            //animation frame for "normal" unit
            Animation idle = new Animation("idle", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                idle.AddKeyFrame(i * 48, 0, 48, 48);
            }

            Animation right = new Animation("right", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                right.AddKeyFrame(i * 48, 48, 48, 48);
            }

            Animation up = new Animation("up", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                up.AddKeyFrame(i * 48, 96, 48, 48);
            }

            Animation down = new Animation("down", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                down.AddKeyFrame(i * 48, 144, 48, 48);
            }

            Animation done = new Animation("done", true, 1, string.Empty);
            done.AddKeyFrame(0, 192, 48, 48);

            //animation frame for "HIGH" unit
            Animation idleAir = new Animation("idle", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                idleAir.AddKeyFrame(i * 64, 0, 64, 64);
            }

            Animation rightAir = new Animation("right", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                rightAir.AddKeyFrame(i * 64, 64, 64, 64);
            }

            Animation upAir = new Animation("up", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                upAir.AddKeyFrame(i * 64, 128, 64, 64);
            }

            Animation downAir = new Animation("down", true, 4, string.Empty);
            for (int i = 0; i < 4; i++)
            {
                downAir.AddKeyFrame(i * 64, 192, 64, 64);
            }

            Animation doneAir = new Animation("done", true, 1, string.Empty);
            doneAir.AddKeyFrame(0, 256, 64, 64);

            //animation frame for copter unit
            Animation idleCopter = new Animation("idle", true, 3, string.Empty);
            for (int i = 0; i < 3; i++)
            {
                idleCopter.AddKeyFrame(i * 64, 0, 64, 64);
            }

            CONTENT_MANAGER.animationTypes.Add(idle);
            CONTENT_MANAGER.animationTypes.Add(right);
            CONTENT_MANAGER.animationTypes.Add(up);
            CONTENT_MANAGER.animationTypes.Add(down);
            CONTENT_MANAGER.animationTypes.Add(done);

            foreach (SpriteSheetUnit unittype in UnitTypes)
            {
                string unittypestring = unittype.ToString();
                AnimatedEntity temp = new AnimatedEntity(Vector2.Zero, Vector2.Zero, Color.White, LayerDepth.Unit);
                if (unittypestring.Contains(delimit))
                {
                    break;
                }

                temp.LoadContent(CONTENT_MANAGER.animationSheets[unittype]);

                if (unittypestring.Contains("TransportCopter")
                  || unittypestring.Contains("BattleCopter")
                  || unittypestring.Contains("Fighter")
                  || unittypestring.Contains("Bomber"))
                {
                    //we enter "HIGH" mode
                    //first we set the origin to "HIGH"


                    //then we load the "HIGH" animation in
                    if (unittypestring.Contains("Copter"))
                    {
                        temp.AddAnimation(idleCopter, rightAir, upAir, downAir, doneAir);
                    }
                    else
                    {
                        temp.AddAnimation(idleAir, rightAir, upAir, downAir, doneAir);
                    }
                }
                else
                {
                    //we enter "normal" mode
                    temp.AddAnimation(idle, right, up, down, done);
                }

                temp.PlayAnimation("idle");
                CONTENT_MANAGER.animationEntities.Add(unittype, temp);
            }
        }

        private void InitScreen()
        {
            SCREEN_MANAGER.add_screen(new EditorScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new MainMenuScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new TestAnimationScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new Screens.MainGameScreen.SetupScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new Screens.MainGameScreen.GameScreen(GraphicsDevice));

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
