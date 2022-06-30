using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Help.Helpers
{
    class AcceptHelperSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Accepted = Packet.PopBoolean();
            var Helper = HelperToolsManager.GetHelper(Session);

            if (Helper == null)
            {
                Session.SendMessage(new Outgoing.Help.Helpers.CloseHelperSessionComposer());
                return;
            }

            if (Accepted)
                Helper.Accept();
            else
                Helper.Decline();



        }
    }
}
