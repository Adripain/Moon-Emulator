using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Users;
using Moon.HabboHotel.Groups;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Session;

namespace Moon.Communication.Packets.Incoming.Users
{
    class OpenPlayerProfileEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int userID = Packet.PopInt();
            Boolean IsMe = Packet.PopBoolean();

            Habbo targetData = MoonEnvironment.GetHabboById(userID);
            if (targetData == null)
            {
                Session.SendNotification("Se produjo un error mientras se encontraba el perfil de ese usuario .");
                return;
            }
            
            List<Group> Groups = MoonEnvironment.GetGame().GetGroupManager().GetGroupsForUser(targetData.Id);
            
            int friendCount = 0;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `messenger_friendships` WHERE (`user_one_id` = @userid OR `user_two_id` = @userid)");
                dbClient.AddParameter("userid", userID);
                friendCount = dbClient.getInteger();
            }

            Session.SendMessage(new ProfileInformationComposer(targetData, Session, Groups, friendCount));
        }
    }
}
