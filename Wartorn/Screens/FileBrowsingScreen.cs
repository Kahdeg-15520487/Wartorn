//using System;
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

using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wartorn.PathFinding.Dijkstras;
using Wartorn.PathFinding;

namespace Wartorn.Screens {
	class FileBrowsingScreen : Screen {
		Canvas canvas;
		List<Button> maplist;
		PictureBox pictureBox_mappreview;
		Texture2D background;
		MiniMapGenerator miniMapGenerator;
		Texture2D minimap;

		public FileBrowsingScreen(GraphicsDevice device) : base(device, "FileBrowsingScreen") { }

		public override bool Init() {
			miniMapGenerator = new MiniMapGenerator(_device, CONTENT_MANAGER.spriteBatch);
			InitUI();

			return base.Init();
		}

		private void InitUI() {
			canvas = new Canvas();

			pictureBox_mappreview = new PictureBox(CONTENT_MANAGER.Sprites["blank8x8"], new Point(200, 100), null, Vector2.Zero, depth: LayerDepth.GuiBackground);

			background = CONTENT_MANAGER.Sprites.Where(x => x.Key.Contains("Startscreen")).PickRandom().Value;

			InitMapList();

			Button button_play = new Button("Play", new Point(600, 10), new Vector2(60, 30), CONTENT_MANAGER.Fonts["defaultfont"]);
			button_play.MouseClick += (o, e) => {
				var setupscreen = ((MainGameScreen.SetupScreen)SCREEN_MANAGER.get_screen("SetupScreen"));
				setupscreen.LoadMap(Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", CONTENT_MANAGER.MapName));
				setupscreen.SetUpSessionDataAndLaunchMainGame();
			};

			canvas.AddElement("pictureBox_mappreview", pictureBox_mappreview);
			canvas.AddElement("button_play", button_play);
		}

		private void InitMapList() {
			var maps = Directory.GetFiles(Path.Combine(CONTENT_MANAGER.LocalRootPath, "map"), "*.map");
			var y = 10;
			maplist = new List<Button>();
			foreach (var m in maps) {
				Button bt = new Button(Path.GetFileNameWithoutExtension(m), new Point(10, y), new Vector2(120, 30), CONTENT_MANAGER.Fonts["defaultfont"]) {
					Origin = new Vector2(10, 0),
					ForegroundColor = Color.Black
				};

				bt.MouseClick += (o, e) => {
					CONTENT_MANAGER.MapName = bt.Text + ".map";
					var tempmap = MapData.LoadMap(File.ReadAllText(Path.Combine(CONTENT_MANAGER.LocalRootPath, "map", bt.Text + ".map")));
					minimap = miniMapGenerator.GenerateMapTexture(tempmap);
					pictureBox_mappreview.Texture2D = minimap;
				};

				y += 35;
				maplist.Add(bt);
			}

			foreach (var m in maplist) {
				canvas.AddElement(m.Text, m);
			}
		}

		public override void Shutdown() {
			base.Shutdown();
		}

		public override void Update(GameTime gameTime) {
			canvas.Update(gameTime, CONTENT_MANAGER.currentInputState, CONTENT_MANAGER.lastInputState);
		}

		public override void Draw(GameTime gameTime) {
			canvas.Draw(CONTENT_MANAGER.spriteBatch, gameTime);
		}
	}
}
