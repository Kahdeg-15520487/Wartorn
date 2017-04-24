using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;
using Wartorn.Utility;
using Wartorn.UIClass;
using Wartorn.ScreenManager;
using Wartorn.Screens;
using System;

using GeonBit.UI;
using GeonBitUI = GeonBit.UI.Entities;

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
            SpriteSheetSourceRectangle.LoadSprite();
            UISpriteSheetSourceRectangle.LoadSprite();

            graphics.PreferredBackBufferWidth = Constants.Width;    // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = Constants.Height;  // set this value to the desired height of your window
            graphics.ApplyChanges();

            DrawingHelper.Initialize(GraphicsDevice);
            
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
            CONTENT_MANAGER.spriteSheet = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\terrain");

            CONTENT_MANAGER.UIspriteSheet = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\ui_sprite_sheet");

            InitUI();
            InitScreens();
        }

        private void InitUI()
        {
            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.ShowCursor = false;

            GeonBitUI.Panel imagepanel = new GeonBitUI.Panel(new Vector2(716, 200), skin: GeonBitUI.PanelSkin.Simple, anchor: GeonBitUI.Anchor.Center);

            GeonBitUI.Label currentlySelectedTerrain = new GeonBitUI.Label("", anchor: GeonBitUI.Anchor.TopLeft, offset: new Vector2(-20, -20));
            imagepanel.AddChild(currentlySelectedTerrain);
            //water selection
            int col = 0;
            int row = 0;
            Vector2 offset = new Vector2(-20, 5);
            for (SpriteSheetTerrain i = SpriteSheetTerrain.Reef; i.CompareWith(SpriteSheetTerrain.Invert_Coast_right_down) <= 0; i = i.Next())
            {
                //Button temp = new Button(SpriteSheetSourceRectangle.GetSpriteRectangle(i), new Point(col * 26 + 10, row * 26 + 80), 0.5f, false);
                GeonBitUI.Image temp = new GeonBitUI.Image(CONTENT_MANAGER.spriteSheet, new Vector2(24, 24), anchor: GeonBitUI.Anchor.TopLeft, offset: offset + new Vector2(26 * col, 26 * row));
                temp.SourceRectangle = SpriteSheetSourceRectangle.GetSpriteRectangle(i);
                Rectangle temprectangle = temp.SourceRectangle ?? default(Rectangle);
                temp.OnClick += (sender) =>
                {
                    currentlySelectedTerrain.Text = SpriteSheetSourceRectangle.GetTerrain(temprectangle).ToString();
                };

                imagepanel.AddChild(temp);
                col++;
                if (col == 27)
                {
                    col = 0;
                    row++;
                }
            }

            //UserInterface.AddEntity(imagepanel);
        }

        private void InitScreens()
        {
            SCREEN_MANAGER.add_screen(new EditorScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new MainMenuScreen(GraphicsDevice));

            //SCREEN_MANAGER.goto_screen("MainMenuScreen");
            SCREEN_MANAGER.goto_screen("EditorScreen");

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

            UserInterface.Update(gameTime);

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

            UserInterface.Draw(CONTENT_MANAGER.spriteBatch);

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
