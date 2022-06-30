using System;
using System.Linq;
using System.Text;

using Moon.HabboHotel.Items.Televisions;
using Moon.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;

namespace Moon.Communication.Packets.Incoming.Rooms.Furni
{
    class ToggleYouTubeVideoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();//Item Id
            string VideoId = Packet.PopString(); //Video ID

            Session.SendMessage(new GetYouTubeVideoComposer(ItemId, VideoId));
        }
    }
}