using System;
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
using Wartorn.Drawing;


namespace Wartorn.Screens
{
    enum ButtonSelected
    {
        None,
        Campaign,
        MapEditor,
        Other
    }
    class MainMenuScreen : Screen
    {
        private Canvas canvas;
        private Camera backgroundCamera;

        //sprite
        private Texture2D SelectScreenBackground;
        private Texture2D TitleBackground;
        private Texture2D ModeSelectDark;
        private Texture2D ModeSelectLight;
        private SpriteFont menufont;

        //de phuc vu cho viec ve background quay vong vong
        private int maxXoffset, maxYoffset;
        private ButtonSelected selectedbutton = ButtonSelected.None;

        public MainMenuScreen(GraphicsDevice device) : base(device, "MainMenuScreen")
        {
            LoadContent();
            backgroundCamera = new Camera(_device.Viewport);

            canvas = new Canvas();
            InitUI();
        }

        private void LoadContent()
        {
            // cái này là cái background lúc vào menu
            SelectScreenBackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Select_screen");
            maxXoffset = 1000;
            maxYoffset = 1000;

            //cái này là cái tựa đề tên game, để vẽ đè lên cái background ở trên
            TitleBackground = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Title");

            //cái này là cái hinh dung cho button
            ModeSelectDark = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Mode_select_dark");
            ModeSelectLight = CONTENT_MANAGER.Content.Load<Texture2D>(@"sprite\GUI\Mode_select_light");

            //cái này là cái font sử dụng trong menu
            menufont = CONTENT_MANAGER.Content.Load<SpriteFont>(@"sprite\GUI\menufont");
        }

        private void InitUI()
        {
            //declare ui element

            //khai báo 1 cái label để hiển thị fps
            Label label_fps = new Label(" ", new Point(0, 0), new Vector2(100, 50), CONTENT_MANAGER.defaultfont);
            label_fps.foregroundColor = Color.DarkBlue;
            label_fps.IsVisible = false;

            //TODO make button for main menu
            Button button_Campaign = new Button(ModeSelectDark, null, new Point(100, 275), 2);
            button_Campaign.Depth = LayerDepth.GuiLower;
            Button button_MapEditor = new Button(ModeSelectDark, null, new Point(300, 275), 2);
            button_MapEditor.Depth = LayerDepth.GuiLower;
            Button button_OtherGamemode = new Button(ModeSelectDark, null, new Point(500, 275), 2);
            button_OtherGamemode.Depth = LayerDepth.GuiLower;
            Label label_campaign = new Label("Single" + Environment.NewLine + "Player", new Point(130, 340), null, CONTENT_MANAGER.hackfont, 1f);
            label_campaign.Origin = new Vector2(1, 1);
            Label label_mapeditor = new Label("  Map" + Environment.NewLine + "Editor", new Point(335, 340), null, CONTENT_MANAGER.hackfont, 1f);
            label_mapeditor.Origin = new Vector2(1, 1);
            Label label_othergamemode = new Label(" Multi" + Environment.NewLine + " Player" + Environment.NewLine + "<not yet>", new Point(525, 340), null, CONTENT_MANAGER.hackfont, 1f);
            label_othergamemode.Origin = new Vector2(1, 1);

            //bind action to ui event
            button_Campaign.MouseClick += (sender, e) =>
            {
                if (selectedbutton != ButtonSelected.Campaign)
                {
                    selectedbutton = ButtonSelected.Campaign;
                    button_Campaign.Sprite = ModeSelectLight;
                    button_MapEditor.Sprite = ModeSelectDark;
                    button_OtherGamemode.Sprite = ModeSelectDark;
                }
                else
                {
                    SCREEN_MANAGER.goto_screen("SetupScreen");
                }
            };

            button_MapEditor.MouseClick += (sender, e) =>
            {
                if (selectedbutton != ButtonSelected.MapEditor)
                {
                    selectedbutton = ButtonSelected.MapEditor;
                    button_Campaign.Sprite = ModeSelectDark;
                    button_MapEditor.Sprite = ModeSelectLight;
                    button_OtherGamemode.Sprite = ModeSelectDark;
                }
                else
                {
                    SCREEN_MANAGER.goto_screen("EditorScreen");
                }
            };

            button_OtherGamemode.MouseClick += (sender, e) =>
            {
                if (selectedbutton != ButtonSelected.Other)
                {
                    selectedbutton = ButtonSelected.Other;
                    button_Campaign.Sprite = ModeSelectDark;
                    button_MapEditor.Sprite = ModeSelectDark;
                    button_OtherGamemode.Sprite = ModeSelectLight;
                }
                else
                {
                    //SCREEN_MANAGER.goto_screen("OtherGamemode");
                }
            };

            //add ui element to canvas
            canvas.AddElement("label_fps", label_fps);
            canvas.AddElement("button_Campaign", button_Campaign);
            canvas.AddElement("button_MapEditor", button_MapEditor);
            canvas.AddElement("button_OtherGameMode", button_OtherGamemode);
            canvas.AddElement("label_campaign", label_campaign);
            canvas.AddElement("label_mapeditor", label_mapeditor);
            canvas.AddElement("label_othergamemode", label_othergamemode);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawMenuBackground(CONTENT_MANAGER.spriteBatch);

            CONTENT_MANAGER.spriteBatch.Draw(TitleBackground, new Vector2(0,-20), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.BackGround);

            canvas.Draw(CONTENT_MANAGER.spriteBatch);
            CONTENT_MANAGER.ShowFPS(gameTime);
        }

        private Rectangle offset = new Rectangle(0, 0, 720, 480);
        private Point moveoffset = new Point(1, 1);
        private void DrawMenuBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: backgroundCamera.TransformMatrix);
            {
                spriteBatch.Draw(SelectScreenBackground, new Rectangle(0, 0, 720, 480), offset, Color.White, 0f, Vector2.Zero, SpriteEffects.None, LayerDepth.BackGround);

                //if (Utility.HelperFunction.IsKeyPress(Keys.P))
                {
                    if ((offset.Size.X > maxXoffset || offset.Size.Y > maxYoffset)
                      || (offset.Location.X < 0 || offset.Location.Y < 0))
                    {
                        moveoffset = new Point(-moveoffset.X, -moveoffset.Y);
                    }
                    offset.Location = new Point(offset.Location.X + moveoffset.X, offset.Location.Y + moveoffset.Y);
                    offset.Size = new Point(offset.Size.X + moveoffset.X, offset.Size.Y + moveoffset.Y);
                }
            }
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack);
        }
    }
}
