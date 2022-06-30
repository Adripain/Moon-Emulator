using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;
using Moon.HabboHotel.Groups;

using Moon.Communication.Packets.Outgoing.Groups;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class GetGroupFurniSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            int ItemId = Packet.PopInt();
            int GroupId = Packet.PopInt();

            Item Item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.Data.InteractionType != InteractionType.GUILD_GATE)
                return;

            Group Group = null;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            Session.SendMessage(new GroupFurniSettingsComposer(Group, ItemId, Session.GetHabbo().Id));
            Session.SendMessage(new GroupInfoComposer(Group, Session, false));
        }
    }
}