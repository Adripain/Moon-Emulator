using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Groups;
using Moon.HabboHotel.Users;

using Moon.Utilities;

namespace Moon.Communication.Packets.Outgoing.Rooms.Engine
{
    class ObjectAddComposer : ServerPacket
    {
        public ObjectAddComposer(Item Item, Room Room)
            : base(ServerPacketHeader.ObjectAddMessageComposer)
        {
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);
            base.WriteInteger(Item.GetX);
            base.WriteInteger(Item.GetY);
            base.WriteInteger(Item.Rotation);
            base.WriteString(String.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
            base.WriteString(String.Empty);

            if (Item.LimitedNo > 0)
            {
                base.WriteInteger(1);
                base.WriteInteger(256);
                base.WriteString(Item.ExtraData);
                base.WriteInteger(Item.LimitedNo);
                base.WriteInteger(Item.LimitedTot);
            }
            else if (Item.Data.InteractionType == InteractionType.INFO_TERMINAL || Item.Data.InteractionType == InteractionType.ROOM_PROVIDER)
            {
                base.WriteInteger(0);
                base.WriteInteger(1);
                base.WriteInteger(1);
                base.WriteString("internalLink");
                base.WriteString(Item.ExtraData);
            }
            else if (Item.Data.InteractionType == InteractionType.FX_PROVIDER)
            {
                base.WriteInteger(0);
                base.WriteInteger(1);
                base.WriteInteger(1);
                base.WriteString("effectId");
                base.WriteString(Item.ExtraData);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATA)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString("6");
                if (Item.ExtraData.Length <= 0) base.WriteInteger(0);
                else base.WriteInteger(int.Parse(Item.ExtraData));
                base.WriteInteger(100);
            }

            else if (Item.Data.InteractionType == InteractionType.PLANT_SEED)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(12);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATATRIGGERED)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString("0");
                if (Item.ExtraData.Length <= 0) base.WriteInteger(0);
                else base.WriteInteger(int.Parse(Item.ExtraData));
                base.WriteInteger(1);
            }

            else if (Item.Data.InteractionType == InteractionType.EASTEREGG)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(20);
            }

            else if (Item.Data.InteractionType == InteractionType.MAGICEGG)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(23);
            }
            else if (Item.Data.InteractionType == InteractionType.CAJAFORBI)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(1);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            base.WriteInteger(-1); // to-do: check
            base.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            base.WriteInteger(Item.UserID);
            base.WriteString(Item.Username);
        }
    }
}