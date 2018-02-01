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
	class LanguageChoosingScreen : Screen {
		Canvas canvas;

		public LanguageChoosingScreen(GraphicsDevice device) : base(device, "LanguageChoosingScreen") { }

		public override bool Init() {
			InitUI();

			return base.Init();
		}

		public void InitUI() {
			canvas = new Canvas();
		}

		public override void Shutdown() {
			base.Shutdown();
		}

		public override void Update(GameTime gameTime) {
			canvas.Update(gameTime,CONTENT_MANAGER.currentInputState, CONTENT_MANAGER.lastInputState);
		}

		public override void Draw(GameTime gameTime) {
			canvas.Draw(CONTENT_MANAGER.spriteBatch,gameTime);
		}
	}
}
