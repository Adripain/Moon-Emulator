﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectAddedComposer : ServerPacket
    {
        public AvatarEffectAddedComposer(int SpriteId, int Duration)
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
            base.WriteInteger(SpriteId);
            base.WriteInteger(0);//Types
            base.WriteInteger(Duration);
            base.WriteBoolean(false);//Permanent
        }
    }
}
