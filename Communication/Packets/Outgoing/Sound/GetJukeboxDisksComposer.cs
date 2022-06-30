using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Rooms.Music;

namespace Moon.Communication.Packets.Outgoing.Rooms.Music
{
    class GetJukeboxDisksComposer : ServerPacket
    {
        public GetJukeboxDisksComposer(Dictionary<int, Item> songs)
            : base(ServerPacketHeader.GetJukeboxDisksMessageComposer)
        {
            base.WriteInteger(songs.Count);

            foreach (Item userItem in songs.Values.ToList())
            {
                int songID = int.Parse(userItem.ExtraData);
                SongData Data = MoonEnvironment.GetGame().GetMusicManager().GetSong(songID);

                base.WriteInteger(userItem.Id);
                base.WriteInteger(Data.Id);
            }
        }
    }
}
