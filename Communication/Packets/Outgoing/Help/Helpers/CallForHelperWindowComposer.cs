using Moon.HabboHotel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Help.Helpers
{
    class CallForHelperWindowComposer : ServerPacket
    {
        public CallForHelperWindowComposer(bool IsHelper, int Category, string Message, int WaitTime)
            : base(ServerPacketHeader.CallForHelperWindowMessageComposer)
        {
            base.WriteBoolean(IsHelper);
            base.WriteInteger(Category);
            base.WriteString(Message);
            base.WriteInteger(WaitTime);
        }

        public CallForHelperWindowComposer(bool IsHelper, HelperCase Case)
            : base(ServerPacketHeader.CallForHelperWindowMessageComposer)
        {
            base.WriteBoolean(IsHelper);
            base.WriteInteger((int)Case.Type);
            base.WriteString(Case.Message);
            base.WriteInteger(Case.ReamingToExpire);
        }

    }
}
