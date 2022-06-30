using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Sound;
using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Users
{
    class SetChatPreferenceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Boolean ChatPreference = Packet.PopBoolean();

            Session.GetHabbo().ChatPreference = ChatPreference;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `chat_preference` = @chatPreference WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("chatPreference", MoonEnvironment.BoolToEnum(ChatPreference));
                dbClient.RunQuery();
            }
        }
    }
}
