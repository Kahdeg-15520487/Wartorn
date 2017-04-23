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
            //IsMouseVisible = true;
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

            SCREEN_MANAGER.add_screen(new EditorScreen(GraphicsDevice));
            SCREEN_MANAGER.add_screen(new MainMenuScreen(GraphicsDevice));
            

            //SCREEN_MANAGER.goto_screen("MainMenuScreen");
            //SCREEN_MANAGER.goto_screen("EditorScreen");

            DrawingHelper.Initialize(GraphicsDevice);

            UserInterface.Initialize(Content, BuiltinThemes.hd);

            GeonBitUI.Panel panel = new GeonBitUI.Panel(new Vector2(200, 200), anchor: GeonBitUI.Anchor.TopCenter);
            GeonBitUI.Button subject = new GeonBitUI.Button("lala", size: new Vector2(150, 40), anchor: GeonBitUI.Anchor.Center);
            GeonBitUI.Button hider = new GeonBitUI.Button("hide",size: new Vector2(150,40),anchor: GeonBitUI.Anchor.TopCenter);
            GeonBitUI.Label info = new GeonBitUI.Label("not hidden", anchor: GeonBitUI.Anchor.BottomCenter);
            panel.AddChild(subject);
            panel.AddChild(hider);
            panel.AddChild(info);

            GeonBitUI.Panel hidepanel = new GeonBitUI.Panel(new Vector2(200, 100), anchor: GeonBitUI.Anchor.BottomCenter);
            GeonBitUI.Label hideinfo = new GeonBitUI.Label("not hidden", anchor: GeonBitUI.Anchor.Center);
            hidepanel.AddChild(hideinfo);

            hider.OnClick = (sender) =>
            {
                subject.Visible = !subject.Visible;
                info.Text = subject.Visible ? "not hidden" : "hidden";

                hidepanel.Visible = !hidepanel.Visible;
                hideinfo.Text = hidepanel.Visible ? "not hidden" : "hidden";
            };

            subject.OnClick = (sender) =>
            {
                info.Text = "lala";
                hideinfo.Text = "lala";
            };

            UserInterface.AddEntity(panel);
            UserInterface.AddEntity(hidepanel);


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

            //expriment

            GeonBitUI.Panel imagepanel = new GeonBitUI.Panel(new Vector2(716, 200),skin: GeonBitUI.PanelSkin.Simple, anchor: GeonBitUI.Anchor.Center);

            GeonBitUI.Label currentlySelectedTerrain = new GeonBitUI.Label("",anchor: GeonBitUI.Anchor.TopLeft,offset: new Vector2(-20,-20));
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

            UserInterface.AddEntity(imagepanel);

            //end experiment

            //SCREEN_MANAGER.Init();
            //InitializeUI();
        }
        #region Init UI example
        /*
        void InitializeUI()
        {
            Label label1 = new Label()
            {
                Text = "1",
                Position = new Point(50,50),
                Size = new Vector2(100,50),
                font = defaultFont,
                foregroundColor = Color.Black,
                Scale = 2
            };

            Label label2 = new Label()
            {
                Text = "",
                Position = new Point(10, 400),
                Size = new Vector2(100, 30),
                font = defaultFont,
                foregroundColor = Color.White
            };

            Label label3 = new Label()
            {
                Text = string.Empty,
                Position = new Point(10, 440),
                Size = new Vector2(100, 30),
                font = defaultFont,
                foregroundColor = Color.White
            };

            Label labelTime = new Label()
            {
                Text = string.Empty,
                Position = new Point(5,5),
                Size = new Vector2(100, 30),
                font = defaultFont,
                foregroundColor = Color.White
            };

            label1.MouseEnter += delegate (object sender, UIEventArgs e)
            {
                label3.Text = "enter";
            };
            label1.MouseLeave += delegate (object sender, UIEventArgs e)
            {
                label3.Text = "leave";
            };

            Button button1 = new Button()
            {
                Text = "test",
                Position = new Point(200, 200),
                Size = new Vector2(50, 50),
                font = defaultFont,
                backgroundColor = Color.White,
                foregroundColor = Color.Black,
                ButtonColorPressed = Color.LightSlateGray,
                ButtonColorReleased = Color.LightGray
            };

            button1.MouseUp += delegate (object sender, UIEventArgs e)
            {
                int temp;
                if (int.TryParse(label1.Text,out temp))
                {
                    label1.Text = (temp + 1).ToString();
                }
            };

            InputBox inputbox1 = new InputBox()
            {
                Text = "test",
                Position = new Point(300, 200),
                Size = new Vector2(50, 50),
                font = defaultFont,
                backgroundColor = Color.White,
                foregroundColor = Color.White
            };

            canvas.AddElement("label1", label1);
            canvas.AddElement("label2", label2);
            canvas.AddElement("label3", label3);
            canvas.AddElement("button1", button1);
            canvas.AddElement("labelTime", labelTime);
            canvas.AddElement("inputbox1", inputbox1);
        }
        */
        #endregion
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
