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
using Moon.Communication.Packets.Outgoing.Rooms.Music;

namespace Moon.HabboHotel.Rooms.Music
{
    public class MusicManager
    {
        private static bool mBroadcastNeeded;
        private bool mIsPlaying;
        private Dictionary<int, SongItem> mLoadedDisks;
        private SortedDictionary<int, SongInstance> mPlaylist;
        private Item mRoomOutputItem;
        private SongInstance mSong;
        private int mSongQueuePosition;
        private double mStartedPlayingTimestamp;

        public MusicManager()
        {
            mLoadedDisks = new Dictionary<int, SongItem>();
            mPlaylist = new SortedDictionary<int, SongInstance>();
        }

        public SongInstance CurrentSong
        {
            get { return mSong; }
        }

        public bool IsPlaying
        {
            get { return mIsPlaying; }
        }

        private double TimePlaying
        {
            get { return MoonEnvironment.GetUnixTimestamp() - mStartedPlayingTimestamp; }
        }

        public int SongSyncTimestamp
        {
            get
            {
                if (!mIsPlaying || mSong == null)
                {
                    return 0;
                }

                if (TimePlaying >= mSong.SongData.LengthSeconds)
                {
                    return (int)mSong.SongData.LengthSeconds;
                }

                return (int)(TimePlaying * 1000);
            }
        }

        public SortedDictionary<int, SongInstance> Playlist
        {
            get
            {
                SortedDictionary<int, SongInstance> Copy = new SortedDictionary<int, SongInstance>();

                foreach (KeyValuePair<int, SongInstance> Data in mPlaylist)
                {
                    Copy.Add(Data.Key, Data.Value);
                }

                return Copy;
            }
        }

        public static int PlaylistCapacity
        {
            get { return 10; }
        }

        public int PlaylistSize
        {
            get { return mPlaylist.Count; }
        }

        public bool HasLinkedItem
        {
            get { return mRoomOutputItem != null; }
        }

        public int LinkedItemId
        {
            get { return (mRoomOutputItem != null ? mRoomOutputItem.Id : 0); }
        }

        public Item LinkedItem
        {
            get { return mRoomOutputItem; }
        }

        public int SongQueuePosition
        {
            get { return mSongQueuePosition; }
        }

        public void LinkRoomOutputItem(Item Item)
        {
            mRoomOutputItem = Item;
        }

        public int AddDisk(SongItem DiskItem)
        {
            int SongId = (int)DiskItem.songID;

            if (SongId == 0)
                return -1;

            SongData SongData = MoonEnvironment.GetGame().GetMusicManager().GetSong(SongId);

            if (SongData == null)
                return -1;

            if (mLoadedDisks.ContainsKey(DiskItem.itemID))
                return -1;

            mLoadedDisks.Add(DiskItem.itemID, DiskItem);

            int NewPlaylistId = mPlaylist.Count;

            mPlaylist.Add(NewPlaylistId, new SongInstance(DiskItem, SongData));

            return NewPlaylistId;
        }

        public SongItem RemoveDisk(int PlaylistIndex)
        {
            SongInstance Instance = null;

            if (!mPlaylist.ContainsKey(PlaylistIndex))
            {
                return null;
            }

            Instance = mPlaylist[PlaylistIndex];
            mPlaylist.Remove(PlaylistIndex);

            mLoadedDisks.Remove(Instance.DiskItem.itemID);

            RepairPlaylist();

            if (PlaylistIndex == mSongQueuePosition)
            {
                PlaySong();
            }

            return Instance.DiskItem;
        }

        public void Update(Room Instance)
        {
            if (mIsPlaying && (mSong == null || (TimePlaying >= (mSong.SongData.LengthSeconds + 1))))
            {
                if (mPlaylist.Count == 0)
                {
                    Stop();

                    mRoomOutputItem.ExtraData = "0";
                    mRoomOutputItem.UpdateState();
                }
                else
                {
                    SetNextSong();
                }

                mBroadcastNeeded = true;
            }

            if (mBroadcastNeeded)
            {
                BroadcastCurrentSongData(Instance);
                mBroadcastNeeded = false;
            }
        }

        private void RepairPlaylist()
        {
            List<SongItem> LoadedDiskCopy = null;

            LoadedDiskCopy = mLoadedDisks.Values.ToList();
            mLoadedDisks.Clear();

            mPlaylist.Clear();

            foreach (SongItem LoadedDisk in LoadedDiskCopy)
            {
                AddDisk(LoadedDisk);
            }
        }

        private void SetNextSong()
        {
            mSongQueuePosition++;
            PlaySong();
        }

        private void PlaySong()
        {
            if (mSongQueuePosition >= mPlaylist.Count)
            {
                mSongQueuePosition = 0;
            }

            if (mPlaylist.Count == 0)
            {
                Stop();
                return;
            }

            mSong = mPlaylist[mSongQueuePosition];
            mStartedPlayingTimestamp = MoonEnvironment.GetUnixTimestamp();
            mBroadcastNeeded = true;
        }

        public void Start()
        {
            mIsPlaying = true;
            mSongQueuePosition = -1;
            SetNextSong();
        }

        public void Stop()
        {
            mSong = null;
            mIsPlaying = false;
            mSongQueuePosition = -1;
            mBroadcastNeeded = true;
        }

        public void BroadcastCurrentSongData(Room Instance)
        {
            Instance.SendMessage(mSong != null
                ? new SyncMusicComposer(mSong.SongData.Id, mSongQueuePosition, 0)
                : new SyncMusicComposer(0, 0, 0));
        }

        public void OnNewUserEnter(RoomUser user)
        {
            if (user.IsBot || user.GetClient() == null || mSong == null)
                return;

            user.GetClient().SendMessage(new SyncMusicComposer(mSong.SongData.Id, mSongQueuePosition, SongSyncTimestamp));
        }

        public void Reset()
        {
            mLoadedDisks.Clear();

            mPlaylist.Clear();

            mRoomOutputItem = null;
            mSongQueuePosition = -1;
            mStartedPlayingTimestamp = 0;
        }

        public void Destroy()
        {
            if (mLoadedDisks != null)
                mLoadedDisks.Clear();

            if (mPlaylist != null)
                mPlaylist.Clear();

            mPlaylist = null;
            mLoadedDisks = null;
            mSong = null;
            mRoomOutputItem = null;
        }
    }
}