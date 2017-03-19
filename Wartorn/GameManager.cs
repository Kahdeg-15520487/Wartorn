﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;
using Wartorn.UIClass;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.IO;

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

        string serializeui(UIObject ui)
        {
            StringBuilder output = new StringBuilder();

            output.Append(ui.GetType());
            output.Append("|");
            output.Append(JsonConvert.SerializeObject(ui, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            output.Append("|");
            return output.ToString();
        }

        void InitializeUI()
        {
            StringBuilder jsonoutput = new StringBuilder();
            Label label1 = new Label()
            {
                Text = "1",
                Position = new Point(50,50),
                Size = new Vector2(100,50),
                font = defaultFont,
                foregroundColor = Color.Black,
                Scale = 2
            };
            jsonoutput.Append(serializeui(label1));

            Label label2 = new Label()
            {
                Text = "",
                Position = new Point(10, 400),
                Size = new Vector2(100, 30),
                font = defaultFont,
                foregroundColor = Color.White
            };
            jsonoutput.Append(serializeui(label2));

            Label label3 = new Label()
            {
                Text = string.Empty,
                Position = new Point(10, 440),
                Size = new Vector2(100, 30),
                font = defaultFont,
                foregroundColor = Color.White
            };
            jsonoutput.Append(serializeui(label3));

            Label labelTime = new Label()
            {
                Text = string.Empty,
                Position = new Point(5,5),
                Size = new Vector2(100, 30),
                font = defaultFont,
                foregroundColor = Color.White
            };
            jsonoutput.Append(serializeui(labelTime));

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
            jsonoutput.Append(serializeui(button1));

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
            jsonoutput.Append(serializeui(inputbox1));

            Console testconsole = new Console()
            {
                Position = new Point(400,200),
                Size = new Vector2(100,200),
                font = defaultFont,
                backgroundColor = Color.LightGray,
                foregroundColor = Color.White
            };
            testconsole.inputbox.font = defaultFont;

            File.WriteAllText("ui.uis", jsonoutput.ToString());

            canvas.AddElement("label1", label1);
            canvas.AddElement("label2", label2);
            canvas.AddElement("label3", label3);
            canvas.AddElement("button1", button1);
            canvas.AddElement("labelTime", labelTime);
            canvas.AddElement("inputbox1", inputbox1);
            canvas.AddElement("testconsole", testconsole);
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

            ((Label)canvas.GetElement("labelTime")).Text = lala();

            canvas.Update(inputState, lastInputState);

            lastInputState = inputState;
            base.Update(gameTime);
        }

        string lala()
        {
            var keyInput = inputState.keyboardState.GetPressedKeys();
            var lastKeyInput = lastInputState.keyboardState.GetPressedKeys();
            foreach (var key in keyInput)
            {
                if (!lastKeyInput.Contains(key))
                {
                    return key.ToString();
                }
            }
            return "";
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
