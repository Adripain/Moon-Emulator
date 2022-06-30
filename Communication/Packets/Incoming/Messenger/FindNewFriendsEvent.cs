using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.Session;
using Moon.Communication.Packets.Outgoing.Messenger;

namespace Moon.Communication.Packets.Incoming.Messenger
{
    class FindNewFriendsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Instance = MoonEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (Instance != null)
            {
                Session.SendMessage(new FindFriendsProcessResultComposer(true));
                Session.SendMessage(new RoomForwardComposer(Instance.Id));
            }
            else
            {
                Session.SendMessage(new FindFriendsProcessResultComposer(false));
            }
        }
    }
}
