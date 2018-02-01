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
	public class Grid : UIObject {
		public class GridCell : UIObject {
			public string Key { get; private set; } = null;
			public UIObject Element { get; private set; }
			public GridCell(Point position, Vector2 size, Grid container) {
				this.Container = container;
				this.Position = position;
				this.Size = size;
			}

			public bool AddElement(string uiName, UIObject element) {
				Key = uiName ?? nameof(element);
				Element = element;
				Element.Container = this;
				return true;
			}

			public override void Update(GameTime gameTime, InputState currentInputState, InputState lastInputState) {
				Element?.Update(gameTime, currentInputState, lastInputState);
			}

			public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
				Element?.Draw(spriteBatch, gameTime);
			}
		}

		private GridCell[,] _cells;

		public int CollumnCount { get; private set; }
		public int RowCount { get; private set; }
		public int CollumnWidth { get; private set; }
		public int RowHeight { get; private set; }
		public int CollumnSpacing { get; private set; }
		public int RowSpacing { get; private set; }

		public Grid(Point position, int collumnCount = 1, int rowCount = 1, int collumnWidth = 100, int rowHeight = 100, int collumnSpacing = 10, int rowSpacing = 10) {
			_cells = new GridCell[collumnCount, rowCount];
			CollumnCount = collumnCount;
			RowCount = rowCount;
			CollumnWidth = collumnWidth;
			RowHeight = rowHeight;
			CollumnSpacing = collumnSpacing;
			RowSpacing = rowSpacing;
			this.Position = position;
			this.Size = new Vector2(collumnCount * (collumnWidth + collumnSpacing) - collumnSpacing, rowCount * (rowHeight + rowSpacing) - rowSpacing);
			for (int c = 0; c < collumnCount; c++) {
				for (int r = 0; r < rowCount; r++) {
					GridCell temp = new GridCell(new Point(c * (collumnWidth + collumnSpacing), r * (rowHeight + rowSpacing)), new Vector2(collumnWidth, rowHeight), this);
					_cells[c, r] = temp;
				}
			}
		}

		private bool CheckGridCellContainKey(string key) {
			foreach (var element in GridCellList()) {
				if (element.Key != null && element.Key.CompareTo(key) == 0) {
					return true;
				}
			}
			return false;
		}

		public IEnumerable<GridCell> GridCellList() {
			for (int c = 0; c < CollumnCount; c++) {
				for (int r = 0; r < RowCount; r++) {
					yield return _cells[c, r];
				}
			}
		}

		public GridCell this[int collumn, int row] {
			get { return _cells[collumn, row]; }
		}

		public bool AddElement(string uiName, UIObject element, int collumn, int row) {
			bool throwFlag = false;
			if (collumn >= CollumnCount) {
				CONTENT_MANAGER.Log(string.Format("Collumn overflow : {0} at {1}", uiName ?? nameof(element), collumn));
				throwFlag = true;
			}
			if (row >= RowCount) {
				CONTENT_MANAGER.Log(string.Format("Row overflow : {0} at {1}", uiName ?? nameof(element), collumn));
				throwFlag = true;
			}
			if (throwFlag) throw new ArgumentOutOfRangeException(string.Format("c: {0};r: {1}", collumn, row));

			if (uiName == null || !CheckGridCellContainKey(uiName)) {
				_cells[collumn, row].AddElement(uiName, element);
				return true;
			}
			else {
				//log stuff
				CONTENT_MANAGER.Log("Duplicate UI element : " + uiName ?? nameof(element));
				return false;
			}
		}

		public override void Update(GameTime gameTime, InputState currentInputState, InputState lastInputState) {
			if (!IsVisible)
				return;

			foreach (var element in GridCellList()) {
				if (element.IsVisible) {
					element.Update(gameTime, currentInputState, lastInputState);
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			if (!IsVisible)
				return;

			foreach (var element in GridCellList()) {
				if (element.IsVisible) {
					element.Draw(spriteBatch, gameTime);
				}
			}
		}
	}
}