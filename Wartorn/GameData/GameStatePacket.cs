using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wartorn.GameData
{
    public class GameStatePacket
    {
        public Guid packetID { get; private set; }

        public Guid unitID { get; private set; }

    }
}
