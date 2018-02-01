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
		List<Label> maplist;
		PictureBox pictureBox_mappreview;

		public FileBrowsingScreen(GraphicsDevice device) : base(device, "FileBrowsingScreen") { }

		public override bool Init() {
			InitUI();

			return base.Init();
		}

		private void InitUI() {
			canvas = new Canvas();

			pictureBox_mappreview = new PictureBox(new Point(0, 0), null, Vector2.Zero, depth: LayerDepth.GuiUpper);

			InitMapList();

			canvas.AddElement("pictureBox_mappreview", pictureBox_mappreview);
		}

		private void InitMapList() {
			var maps = Directory.GetFiles(Path.Combine(CONTENT_MANAGER.LocalRootPath, "map"), "*.map");

			foreach (var m in maps) {
				//Label label = new Label(``)
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
