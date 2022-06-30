using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Helpers;
using Moon.Communication.Packets.Outgoing.Help;
using Moon.Communication.Packets.Outgoing;

namespace Moon.Communication.Packets.Incoming.Help.Helpers
{
    class ReportBullyUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            //int userId = Packet.PopInt();
            //int roomId = Packet.PopInt();
            //string message = Packet.PopString();

            //Session.SendWhisper(message);

            //var call = HelperToolsManager.AddCall(Session, "", 3);
            //var helpers = HelperToolsManager.GetHelpersToCase(call).FirstOrDefault();

            //if (helpers != null)
            //{
            //    HelperToolsManager.InviteGuardianCall(helpers, call);
            //}

                //var response = new ServerPacket(ServerPacketHeader.GuideSessionErrorMessageComposer);
                //response.WriteInteger(0);
                //Session.SendMessage(response);
                //return;
            }
        }
    }
