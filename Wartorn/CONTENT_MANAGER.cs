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
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;
using Fclp;

namespace Wartorn {
	//singleton to store common data
	public static class CONTENT_MANAGER {
		public static bool IsTutorial = false;
		public static string Language = "en";

		public static Game gameinstance;
		/*
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

		public static Texture2D background_terrain;
		public static Texture2D background_unit;

		public static Texture2D SelectedMapCell_border;
		public static Texture2D SelectedMapCell_capturePoint;
		public static Texture2D SelectedMapCell_defenseStar;
		public static Texture2D SelectedMapCell_HPbar;
		public static Texture2D SelectedMapCell_loadedUnit;
		public static Texture2D SelectedMapCell_unitInfo;

		public static Texture2D buymenu_factory;
		public static Texture2D buymenu_airport_harbor;

		public static Texture2D selectCursor;
		public static Texture2D attackCursor;
		public static Texture2D buyCursor;

		public static List<Texture2D> endgameBG;

		public static Texture2D setuptutorial;
		public static Texture2D gametutorial;
		public static Texture2D mapeditortutorial;

		#region animation sprite sheet
		public static Dictionary<SpriteSheetUnit, AnimatedEntity> animationEntities;
		public static Dictionary<SpriteSheetUnit, Texture2D> animationSheets;
		public static List<Animation> animationTypes;
		#endregion

		public static void LoadContent() {
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

			background_terrain = Content.Load<Texture2D>(@"sprite\GUI\background\background_terrain");
			background_unit = Content.Load<Texture2D>(@"sprite\GUI\background\background_unit");

			SelectedMapCell_border = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_Border");
			SelectedMapCell_capturePoint = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_Cap_Point");
			SelectedMapCell_defenseStar = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_Def_Star");
			SelectedMapCell_HPbar = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_HP_Bar");
			SelectedMapCell_loadedUnit = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_Loaded");
			SelectedMapCell_unitInfo = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_Unit_info");

			buymenu_factory = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_Buy_F");
			buymenu_airport_harbor = Content.Load<Texture2D>(@"sprite\GUI\selected_cell_info\SC_Buy_A_H");

			selectCursor = Content.Load<Texture2D>(@"sprite\Cursor\Select_Cursor");
			attackCursor = Content.Load<Texture2D>(@"sprite\Cursor\Attack_Cursor");
			buyCursor = Content.Load<Texture2D>(@"sprite\Cursor\Buy_Cursor");

			endgameBG = new List<Texture2D>();
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background1"));
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background2"));
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background3"));
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background4"));
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background5"));
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background6"));
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background7"));
			endgameBG.Add(Content.Load<Texture2D>(@"sprite\GUI\endgame\Background8"));

			setuptutorial = Content.Load<Texture2D>(@"sprite\tutorial\setupscreentutorial_" + Language);
			gametutorial = Content.Load<Texture2D>(@"sprite\tutorial\gamescreentutorial_" + Language);
			mapeditortutorial = Content.Load<Texture2D>(@"sprite\tutorial\mapeditortutorial_" + Language);

			LoadAnimationContent();

			LoadSound();
		}

		private static void LoadAnimationContent() {
			//string delimit = "Yellow";
			CONTENT_MANAGER.animationEntities = new Dictionary<SpriteSheetUnit, AnimatedEntity>();
			CONTENT_MANAGER.animationSheets = new Dictionary<SpriteSheetUnit, Texture2D>();
			CONTENT_MANAGER.animationTypes = new List<Animation>();

			//list of unit type
			var UnitTypes = new List<SpriteSheetUnit>((IEnumerable<SpriteSheetUnit>)Enum.GetValues(typeof(SpriteSheetUnit)));
			//Artillery
			//load animation sprite sheet for each unit type
			foreach (SpriteSheetUnit unittype in UnitTypes) {
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
			for (int i = 0; i < 4; i++) {
				idle.AddKeyFrame(i * 48, 0, 48, 48);
			}

			Animation right = new Animation("right", true, 4, string.Empty);
			for (int i = 0; i < 4; i++) {
				right.AddKeyFrame(i * 48, 48, 48, 48);
			}

			Animation up = new Animation("up", true, 4, string.Empty);
			for (int i = 0; i < 4; i++) {
				up.AddKeyFrame(i * 48, 96, 48, 48);
			}

			Animation down = new Animation("down", true, 4, string.Empty);
			for (int i = 0; i < 4; i++) {
				down.AddKeyFrame(i * 48, 144, 48, 48);
			}

			Animation done = new Animation("done", true, 1, string.Empty);
			done.AddKeyFrame(0, 192, 48, 48);

			//animation frame for "HIGH" unit
			Animation idleAir = new Animation("idle", true, 4, string.Empty);
			for (int i = 0; i < 4; i++) {
				idleAir.AddKeyFrame(i * 64, 0, 64, 64);
			}

			Animation rightAir = new Animation("right", true, 4, string.Empty);
			for (int i = 0; i < 4; i++) {
				rightAir.AddKeyFrame(i * 64, 64, 64, 64);
			}

			Animation upAir = new Animation("up", true, 4, string.Empty);
			for (int i = 0; i < 4; i++) {
				upAir.AddKeyFrame(i * 64, 128, 64, 64);
			}

			Animation downAir = new Animation("down", true, 4, string.Empty);
			for (int i = 0; i < 4; i++) {
				downAir.AddKeyFrame(i * 64, 192, 64, 64);
			}

			Animation doneAir = new Animation("done", true, 1, string.Empty);
			doneAir.AddKeyFrame(0, 256, 64, 64);

			//animation frame for copter unit
			Animation idleCopter = new Animation("idle", true, 3, string.Empty);
			for (int i = 0; i < 3; i++) {
				idleCopter.AddKeyFrame(i * 64, 0, 64, 64);
			}

			CONTENT_MANAGER.animationTypes.Add(idle);
			CONTENT_MANAGER.animationTypes.Add(right);
			CONTENT_MANAGER.animationTypes.Add(up);
			CONTENT_MANAGER.animationTypes.Add(down);
			CONTENT_MANAGER.animationTypes.Add(done);

			foreach (SpriteSheetUnit unittype in UnitTypes) {
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
				  || unittypestring.Contains("Bomber")) {
					//we enter "HIGH" mode
					//first we set the origin to "HIGH"
					//because we are drawing from topleft and the sprite size is 64x64
					temp.Origin = new Vector2(8, 16);

					//then we load the "HIGH" animation in
					if (unittypestring.Contains("Copter")) {
						temp.AddAnimation(idleCopter, rightAir, upAir, downAir, doneAir);
					}
					else {
						temp.AddAnimation(idleAir, rightAir, upAir, downAir, doneAir);
					}
				}
				else {
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

		private static void LoadSound() {
			menu_select = Content.Load<SoundEffect>(@"sound\sfx\menu_select");
			yes1 = Content.Load<SoundEffect>(@"sound\yessir\yes1");
			moving_out = Content.Load<SoundEffect>(@"sound\speech\moving_out");
		}

		#endregion
		#endregion
		*/

		#region resources
		public static ContentManager Content;

		public static SpriteBatch spriteBatch;

		public static Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();

		public static Dictionary<string, Texture2D> Sprites = new Dictionary<string, Texture2D>();
		//public static Dictionary<string, SpriteSheetMap> SpriteSheets = new Dictionary<string, SpriteSheetMap>();

		public static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();

		public static Dictionary<string, AnimatedEntity> animationEntities;
		public static Dictionary<string, Texture2D> animationSheets;
		public static List<Animation> animationTypes;

		/// <summary>
		/// load font, all font is put inside folder font
		/// </summary>
		/// <param name="fontList">fonts to load</param>
		public static void LoadFont(params string[] fontList) {
			foreach (var font in fontList) {
				Fonts.Add(font, Content.Load<SpriteFont>(string.Format(@"font\{0}", font)));
			}
		}

		/// <summary>
		/// load sprite, all sprite is put inside folder sprite
		/// </summary>
		/// <param name="sprites">sprite to load</param>
		public static void LoadSprites(params string[] sprites) {
			foreach (var sprite in sprites) {
				Sprites.Add(sprite, Content.Load<Texture2D>(string.Format(@"sprite\{0}", sprite)));
			}
		}

		/// <summary>
		/// load in a spritesheet with given <paramref name="width"/> and <paramref name="height"/> of a sprite<para/>
		/// all spritesheet is put inside folder sprite
		/// </summary>
		/// <param name="spriteSheet">spritesheet to load</param>
		/// <param name="width">width of a sprite</param>
		/// <param name="height">height of a sprite</param>
		//public static SpriteSheetMap LoadSpriteSheet(string spriteSheet, int width = 0, int height = 0) {
		//	var spm = new SpriteSheetMap(spriteSheet, Content.Load<Texture2D>(string.Format(@"sprite\{0}", spriteSheet)), width, height);
		//	SpriteSheets.Add(spriteSheet, spm);
		//	return spm;
		//}

		public static void LoadAnimationContent(params string[] animationList) {
			//string delimit = "Yellow";
			animationEntities = new Dictionary<string, AnimatedEntity>();
			animationSheets = new Dictionary<string, Texture2D>();
			animationTypes = new List<Animation>();
		}


		public static void LoadSound(params string[] soundlist) {
			//menu_select = Content.Load<SoundEffect>(@"sound\sfx\menu_select");
			foreach (var sound in soundlist) {
				Sounds.Add(sound, Content.Load<SoundEffect>(string.Format(@"sound\{0}", sound)));
			}
		}

		#endregion


		public static void BeginSpriteBatch() {
			spriteBatch.Begin(SpriteSortMode.FrontToBack);
		}
		public static void BeginSpriteBatchWithCamera(Camera camera) {
			spriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: camera.TransformMatrix);
		}

		public static void EndSpriteBatch() {
			spriteBatch.End();
		}

		public static string MapName { get; private set; } = null;

		public static void ParseArguments(string[] args) {
			var p = new FluentCommandLineParser();
			p.Setup<string>('m')
				.Callback(x => MapName = x);
			p.Parse(args);
		}

		public static string LocalRootPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);

		public static RasterizerState antialiasing = new RasterizerState { MultiSampleAntiAlias = true };

		private static InputState _inputState;
		public static InputState currentInputState {
			get {
				return _inputState;
			}
			set {
				lastInputState = _inputState;
				_inputState = value;
			}
		}
		public static InputState lastInputState { get; private set; }

		//todo make internal messagebox
		public static void ShowMessageBox(object message) {

		}

		//todo make internal messagebox
		public static string ShowPromptBox(object message) {
			return string.Empty;
		}

		//todo make internal file browser
		public static string ShowFileOpenDialog(object message) {
			return string.Empty;
		}

		public static string ShowDropdownBox(object message) {
			return string.Empty;
		}

		public static void Log(object message) {

		}

		//public static event EventHandler<MessageEventArgs> messagebox;
		//public static event EventHandler<MessageEventArgs> fileopendialog;
		//public static event EventHandler<MessageEventArgs> promptbox;
		//public static event EventHandler<MessageEventArgs> dropdownbox;
		//public static event EventHandler<MessageEventArgs> getclipboard;
		//public static event EventHandler<MessageEventArgs> setclipboard;

		//public static string ShowMessageBox(string message) {
		//	MessageEventArgs e = new MessageEventArgs(message);
		//	messagebox?.Invoke(null, e);
		//	return e.message;
		//}

		//public static string ShowMessageBox(object message) {
		//	MessageEventArgs e = new MessageEventArgs(message.ToString());
		//	messagebox?.Invoke(null, e);
		//	return e.message;
		//}

		//public static string ShowFileOpenDialog(string rootpath) {
		//	MessageEventArgs e = new MessageEventArgs(rootpath);
		//	fileopendialog?.Invoke(null, e);
		//	return e.message;
		//}

		//public static string ShowPromptBox(string prompt) {
		//	MessageEventArgs e = new MessageEventArgs(prompt);
		//	promptbox?.Invoke(null, e);
		//	return e.message;
		//}

		//public static string ShowDropdownBox(string prompt) {
		//	MessageEventArgs e = new MessageEventArgs(prompt);
		//	dropdownbox?.Invoke(null, e);
		//	return e.message;
		//}

		//public static string GetClipboard() {
		//	MessageEventArgs e = new MessageEventArgs();
		//	getclipboard?.Invoke(null, e);
		//	return e.message;
		//}

		//public static void SetClipboard(string text) {
		//	MessageEventArgs e = new MessageEventArgs(text);
		//	setclipboard?.Invoke(null, e);
		//}

		public static void ShowFPS(GameTime gameTime) {
			int frameRate = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
			spriteBatch.DrawString(Fonts["defaultfont"], frameRate.ToString(), new Vector2(0, 0), Color.Black);
		}
	}
}
