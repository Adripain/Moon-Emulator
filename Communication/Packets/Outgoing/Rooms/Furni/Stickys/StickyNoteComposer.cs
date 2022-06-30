using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Rooms.Furni.Stickys
{
    class StickyNoteComposer : ServerPacket
    {
        public StickyNoteComposer(string ItemId, string Extradata)
            : base(ServerPacketHeader.StickyNoteMessageComposer)
        {
           base.WriteString(ItemId);
           base.WriteString(Extradata);
        }
    }
}
