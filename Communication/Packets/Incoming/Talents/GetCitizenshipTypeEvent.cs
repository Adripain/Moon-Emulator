/*using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Talents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Talents
{
    class GetCitizenshipTypeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            string data = Packet.PopString();

            Session.SendMessage(new TalentTrackLevelComposer(data));

        }
    }
}
*/