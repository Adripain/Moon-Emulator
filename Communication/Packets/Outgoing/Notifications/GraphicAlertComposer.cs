using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Notifications
{
    class GraphicAlertComposer : ServerPacket
    {
        public GraphicAlertComposer(string image) : base(ServerPacketHeader.GraphicAlertComposer)
        { base.WriteString("${image.library.url}" + image + ".png"); }
    }
}
