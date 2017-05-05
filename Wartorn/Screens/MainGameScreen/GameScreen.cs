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

namespace Wartorn.Screens.MainGameScreen
{
    class GameScreen : Screen
    {
        Session session;

        Canvas canvas;
        Canvas canvas_generalInfo;
        Canvas canvas_action;
        Texture2D guibackground;
        Texture2D minimap;
        MiniMapGenerator minimapgen;

        Camera camera;

        Point selectedMapCell;

        readonly Rectangle minimapbound = new Rectangle(2, 312, 234, 166);

        bool isHideGUI = false;

        public GameScreen(GraphicsDevice device) : base(device, "GameScreen")
        {
            LoadContent();
            minimapgen = new MiniMapGenerator(device, CONTENT_MANAGER.Content, CONTENT_MANAGER.spriteBatch);
        }

        public void InitSession(SessionData sessiondata)
        {
            session = new Session(sessiondata);
            minimap = minimapgen.GenerateMapTexture(session.map);
        }

        private void LoadContent()
        {
            guibackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\placeholdergui");
        }

        public override bool Init()
        {
            camera = new Camera(_device.Viewport);
            canvas = new Canvas();

            InitUI();

            return base.Init();
        }

        private void InitUI()
        {
            //declare ui elements
            canvas_generalInfo = new Canvas();
            InitCanvas_generalInfo();

            canvas_action = new Canvas();
            InitCanvas_action();

            Label label_mousepos = new Label(" ", new Point(0, 0), new Vector2(80, 20), CONTENT_MANAGER.defaultfont);

            //bind event

            //add to canvas
            canvas.AddElement("generalInfo", canvas_generalInfo);
            canvas.AddElement("action", canvas_action);
            canvas.AddElement("label_mousepos", label_mousepos);
        }

        private void InitCanvas_generalInfo()
        {
            //declare ui elements
            Label label_terraintype = new Label(" ", new Point(465, 365), new Vector2(50, 20), CONTENT_MANAGER.arcadefont);

            //bind event

            //add to canvas
            canvas_generalInfo.AddElement("label_terraintype", label_terraintype);
        }

        private void InitCanvas_action()
        {
            //declare ui elements
            //bind event
            //add to canvas
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            var mouseInputState = CONTENT_MANAGER.inputState.mouseState;
            var lastMouseInputState = CONTENT_MANAGER.lastInputState.mouseState;
            var keyboardInputState = CONTENT_MANAGER.inputState.keyboardState;
            var lastKeyboardInputState = CONTENT_MANAGER.lastInputState.keyboardState;

            UpdateAnimation(gameTime);

            //update canvas
            ((Label)canvas["label_mousepos"]).Text = mouseInputState.Position.ToString();
            UpdateCanvas_generalInfo();

            //camera control
            MoveCamera(keyboardInputState, mouseInputState);

            selectedMapCell = Utility.HelperFunction.TranslateMousePosToMapCellPos(mouseInputState.Position, camera, session.map.Width, session.map.Height);
            minimap = minimapgen.GenerateMapTexture(session.map);
        }

        #region Update function helper

        private void UpdateCanvas_generalInfo()
        {
            ((Label)canvas_generalInfo["label_terraintype"]).Text = session.map[selectedMapCell].terrain.ToString();
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
        public override void Draw(GameTime gameTime)
        {
            DrawMap(CONTENT_MANAGER.spriteBatch, gameTime);
            
            //draw the guibackground
            CONTENT_MANAGER.spriteBatch.Draw(guibackground, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiBackground);
            canvas.Draw(CONTENT_MANAGER.spriteBatch);

            //draw canvas_generalInfo
            //MapCell temp = session.map[selectedMapCell];
            

            //draw the minimap
            CONTENT_MANAGER.spriteBatch.Draw(minimap, minimapbound, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, LayerDepth.GuiBackground);
        }

        private void DrawMap(SpriteBatch spriteBatch,GameTime gameTime)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);

            //render the map
            MapRenderer.Render(session.map, spriteBatch, gameTime);

            //draw the cursor
            spriteBatch.Draw(CONTENT_MANAGER.UIspriteSheet, new Vector2(selectedMapCell.X * Constants.MapCellWidth, selectedMapCell.Y * Constants.MapCellHeight), new Rectangle(0, 0, 48, 48), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.GuiUpper);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
