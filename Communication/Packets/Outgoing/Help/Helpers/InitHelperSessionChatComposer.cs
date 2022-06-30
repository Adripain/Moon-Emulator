using Moon.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Help.Helpers
{
    class InitHelperSessionChatComposer : ServerPacket
    {

        public InitHelperSessionChatComposer(Habbo Habbo1, Habbo Habbo2)
            : base(ServerPacketHeader.InitHelperSessionChatMessageComposer)
        {
            base.WriteInteger(Habbo1.Id);
            base.WriteString(Habbo1.Username);
            base.WriteString(Habbo1.Look);

            base.WriteInteger(Habbo2.Id);
            base.WriteString(Habbo2.Username);
            base.WriteString(Habbo2.Look);




        }
    }
}
