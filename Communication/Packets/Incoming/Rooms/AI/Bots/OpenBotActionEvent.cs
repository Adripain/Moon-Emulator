﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Moon.HabboHotel.Rooms.AI.Speech;


namespace Moon.Communication.Packets.Incoming.Rooms.AI.Bots
{
    class OpenBotActionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            int BotId = Packet.PopInt();
            int ActionId = Packet.PopInt();

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser BotUser = null;
            if (!Room.GetRoomUserManager().TryGetBot(BotId, out BotUser))
                return;

            string BotSpeech = "";
            foreach (RandomSpeech Speech in BotUser.BotData.RandomSpeech.ToList())
            {
                BotSpeech += (Speech.Message + "\n");
            }

            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.AutomaticChat;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.SpeakingInterval;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.MixSentences;

            if (ActionId == 2 || ActionId == 5)
                Session.SendMessage(new OpenBotActionComposer(BotUser, ActionId, BotSpeech));
        }
    }
}