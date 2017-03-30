using System;
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

namespace Wartorn.Screens
{
    class EditorScreen : Screen
    {
        private Map map;
        private Canvas canvas;

        public EditorScreen(GraphicsDevice device) : base(device, "EditorScreen")
        {
            map = new Map();
            canvas = new Canvas();
            InitUI();
        }
        private void InitUI()
        {
            Button AirPort = new Button()
            {

            };
        }

        public override void Update(GameTime gameTime)
        {
            canvas.Update(CONTENT_MANAGER.inputState, CONTENT_MANAGER.lastInputState);
        }

        public override void Draw(GameTime gameTime)
        {
            canvas.Draw(CONTENT_MANAGER.spriteBatch);
        }
    }
}
