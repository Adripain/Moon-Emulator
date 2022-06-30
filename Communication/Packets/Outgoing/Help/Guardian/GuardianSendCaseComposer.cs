using Moon.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Help.Helpers
{
    class GuardianSendCaseComposer : ServerPacket
    {
        public GuardianSendCaseComposer(int seconds, Habbo reported)
            : base(ServerPacketHeader.GuardianSendCaseMessageComposer)
        {
            base.WriteInteger(1); // length
            base.WriteInteger(3); // type: Bully
            base.WriteInteger(15); // timer sec
            base.WriteBoolean(false); // false = usuario, true = null
            //if (user != null)
            //{
            base.WriteString(reported.Username);
            base.WriteString(reported.Look);
            base.WriteString((reported.CurrentRoom == null) ? "" : reported.CurrentRoom.RoomData.Name);
            //}
        }
    }
}
