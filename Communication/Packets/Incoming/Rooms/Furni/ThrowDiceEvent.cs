using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class ThrowDiceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            Item Item = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null)
                return;

            Boolean hasRights = false;
            if (Room.CheckRights(Session, false, true))
                hasRights = true;

            int request = Packet.PopInt();

            Item.Interactor.OnTrigger(Session, Item, request, hasRights);
        }
    }
}