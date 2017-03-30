using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Wartorn.ScreenManager;

namespace Wartorn.Screens
{
    class RedScreen : Screen
    {
        public RedScreen(GraphicsDevice device) : base(device, "RedScreen")
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
                SCREEN_MANAGER.goto_screen("BlueScreen");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _device.Clear(Color.Red);
        }
    }
}
