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

using Wartorn.CustomJsonConverter;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wartorn.UIClass {
	public class Canvas : UIObject {
		Dictionary<string, UIObject> UIelements;

		public UIObject this[string uiname] {
			get {
				return UIelements[uiname];
			}
		}

		public List<string> UInames {
			get {
				return UIelements.Keys.ToList();
			}
		}

		public Canvas() {
			InitUIelements();
		}

		public Canvas(Point position, Vector2 size) {
			Position = position;
			Size = size;
		}

		private void InitUIelements() {
			UIelements = new Dictionary<string, UIObject>();
		}

		public bool AddElement(string uiName, UIObject element) {
			if (uiName == null || !UIelements.ContainsKey(uiName)) {
				UIelements.Add(uiName ?? nameof(element), element);
				element.Container = this;
				return true;
			}
			else {
				//log stuff
				CONTENT_MANAGER.Log("Duplicate UI element : " + nameof(element));
				return false;
			}
		}

		public UIObject GetElement(string uiName) {
			if (UIelements.ContainsKey(uiName)) {
				return UIelements[uiName];
			}
			else {
				//log stuff
				CONTENT_MANAGER.Log("UI element not found : " + uiName);
				return null;
			}
		}

		public T GetElementAs<T>(string uiName) {
			if (UIelements.ContainsKey(uiName)) {
				return (T)(object)UIelements[uiName];
			}
			else {
				//log stuff
				CONTENT_MANAGER.Log("UI element not found : " + uiName);
				return default(T);
			}
		}

		public IEnumerable<UIObject> GetElements() {
			foreach (var element in UIelements.Values) {
				yield return element;
			}
		}

		public void LoadContent() {
			//UIspritesheet = content.Load<Texture2D>("sprite\\UIspritesheet");
		}

		public override void Update(GameTime gameTime, InputState currentInputState, InputState lastInputState) {
			base.Update(gameTime, currentInputState, lastInputState);

			if (!IsVisible)
				return;

			foreach (var element in UIelements.Values) {
				if (element.IsVisible) {
					element.Update(gameTime, currentInputState, lastInputState);
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			if (!IsVisible)
				return;

			foreach (var element in UIelements.Values) {
				if (element.IsVisible) {
					element.Draw(spriteBatch, gameTime);
				}
			}
		}
	}
}