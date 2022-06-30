using System;
using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Collections;
using Moon.Communication.Packets.Outgoing;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    class SendYouTubeVideoBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectSendYouTubeVideo; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public SendYouTubeVideoBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Link = Packet.PopString();

            this.StringData = Link;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            ServerPacket packet = new ServerPacket(2);
            packet.WriteString("Youtube");
            packet.WriteString("<iframe id=\"youtube-player\" frameborder=\"0\" allowfullscreen=\"1\" allow=\"autoplay; encrypted - media\" title=\"YouTube video player\" width=\"480\" height=\"270\" src=\"https://www.youtube.com/embed/" + StringData + "?autoplay=1&amp;fs=0&amp;modestbranding=1&amp;rel=0&amp;enablejsapi=1&amp;origin=http%3A%2F%2Fhabblive.in&amp;widgetid=1\"></iframe>");

            if (Player.GetClient().wsSession != null)
                Player.GetClient().wsSession.send(packet);

            return true;
        }
    }
}
