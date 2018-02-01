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
	public class ListView : UIObject {
		public class ListViewItem : Label {
			public override Color BackgroundColor {
				get { return base.BackgroundColor; }
				set {
					base.BackgroundColor = value;
					//background.color = BackgroundColor;
				}
			}

			public ListViewItem(string text, Point position, Vector2 size, Vector2 origin, SpriteFont font, float scale = 1f) : base(text, position, size, font, scale) {
				Origin = origin;
			}

			public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
				base.Draw(spriteBatch, gameTime);
			}
		}

		List<ListViewItem> listViewItems;
		ListViewItem selectedItem = null;
		Point lastPosition = Point.Zero;
		Vector2 listViewItemSize = Vector2.Zero;
		public Point Offset { get; set; }
		public Color SelectedItemColor { get; set; } = Color.LightGray;
		public Color UnselectedItemColor { get; set; } = Color.White;

		public ListViewItem this[int index] {
			get { return listViewItems[index]; }
		}

		public ListView(Point position, Vector2 size, Point offset, Vector2 lviSize, float scale = 1f) {
			Position = position;
			Size = size;
			Offset = offset;
			listViewItemSize = lviSize;
			Scale = scale;
			listViewItems = new List<ListViewItem>();
		}

		public void AddItem(string data) {
			Point position = new Point(lastPosition.X, lastPosition.Y + Offset.Y);
			lastPosition = position;
			ListViewItem lvi = new ListViewItem(data, position, listViewItemSize, new Vector2(-2, -2), CONTENT_MANAGER.Fonts["defaultFont"]) {
				Container = this,
				MetaData = listViewItems.Count,
			};
			lvi.MouseClick += ItemSelectHandler;
			listViewItems.Add(lvi);
		}

		public void ClearItems() {
			listViewItems.Clear();
		}

		public string GetSelectedItem() {
			return selectedItem == null ? string.Empty : selectedItem.Text;
		}

		private void ItemSelectHandler(object sender, UIEventArgs e) {
			int index = (int)((UIObject)sender).MetaData;
			OnItemSelected(sender, listViewItems[index]);
		}

		public override void Update(GameTime gameTime, InputState currentInputState, InputState lastInputState) {
			base.Update(gameTime, currentInputState, lastInputState);

			foreach (var lvi in listViewItems) {
				lvi.Update(gameTime, currentInputState, lastInputState);
			}
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			foreach (var lvi in listViewItems) {
				lvi.Draw(spriteBatch, gameTime);
			}
		}

		public event EventHandler<ListViewItem> ItemSelected;
		private void OnItemSelected(object sender, ListViewItem lvi) {
			if (selectedItem != null) {
				selectedItem.BackgroundColor = UnselectedItemColor;
			}
			selectedItem = lvi;
			selectedItem.BackgroundColor = SelectedItemColor;
			ItemSelected?.Invoke(sender, lvi);
		}
	}
}