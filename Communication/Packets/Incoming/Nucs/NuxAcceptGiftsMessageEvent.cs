using System;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Utilities;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Nux;

namespace Moon.Communication.Packets.Incoming.Rooms.Nux
{
    class NuxAcceptGiftsMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new NuxItemListComposer());
        }
    }
}