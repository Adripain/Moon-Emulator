using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.GameCenter
{
    class GameAccountStatusComposer : ServerPacket
    {
        public GameAccountStatusComposer(int GameID)
            : base(ServerPacketHeader.GameAccountStatusMessageComposer)
        {
            base.WriteInteger(GameID);
            base.WriteInteger(100); // Games Left
            base.WriteInteger(0);//Was 16?
        }
    }
}