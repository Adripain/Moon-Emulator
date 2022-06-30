using Moon.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Help.Helpers
{
    class HelperSessioChatTypingEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var element = HabboHotel.Helpers.HelperToolsManager.GetElement(Session);
            if (element != null && element.OtherElement != null)
                element.OtherElement.Session.SendMessage(new Outgoing.Help.Helpers.HelperSessionChatIsTypingComposer(Packet.PopBoolean()));
        }
    }
}
