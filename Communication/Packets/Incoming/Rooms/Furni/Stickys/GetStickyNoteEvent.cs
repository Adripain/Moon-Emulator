using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Rooms.Furni.Stickys;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni.Stickys
{
    class GetStickyNoteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            Item Item = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null || Item.GetBaseItem().InteractionType != InteractionType.POSTIT)
                return;

            Session.SendMessage(new StickyNoteComposer(Item.Id.ToString(), Item.ExtraData));
        }
    }
}