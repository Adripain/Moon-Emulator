using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Catalog
{
    class ClubGiftsComposer : ServerPacket
    {
        public ClubGiftsComposer()
            : base(ServerPacketHeader.ClubGiftsMessageComposer)
        {
            base.WriteInteger(-1);//Days until next gift.
            base.WriteInteger(0);//Gifts available
            base.WriteInteger(12);//Count?
            {
                base.WriteInteger(12701);
                base.WriteString("hc16_1");
                base.WriteBoolean(false);
                base.WriteInteger(1);
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteBoolean(true);
                base.WriteInteger(1);//Count for some reason
                {
                    base.WriteString("s");
                    base.WriteInteger(8228);
                    base.WriteString("");
                    base.WriteInteger(1);
                    base.WriteBoolean(false);
                }
                //  base.WriteInteger(0);
                //base.WriteBoolean(true);
            }

            base.WriteInteger(0);//Count
            {
                //int, bool, int, bool
                base.WriteInteger(3253248);//Maybe the item id?

                base.WriteBoolean(false);//Can we get?
                base.WriteInteger(256);//idk
                base.WriteBoolean(false);//idk
                base.WriteInteger(0);
                base.WriteBoolean(true);

            }
        }
    }
}
