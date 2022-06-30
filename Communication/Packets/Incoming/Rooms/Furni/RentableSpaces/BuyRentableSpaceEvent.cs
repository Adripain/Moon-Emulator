using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Items.RentableSpaces;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class BuyRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            int itemId = Packet.PopInt();

            Room room;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out room))
                return;

            if (room == null || room.GetRoomItemHandler() == null)
                return;

            RentableSpaceItem rsi;
            if (MoonEnvironment.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(itemId, out rsi))
            {
                MoonEnvironment.GetGame().GetRentableSpaceManager().ConfirmBuy(Session, rsi, 3600);
            }


        }
    }
}