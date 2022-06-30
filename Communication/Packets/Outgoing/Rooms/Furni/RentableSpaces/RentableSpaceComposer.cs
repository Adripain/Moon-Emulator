using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces
{
    public class RentableSpaceComposer : ServerPacket
    {

        public RentableSpaceComposer()
            : base(ServerPacketHeader.RentableSpaceMessageComposer)
        {
            base.WriteBoolean(true); //Is rented y/n
            base.WriteInteger(-1); //No fucking clue
            base.WriteInteger(-1); //No fucking clue
            base.WriteString("Tyler-Retros"); //Username of who owns.
            base.WriteInteger(360); //Time to expire.
            base.WriteInteger(-1); //No fucking clue
        }

        public RentableSpaceComposer(bool Rented, int ErrorId, int OwnerId, string OwnerUsername, int ExpireSecs, int Price)
            : base(ServerPacketHeader.RentableSpaceMessageComposer)
        {
            base.WriteBoolean(Rented); //Is rented??
            base.WriteInteger(ErrorId);
            base.WriteInteger(OwnerId);
            base.WriteString(OwnerUsername);
            base.WriteInteger(ExpireSecs);
            base.WriteInteger(Price);
        }

        public RentableSpaceComposer(int OwnerId, string OwnerUsername, int ExpireSecs)
            : base(ServerPacketHeader.RentableSpaceMessageComposer)
        {
            base.WriteBoolean(true); //Is rented??
            base.WriteInteger(0);
            base.WriteInteger(OwnerId);
            base.WriteString(OwnerUsername);
            base.WriteInteger(ExpireSecs);
            base.WriteInteger(0);
        }
    }
}