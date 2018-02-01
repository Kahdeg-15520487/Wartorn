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

using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;
using Wartorn.SpriteRectangle;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Content;

namespace Wartorn.Screens.MainGameScreen {
	class SetupScreen : Screen {
		SessionData sessiondata;
		Canvas canvas;

		Map map = null;

		MiniMapGenerator minimapgen;
		Texture2D minimap;

		string mapdata;

		public SetupScreen(GraphicsDevice device) : base(device, "SetupScreen") {

		}

		public override bool Init() {
			canvas = new Canvas();
			sessiondata = new SessionData();

			InitUI();

			minimapgen = new MiniMapGenerator(_device, CONTENT_MANAGER.spriteBatch);

			return base.Init();
		}

		private void InitUI() {
			//declare ui elements
			Label label_playerinfo = new Label("kahdeg", new Point(10, 20), new Vector2(80, 30), CONTENT_MANAGER.Fonts["arcadefont"]);

			PictureBox picturebox_tutorial = new PictureBox(CONTENT_MANAGER.Sprites[@"tutorial\setupscreentutorial_en"], Point.Zero, null, null) {
				IsVisible = CONTENT_MANAGER.IsTutorial
			};

			Button button_selectmap = new Button(CONTENT_MANAGER.Sprites["UIspriteSheet"], UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Open), new Point(650, 20), 0.5f);
			Button button_exit = new Button(CONTENT_MANAGER.Sprites["UIspriteSheet"], UISpriteSheetSourceRectangle.GetSpriteRectangle(SpriteSheetUI.Exit), new Point(5, 5), 0.5f);
			Button button_start = new Button("Start", new Point(100, 50), new Vector2(40, 20), CONTENT_MANAGER.Fonts["arcadefont"]);

			//bind event
			button_selectmap.MouseClick += (sender, e) => {

				string path = CONTENT_MANAGER.ShowFileOpenDialog(Path.Combine(CONTENT_MANAGER.LocalRootPath, "map"));
				LoadMap(path);
				minimap = minimapgen.GenerateMapTexture(map);
			};
			button_exit.MouseClick += (sender, e) => {
				SCREEN_MANAGER.goto_screen("MainMenuScreen");
			};
			button_start.MouseClick += (sender, e) => {
				SetUpSessionDataAndLaunchMainGame();
			};

			//add to canvas
			canvas.AddElement("label_playerinfo", label_playerinfo);
			canvas.AddElement("button_selectmap", button_selectmap);
			canvas.AddElement("button_exit", button_exit);
			canvas.AddElement("button_start", button_start);
			canvas.AddElement("picturebox_tutorial", picturebox_tutorial);
		}

		public void LoadMap(string path) {
			string content = string.Empty;
			try {
				content = File.ReadAllText(path);
			}
			catch (Exception er) {
				Utility.HelperFunction.Log(er);
			}

			if (!string.IsNullOrEmpty(content)) {
				mapdata = content;
				var temp = Storage.MapData.LoadMap(content);
				if (temp != null) {
					map = new Map(temp);
				}
			}
		}

		public void SetUpSessionDataAndLaunchMainGame() {
			if (map == null) {
				return;
			}
			sessiondata = new SessionData {
				map = new Map(MapData.LoadMap(mapdata)),
				gameMode = GameMode.campaign,
				playerInfos = new PlayerInfo[2] { new PlayerInfo(0, Owner.Red), new PlayerInfo(1, Owner.Blue) }
			};

			foreach (var p in map.GetOwnedBuilding(Owner.Red)) {
				if (map[p].terrain == TerrainType.HQ && map[p].owner == Owner.Red) {
					sessiondata.playerInfos[0].HQlocation = p;
					break;
				}
			}

			foreach (var p in map.GetOwnedBuilding(Owner.Blue)) {
				if (map[p].terrain == TerrainType.HQ && map[p].owner == Owner.Blue) {
					sessiondata.playerInfos[1].HQlocation = p;
					break;
				}
			}

				((GameScreen)SCREEN_MANAGER.get_screen("GameScreen")).InitSession(sessiondata);
			SCREEN_MANAGER.goto_screen("GameScreen");
		}

		public override void Shutdown() {
			sessiondata.playerInfos = null;
			sessiondata.map = null;
			map = null;
			minimap?.Dispose();
			minimap = null;
		}

		public override void Update(GameTime gameTime) {
			canvas.Update(gameTime, CONTENT_MANAGER.currentInputState, CONTENT_MANAGER.lastInputState);
		}

		public override void Draw(GameTime gameTime) {
			canvas.Draw(CONTENT_MANAGER.spriteBatch, gameTime);
			if (minimap != null) {
				CONTENT_MANAGER.spriteBatch.Draw(minimap, new Vector2(100, 100), Color.White);
			}
		}
	}


}
