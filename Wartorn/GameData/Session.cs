using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Wartorn.Utility.Drawing;

namespace Wartorn.GameData
{
    class Session
    {
        int turn;
        Weather weather;
        Map map;
        GameMode gameMode;

        public Session(SessionData sessiondata)
        {
            map = sessiondata.map;
            gameMode = sessiondata.gameMode;
        }
    }
}
