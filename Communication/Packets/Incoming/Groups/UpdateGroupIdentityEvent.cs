using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Groups;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Groups;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class UpdateGroupIdentityEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            string word;
            string Name = Packet.PopString();
            Name = MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out word) ? "Spam" : Name;
            string Desc = Packet.PopString();
            Desc = MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Desc, out word) ? "Spam" : Desc;

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `name`= @name, `desc` = @desc WHERE `id` = '" + GroupId + "' LIMIT 1");
                dbClient.AddParameter("name", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.RunQuery();
            }

            Group.Name = Name;
            Group.Description = Desc;

            Session.SendMessage(new GroupInfoComposer(Group, Session));
        }
    }
}
