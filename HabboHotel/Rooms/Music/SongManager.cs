using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Incoming;
using System.Collections.Concurrent;

using Moon.Database.Interfaces;
using log4net;

namespace Moon.HabboHotel.Rooms.Music
{
    public class SongManager
    {

        private Dictionary<int, SongData> songs;

        public SongManager()
        {
            this.songs = new Dictionary<int, SongData>();

            this.Init();
        }

        public void Init()
        {
            if (this.songs.Count > 0)
                songs.Clear();

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM jukebox_songs_data");
                DataTable dTable = dbClient.getTable();

                foreach (DataRow dRow in dTable.Rows)
                {
                    SongData song = new SongData(Convert.ToInt32(dRow["id"]), Convert.ToString(dRow["name"]), Convert.ToString(dRow["artist"]), Convert.ToString(dRow["song_data"]), Convert.ToDouble(dRow["length"]));
                    songs.Add(song.Id, song);
                }
            }
        }

        public SongData GetSong(int SongId)
        {
            SongData song = null;

            this.songs.TryGetValue(SongId, out song);

            return song;
        }
    }
}