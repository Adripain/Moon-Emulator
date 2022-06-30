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
    class CancelRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            int itemId = Packet.PopInt();

            Room room;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out room))
                return;

            if (room == null || room.GetRoomItemHandler() == null)
                return;

            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            RentableSpaceItem _rentableSpace;
            if (!MoonEnvironment.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(itemId, out _rentableSpace))
                return;

            int errorCode = MoonEnvironment.GetGame().GetRentableSpaceManager().GetCancelErrorCode(Session, _rentableSpace);

            if (errorCode > 0)
            {
                Session.SendMessage(new RentableSpaceComposer(_rentableSpace.IsRented(), errorCode, _rentableSpace.OwnerId, _rentableSpace.OwnerUsername, _rentableSpace.GetExpireSeconds(), _rentableSpace.Price));
                return;
            }


            if (!MoonEnvironment.GetGame().GetRentableSpaceManager().ConfirmCancel(Session, _rentableSpace))
            {
                Session.SendNotification("global.error");
                return;
            }

            Session.SendMessage(new RentableSpaceComposer(false, 0, 0, "", 0, _rentableSpace.Price));
        }
    }
}