using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Wartorn.Utility.Drawing;
using Wartorn.UIClass;

namespace Wartorn
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameManager : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Canvas canvas;
        SpriteFont defaultFont;

        InputState inputState;
        InputState lastInputState;

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            lastInputState = new InputState();       
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
            graphics.PreferredBackBufferWidth = Utility.Constants.Width;    // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = Utility.Constants.Height;  // set this value to the desired height of your window
            graphics.ApplyChanges();

            canvas = new Canvas();

            DrawingHelper.Initialize(base.GraphicsDevice);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            canvas.LoadContent(Content);
            defaultFont = Content.Load<SpriteFont>("defaultfont");

            InitializeUI();
        }

        void InitializeUI()
        {
            Label label1 = new Label()
            {
                Text = "1",
                Position = new Point(50,50),
                Size = new Vector2(100,50),
                font = defaultFont,
                backgroundColor = Color.White,
                foregroundColor = Color.Black,
                rotation = 0f,
                Scale = 2
            };

            Label label2 = new Label()
            {
                Text = "",
                Position = new Point(10, 400),
                Size = new Vector2(100, 30),
                font = defaultFont,
                backgroundColor = Color.White,
                foregroundColor = Color.Black,
                rotation = 0f,
                Scale = 1
            };

            Label label3 = new Label()
            {
                Text = string.Empty,
                Position = new Point(10, 440),
                Size = new Vector2(100, 30),
                font = defaultFont,
                backgroundColor = Color.White,
                foregroundColor = Color.Black,
                rotation = 0f,
                Scale = 1
            };

            Label labelTime = new Label()
            {
                Text = string.Empty,
                Position = new Point(5,5),
                Size = new Vector2(100, 30),
                font = defaultFont,
                backgroundColor = Color.White,
                foregroundColor = Color.Black,
                rotation = 0f,
                Scale = 1
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
                ButtonColorReleased = Color.LightGray,
                rotation = 0f,
                Scale = 1
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
                foregroundColor = Color.Black,
                rotation = 0f,
                Scale = 1
            };

            canvas.AddElement("label1", label1);
            canvas.AddElement("label2", label2);
            canvas.AddElement("label3", label3);
            canvas.AddElement("button1", button1);
            canvas.AddElement("labelTime", labelTime);
            canvas.AddElement("inputbox1", inputbox1);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // TODO: Add your update logic here
            inputState = new InputState(Mouse.GetState(), Keyboard.GetState());

            ((Label)canvas.GetElement("label2")).Text = inputState.mouseState.Position.ToString();
            ((Label)canvas.GetElement("labelTime")).Text = gameTime.TotalGameTime.ToString();
            
            canvas.Update(inputState, lastInputState);

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
            spriteBatch.Begin(SpriteSortMode.FrontToBack);
            {
                canvas.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
