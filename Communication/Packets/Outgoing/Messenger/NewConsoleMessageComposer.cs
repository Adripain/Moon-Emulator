using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.Communication.Packets.Outgoing.Messenger
{
    class NewConsoleMessageComposer : ServerPacket
    {
        public NewConsoleMessageComposer(int Sender, string Message, int Time = 0)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
            base.WriteInteger(Sender);
            base.WriteString(Message);
            base.WriteInteger(Time);
        }
    }

    class FuckingConsoleMessageComposer : ServerPacket
    {
        public FuckingConsoleMessageComposer(int Sender, string Message, string Data)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
            base.WriteInteger(Sender);
            base.WriteString(Message);
            base.WriteInteger(0);
            base.WriteString(Data);
        }
    }
}
