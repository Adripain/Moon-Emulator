
using System.Linq;
using Moon.Communication.Packets.Outgoing.Rooms.Music;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Rooms.Music;

namespace Moon.Communication.Packets.Incoming.Rooms.Music
{
    class RemovePlaylistItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Instance = Session.GetHabbo().CurrentRoom;

            if (Instance == null || !Instance.CheckRights(Session, true))
            {
                return;
            }

            SongItem TakenItem = Instance.GetRoomMusicManager().RemoveDisk(Packet.PopInt());

            if (TakenItem != null)
            {
                TakenItem.RemoveFromDatabase();
                Session.GetHabbo().GetInventoryComponent().AddNewItem(TakenItem.itemID, TakenItem.baseItem.Id, TakenItem.songID.ToString(), 0, false, false, 0, 0);
                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                Session.SendMessage(new GetJukeboxDisksComposer(Session.GetHabbo().GetInventoryComponent().songDisks));
                Session.SendMessage(new GetJukeboxPlaylistsComposer(MusicManager.PlaylistCapacity, Instance.GetRoomMusicManager().Playlist.Values.ToList()));
            }
        }
    }
}
