using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Groups;
using Moon.Communication.Packets.Outgoing.Groups;

namespace Moon.Communication.Packets.Incoming.Groups
{
    class ManageGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("group_management_override"))
                return;

            Session.SendMessage(new ManageGroupComposer(Group));
        }
    }
}
