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
    public class PlayerInfo
    {
        public int playerId;
        public Owner owner;
        public int money;
        public List<Guid> ownedUnit;
        public PlayerInfo(int playerid,Owner owner,int startingmoney = 0)
        {
            playerId = playerid;
            this.owner = owner;
            money = startingmoney;
            ownedUnit = new List<Guid>();
        }
    }

    struct SessionData
    {
        public GameMode gameMode;
        public PlayerInfo[] playerInfos;
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
