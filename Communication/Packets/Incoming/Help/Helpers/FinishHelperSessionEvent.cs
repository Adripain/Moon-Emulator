using Moon.Communication.Packets.Outgoing.Help.Helpers;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Help.Helpers
{
    class FinishHelperSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Voted = Packet.PopBoolean();
            var Element = HelperToolsManager.GetElement(Session);
            if (Element is HelperCase)
            {
                if (Voted)
                {
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Element.OtherElement.Session.GetHabbo().Username + ", gracias por colaborar en el programa de Alfas, has atendido correctamente la duda del usuario.", ""));
                    //if (Element.OtherElement.Session.GetHabbo()._guidelevel >= 1)
                    //{
                    //    MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Element.OtherElement.Session, "ACH_GuideTourGiver", 1);
                    //}
                }
                else
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Element.OtherElement.Session.GetHabbo().Username + ", gracias por colaborar en el programa de Alfas, has atendido satisfactoriamente la duda del usuario.", ""));
            }

            Element.Close();
        }
    }
}
