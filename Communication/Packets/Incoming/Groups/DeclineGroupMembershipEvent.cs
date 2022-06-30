using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Groups;
using Moon.Communication.Packets.Outgoing.Groups;
using Moon.Communication.Packets.Outgoing.Rooms.Permissions;


using Moon.HabboHotel.Users;
using Moon.HabboHotel.Cache;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class DeclineGroupMembershipEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id))
                return;

            if (!Group.HasRequest(UserId))
                return;

            Group.HandleRequest(UserId, false);
            Session.SendMessage(new UnknownGroupComposer(Group.Id, UserId));
        }
    }
}