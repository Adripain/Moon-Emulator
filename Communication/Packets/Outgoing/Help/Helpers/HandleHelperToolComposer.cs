using Moon.HabboHotel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Help.Helpers
{
    class HandleHelperToolComposer : ServerPacket
    {
        public HandleHelperToolComposer(bool onDuty, int helperAmount, int guideAmount, int guardianAmount)
            : base(ServerPacketHeader.HandleHelperToolMessageComposer)
        {
            base.WriteBoolean(onDuty);
            base.WriteInteger(guideAmount);
            base.WriteInteger(helperAmount);
            base.WriteInteger(guardianAmount);
        }

        public HandleHelperToolComposer(bool onDuty)
            : base(ServerPacketHeader.HandleHelperToolMessageComposer)
        {
            base.WriteBoolean(onDuty);
            base.WriteInteger(HelperToolsManager.GuideCount);
            base.WriteInteger(HelperToolsManager.HelperCount);
            base.WriteInteger(HelperToolsManager.GuardianCount);

        }

    }
}
