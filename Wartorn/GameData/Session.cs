using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            map = Storage.MapData.LoadMap(sessiondata.mapName);
            gameMode = sessiondata.gameMode;
        }
    }
}
