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
    struct PlayerInfo
    {
        public int playerId;
        public Owner owner;
        public PlayerInfo(int playerid,Owner owner)
        {
            playerId = playerid;
            this.owner = owner;
        }
    }

    struct SessionData
    {
        public GameMode gameMode;
        public PlayerInfo[] playerId;
        public Map map;
    }

    struct Session
    {
        public int turn;
        public Weather weather;
        public Map map;
        public GameMode gameMode;

        public Session(SessionData sessiondata)
        {
            map = sessiondata.map;
            gameMode = sessiondata.gameMode;
            turn = 0;
            weather = Weather.Sunny;
        }
    }
}
