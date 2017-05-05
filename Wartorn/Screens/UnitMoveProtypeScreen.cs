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
using Wartorn.Drawing;
using Newtonsoft.Json;
using Wartorn.Drawing.Animation;

namespace Wartorn.Screens
{
    class UnitMoveProtypeScreen : Screen
    {
        SessionData sessiondata;
        public UnitMoveProtypeScreen(GraphicsDevice device) : base(device, "UnitMoveProtypeScreen")
        {
            
        }

        public override bool Init()
        {
            return base.Init();
        }

        private void InitMap()
        {
            sessiondata.map = new Map(30, 20);
            sessiondata.map.Fill(TerrainType.Plain);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
