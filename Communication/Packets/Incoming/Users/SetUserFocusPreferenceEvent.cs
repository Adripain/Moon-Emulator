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
    class SetUserFocusPreferenceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            bool FocusPreference = Packet.PopBoolean();

            Session.GetHabbo().FocusPreference = FocusPreference;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `focus_preference` = @focusPreference WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("focusPreference", MoonEnvironment.BoolToEnum(FocusPreference));
                dbClient.RunQuery();
            }
        }
    }
}
