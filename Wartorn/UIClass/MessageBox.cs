using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wartorn.UIClass {
	public class MessageBox : UIObject {
		Button button;

		public void Show() {
			this.isVisible = true;
		}

		public void Hide() {
			this.isVisible = false;
		}

		public override void Update(GameTime gameTime, InputState inputState, InputState lastInputState) {
			base.Update(gameTime, inputState, lastInputState);
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
			//draw the border
			//draw the prompt
			//draw button
		}
	}
}
