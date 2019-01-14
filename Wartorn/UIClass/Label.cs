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
	public class Label : UIObject {

		public virtual bool AutoSize {
			get;
			set;
		}

		protected string text;
		public virtual string Text {
			get {
				return text;
			}
			set {
				text = string.IsNullOrEmpty(value) ? "" : value;
			}
		}

		public override Point Position {
			get {
				return base.Position;
			}
			set {
				base.Position = value;
				//background.rectangle.Location = value;
				//border.rectangle.Location = value;
			}
		}

		public override Vector2 Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
				//background.rectangle.Size = value.ToPoint();
				//border.rectangle.Size = value.ToPoint();
			}
		}
		/// <summary>
		/// Offset of the text inside the button
		/// </summary>
		public Vector2 Origin {
			get { return origin; }
			set { origin = value; }
		}

		public float Depth { get; set; } = LayerDepth.GuiUpper;

		public Label() {

		}

		public Label(string text, Point position, Vector2 size, SpriteFont font) {
			//Init();
			Text = text;
			Position = position;
			Size = size;
			Font = font;
			origin = new Vector2(rect.X, rect.Y) + Size / 4;
		}

		public Label(string text, Point position, Vector2? size, SpriteFont font, float _scale) {
			//Init();
			Text = text;
			Position = position;
			if (size != null) {
				Size = size.Value;
			}
			else {
				Size = font.MeasureString(text);
				origin = position.ToVector2();
			}
			Font = font;
			Scale = _scale;
		}

		private void Init() {
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			spriteBatch.DrawString(Font ?? CONTENT_MANAGER.Fonts["defaultFont"], (string.IsNullOrEmpty(text)) ? "" : text, Position.ToVector2() - origin, ForegroundColor, Rotation, Vector2.Zero, scale, SpriteEffects.None, Depth);
			DrawingHelper.DrawRectangle(rect, BackgroundColor, true);
			DrawingHelper.DrawRectangle(rect, BorderColor, false);
		}
	}
}
