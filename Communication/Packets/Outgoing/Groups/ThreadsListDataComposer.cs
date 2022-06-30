using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Groups
{
    class ThreadsListDataComposer : ServerPacket
    {
        public ThreadsListDataComposer(GroupForum Forum, GameClient Session, int StartIndex = 0, int MaxLength = 20)
            : base(ServerPacketHeader.ThreadsListDataMessageComposer)
        {
            base.WriteInteger(Forum.GroupId);//Group Forum ID
            base.WriteInteger(StartIndex); //Page Index

            var Threads = Forum.Threads;
            if (Threads.Count - 1 >= StartIndex)
                Threads = Threads.GetRange(StartIndex, Math.Min(MaxLength, Threads.Count - StartIndex));

            base.WriteInteger(Threads.Count); //Thread Count

            var UnPinneds = new List<GroupForumThread>();

            foreach (var Thread in Threads)
            {
                if (!Thread.Pinned)
                {
                    UnPinneds.Add(Thread);
                    continue;
                }

                Thread.SerializeData(Session, this);
            }

            foreach (var unPinned in UnPinneds)
            {
                unPinned.SerializeData(Session, this);
            }
        }
    }
}

