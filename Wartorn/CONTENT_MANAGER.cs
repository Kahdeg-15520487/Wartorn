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

namespace Wartorn
{
    //singleton to store common data
    public static class CONTENT_MANAGER
    {
        public static Game gameinstance;

        #region resources
        public static ContentManager Content;

        #region sprite
        public static SpriteBatch spriteBatch;

        public static SpriteFont defaultfont;
        public static SpriteFont arcadefont;
        public static SpriteFont hackfont;

        public static Texture2D spriteSheet;
        public static Texture2D UIspriteSheet;
        public static Texture2D buildingSpriteSheet;
        public static Texture2D blank8x8;
        public static Texture2D attackOverlay;
        public static Texture2D moveOverlay;
        public static Texture2D unitSpriteSheet;
        public static Texture2D directionarrow;
        public static Texture2D commandspritesheet;

        public static Texture2D selectCursor;
        public static Texture2D attackCursor;

        public static Texture2D caigidoSpriteSheet;

        #region animation sprite sheet
        public static Dictionary<SpriteSheetUnit, AnimatedEntity> animationEntities;
        public static Dictionary<SpriteSheetUnit, Texture2D> animationSheets;
        public static List<Animation> animationTypes;
        #endregion

        public static void LoadContent()
        {
            defaultfont = Content.Load<SpriteFont>("defaultfont");
            arcadefont = Content.Load<SpriteFont>(@"sprite\GUI\menufont");
            hackfont = Content.Load<SpriteFont>(@"hackfont");

            spriteSheet = Content.Load<Texture2D>(@"sprite\terrain");
            buildingSpriteSheet = Content.Load<Texture2D>(@"sprite\building");
            UIspriteSheet = Content.Load<Texture2D>(@"sprite\ui_sprite_sheet");
            unitSpriteSheet = Content.Load<Texture2D>(@"sprite\unit");
            blank8x8 = Content.Load<Texture2D>(@"sprite\blank8x8");
            attackOverlay = Content.Load<Texture2D>(@"sprite\AttackOverlay");
            moveOverlay = Content.Load<Texture2D>(@"sprite\MoveOverlay");
            directionarrow = Content.Load<Texture2D>(@"sprite\directionarrow");
            commandspritesheet = Content.Load<Texture2D>(@"sprite\GUI\commandspritesheet");

            selectCursor = Content.Load<Texture2D>(@"sprite\Cursor\Select_Cursor");
            attackCursor = Content.Load<Texture2D>(@"sprite\Cursor\Attack_Cursor");

            caigidoSpriteSheet = Content.Load<Texture2D>(@"sprite\building");

            LoadAnimationContent();

            LoadSound();
        }

        private static void LoadAnimationContent()
        {
            //string delimit = "Yellow";
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
                //if (paths[0].CompareTo(delimit) == 0)
                {
                    //  break;
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
                //if (unittypestring.Contains(delimit))
                {
                    //break;
                }

                temp.LoadContent(CONTENT_MANAGER.animationSheets[unittype]);

                if (unittypestring.Contains("TransportCopter")
                  || unittypestring.Contains("BattleCopter")
                  || unittypestring.Contains("Fighter")
                  || unittypestring.Contains("Bomber"))
                {
                    //we enter "HIGH" mode
                    //first we set the origin to "HIGH"
                    //because we are drawing from topleft and the sprite size is 64x64
                    temp.Origin = new Vector2(8, 16);

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

        #endregion

        #region sound

        public static SoundEffect menu_select;
        public static SoundEffect yes1;
        public static SoundEffect moving_out;

        private static void LoadSound()
        {
            menu_select = Content.Load<SoundEffect>(@"sound\sfx\menu_select");
            yes1 = Content.Load<SoundEffect>(@"sound\yessir\yes1");
            moving_out = Content.Load<SoundEffect>(@"sound\speech\moving_out");
        }

        #endregion
        #endregion

        public static string LocalRootPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);

        public static RasterizerState antialiasing = new RasterizerState { MultiSampleAntiAlias = true };

        private static InputState _inputState;
        public static InputState inputState
        {
            get
            {
                return _inputState;
            }
            set
            {
                lastInputState = _inputState;
                _inputState = value;
            }
        }
        public static InputState lastInputState { get; private set; }
        
        public static event EventHandler<MessageEventArgs> messagebox;
        public static event EventHandler<MessageEventArgs> fileopendialog;
        public static event EventHandler<MessageEventArgs> promptbox;
        public static event EventHandler<MessageEventArgs> dropdownbox;
        public static event EventHandler<MessageEventArgs> getclipboard;
        public static event EventHandler<MessageEventArgs> setclipboard;

        public static string ShowMessageBox(string message)
        {
            MessageEventArgs e = new MessageEventArgs(message);
            messagebox?.Invoke(null, e);
            return e.message;
        }

        public static string ShowMessageBox(object message)
        {
            MessageEventArgs e = new MessageEventArgs(message.ToString());
            messagebox?.Invoke(null, e);
            return e.message;
        }

        public static string ShowFileOpenDialog(string rootpath)
        {
            MessageEventArgs e = new MessageEventArgs(rootpath);
            fileopendialog?.Invoke(null, e);
            return e.message;
        }

        public static string ShowPromptBox(string prompt)
        {
            MessageEventArgs e = new MessageEventArgs(prompt);
            promptbox?.Invoke(null, e);
            return e.message;
        }

        public static string ShowDropdownBox(string prompt)
        {
            MessageEventArgs e = new MessageEventArgs(prompt);
            dropdownbox?.Invoke(null, e);
            return e.message;
        }

        public static string GetClipboard()
        {
            MessageEventArgs e = new MessageEventArgs();
            getclipboard?.Invoke(null, e);
            return e.message;
        }

        public static void SetClipboard(string text)
        {
            MessageEventArgs e = new MessageEventArgs(text);
            setclipboard?.Invoke(null, e);
        }

        public static void ShowFPS(GameTime gameTime)
        {
            int frameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            spriteBatch.DrawString(defaultfont, frameRate.ToString(), new Vector2(0, 0), Color.Black);
        }
    }
}
