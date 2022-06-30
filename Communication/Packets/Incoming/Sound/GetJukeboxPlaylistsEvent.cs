using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Rooms.Music;
using Moon.Communication.Packets.Outgoing.Rooms.Music;

namespace Moon.Communication.Packets.Incoming.Rooms.Music
{
    class GetJukeboxPlaylistsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session != null)
            {
                Room Instance = Session.GetHabbo().CurrentRoom;

                if (Instance == null || !Instance.CheckRights(Session, true))
                    return;

                Session.SendMessage(new GetJukeboxPlaylistsComposer(MusicManager.PlaylistCapacity, Instance.GetRoomMusicManager().Playlist.Values.ToList()));
            }
        }
    }
}
