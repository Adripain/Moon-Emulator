using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Items;
using Moon.HabboHotel.Items.Wired;


namespace Moon.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    class WiredEffectConfigComposer : ServerPacket
    {
        public WiredEffectConfigComposer(IWiredItem Box, List<int> BlockedItems)
            : base(ServerPacketHeader.WiredEffectConfigMessageComposer)
        {
            base.WriteBoolean(false);
            if (Box.Type == WiredBoxType.EffectMoveUser || Box.Type == WiredBoxType.EffectProgressUserAchievement || Box.Type == WiredBoxType.EffectTimerReset)
                base.WriteInteger(0);
            else
                base.WriteInteger(20);

            base.WriteInteger(Box.SetItems.Count);
            foreach (Item Item in Box.SetItems.Values.ToList())
                base.WriteInteger(Item.Id);

            base.WriteInteger(Box.Item.GetBaseItem().SpriteId);
            base.WriteInteger(Box.Item.Id);

            if (Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "Bot name;0";

                base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;Bot name";

                base.WriteString(Box.StringData != null ? (Box.StringData.Split(';')[1]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectGiveReward)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "1,,;1,,;1,,;1,,;1,,-0-0-0";

                base.WriteString(Box.StringData != null ? (Box.StringData.Split('-')[0]) : "");
            }
            else if (Box.Type == WiredBoxType.EffectMoveToDir)
                base.WriteString(string.Empty);
            else if (Box.Type == WiredBoxType.EffectTimerReset)
            {
                base.WriteString("");
            }
            else
            {
                base.WriteString(Box.StringData);
            }

            if (Box.Type != WiredBoxType.EffectMatchPosition &&
                Box.Type != WiredBoxType.EffectMoveAndRotate &&
                Box.Type != WiredBoxType.EffectMuteTriggerer &&
                Box.Type != WiredBoxType.EffectBotFollowsUserBox &&
                Box.Type != WiredBoxType.EffectAddScore &&
                Box.Type != WiredBoxType.EffectMoveToDir &&
                Box.Type != WiredBoxType.EffectGiveReward &&
                Box.Type != WiredBoxType.EffectAddRewardPoints &&
                Box.Type != WiredBoxType.EffectMoveUser)
            {
                base.WriteInteger(0); // Loop
            }
            else if (Box.Type == WiredBoxType.EffectMatchPosition)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0;0";

                base.WriteInteger(3);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[2]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveUser)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0";

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveToDir)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0";

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 50);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 5);
            }
            else if (Box.Type == WiredBoxType.EffectGiveReward)
            {
                base.WriteInteger(4);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[1]) : 0);
                base.WriteInteger(Box.BoolData ? 1 : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[2]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split('-')[3]) : 1);
            }
            else if (Box.Type == WiredBoxType.EffectAddScore || Box.Type == WiredBoxType.EffectAddRewardPoints)
            {

                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "1;1";

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMoveAndRotate)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;0";

                base.WriteInteger(2);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectMuteTriggerer)
            {
                if (String.IsNullOrEmpty(Box.StringData))
                    Box.StringData = "0;Message";

                base.WriteInteger(1);//Count, for the time.
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotFollowsUserBox)
            {
                base.WriteInteger(1);//Count, for the time.
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[0]) : 0);
            }
            else if (Box.Type == WiredBoxType.EffectBotGivesHanditemBox)
            {
                base.WriteInteger(Box.StringData != null ? int.Parse(Box.StringData.Split(';')[1]) : 0);
            }

            if (Box is IWiredCycle && Box.Type != WiredBoxType.EffectKickUser && Box.Type != WiredBoxType.EffectMatchPosition && Box.Type != WiredBoxType.EffectMoveAndRotate && Box.Type != WiredBoxType.EffectSetRollerSpeed && Box.Type != WiredBoxType.EffectAddScore && Box.Type != WiredBoxType.EffectAddRewardPoints &&
                Box.Type != WiredBoxType.EffectMoveToDir && Box.Type != WiredBoxType.EffectMoveUser && Box.Type != WiredBoxType.EffectShowMessage && Box.Type != WiredBoxType.EffectGiveUserHanditem && Box.Type != WiredBoxType.EffectGiveUserEnable && Box.Type != WiredBoxType.EffectTimerReset
                && Box.Type != WiredBoxType.EffectGiveUserFreeze && Box.Type != WiredBoxType.EffectExecuteWiredStacks)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(0);
                base.WriteInteger(Cycle.Delay);
            }
            else if (Box.Type == WiredBoxType.EffectMatchPosition || Box.Type == WiredBoxType.EffectMoveAndRotate || Box.Type == WiredBoxType.EffectAddScore || Box.Type == WiredBoxType.EffectAddRewardPoints || Box.Type == WiredBoxType.EffectMoveToDir || Box.Type == WiredBoxType.EffectMoveUser || Box.Type == WiredBoxType.EffectShowMessage || Box.Type == WiredBoxType.EffectGiveUserHanditem || Box.Type == WiredBoxType.EffectGiveUserEnable || Box.Type == WiredBoxType.EffectTimerReset || Box.Type == WiredBoxType.EffectGiveUserFreeze || Box.Type == WiredBoxType.EffectExecuteWiredStacks)
            {
                IWiredCycle Cycle = (IWiredCycle)Box;
                base.WriteInteger(0);
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(Cycle.Delay);
            }

            else
            {
                base.WriteInteger(0);
                base.WriteInteger(WiredBoxTypeUtility.GetWiredId(Box.Type));
                base.WriteInteger(0);
            }

            base.WriteInteger(BlockedItems.Count());
            if (BlockedItems.Count() > 0)
            {
                foreach (int ItemId in BlockedItems.ToList())
                    base.WriteInteger(ItemId);
            }
        }
    }
}