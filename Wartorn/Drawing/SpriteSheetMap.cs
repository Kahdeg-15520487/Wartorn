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
using Wartorn.CustomJsonConverter;
using Wartorn.Screens;
using Wartorn.Drawing;
using Wartorn.Drawing.Animation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wartorn.Drawing {
	public class SpriteSheetMap {
		public readonly string SpriteSheetName;
		public readonly Texture2D SpriteSheet;
		public List<Rectangle> SpriteRect;
		public Dictionary<string, int> SpriteRectDict;

		public Rectangle this[string index] {
			get { return SpriteRect[SpriteRectDict[index]]; }
		}
		public Rectangle this[int index] {
			get { return SpriteRect[index]; }
		}

		public SpriteSheetMap(string name, Texture2D spritesheet, int width = 0, int height = 0) {
			SpriteSheetName = name;
			SpriteSheet = spritesheet;
			SpriteRect = new List<Rectangle>();
			SpriteRectDict = new Dictionary<string, int>();
			var x = SpriteSheet.Width / width;
			var y = SpriteSheet.Height / height;

			if (width != 0 && height != 0) {
				StringBuilder tt = new StringBuilder();
				tt.AppendFormat("W = {0}; H = {1}", SpriteSheet.Width, SpriteSheet.Height);
				tt.AppendLine();
				tt.AppendFormat("x = {0}; y = {1}", x, y);
				tt.AppendLine();
				for (int h = 0; h < y; h++) {
					for (int w = 0; w < x; w++) {
						AddSpriteRect((h * x + w).ToString(), new Rectangle(w * width, h * height, width, height));
						tt.Append((h * x + w) + "    :    ");
						tt.AppendLine(new Rectangle(w * width, h * height, width, height).ToString());
					}
				}
				System.IO.File.AppendAllText("map.txt", tt.ToString());
			}
		}

		public Rectangle GetSpriteRect(string rectname) {
			if (!SpriteRectDict.ContainsKey(rectname)) {
				throw new KeyNotFoundException(rectname);
			}

			return SpriteRect[SpriteRectDict[rectname]];
		}

		public bool AddSpriteRect(string rectname, Rectangle rect) {
			if (SpriteRectDict.ContainsKey(rectname)) {
				return false;
			}

			SpriteRect.Add(rect);
			SpriteRectDict.Add(rectname, SpriteRect.Count - 1);

			return true;
		}

		public void RenameSpriteRect(string oldname, string newname) {
			int spriteindex = SpriteRectDict[oldname];
			SpriteRectDict.Remove(oldname);
			SpriteRectDict.Add(newname, spriteindex);
		}
	}
}
