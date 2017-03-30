using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wartorn.ScreenManager;
using Microsoft.Xna.Framework.Input;

namespace Wartorn.Screens
{
    class BlueScreen : Screen
    {
        public BlueScreen(GraphicsDevice device) : base(device, "BlueScreen")
        {
        }

        public override bool Init()
        {
            return base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override void Update(GameTime gameTime)
        {
            var inputState = CONTENT_MANAGER.inputState;
            var lastInputState = CONTENT_MANAGER.lastInputState;

            if (inputState.keyboardState.IsKeyUp(Keys.E) && lastInputState.keyboardState.IsKeyDown(Keys.E))
            {
                SCREEN_MANAGER.goto_screen("RedScreen");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _device.Clear(Color.Blue);
        }
    }
}
