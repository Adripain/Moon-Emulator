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
using Moon.HabboHotel.Items;

namespace Moon.HabboHotel.Rooms.Music
{
    public class SongInstance
    {
        private readonly SongItem mDiskItem;
        private readonly SongData mSongData;

        public SongInstance(SongItem Item, SongData SongData)
        {
            mDiskItem = Item;
            mSongData = SongData;
        }

        public SongData SongData
        {
            get { return mSongData; }
        }

        public SongItem DiskItem
        {
            get { return mDiskItem; }
        }
    }
}