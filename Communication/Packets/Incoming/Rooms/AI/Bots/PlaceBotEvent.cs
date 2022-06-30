﻿using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users.Inventory.Bots;
using Moon.Communication.Packets.Outgoing.Inventory.Bots;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Rooms.AI.Speech;
using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Rooms.AI.Responses;

namespace Moon.Communication.Packets.Incoming.Rooms.AI.Bots
{
    class PlaceBotEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            int BotId = Packet.PopInt();
            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false) || !Room.GetGameMap().ValidTile(X, Y))
            {
                Session.SendNotification("No puedes colocar al Bot aqui!");
                return;
            }

            Bot Bot = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryGetBot(BotId, out Bot))
                return;

            int BotCount = 0;
            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                    continue;

                BotCount += 1;
            }

            if (BotCount >= 5 && !Session.GetHabbo().GetPermissions().HasRight("bot_place_any_override"))
            {
                Session.SendNotification("Lo siento, solo son un maximo de 5 bot por sala!");
                return;
            }

            //TODO: Hmm, maybe not????
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `room_id` = '" + Room.RoomId + "', `x` = @CoordX, `y` = @CoordY WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", Bot.Id);
                dbClient.AddParameter("CoordX", X);
                dbClient.AddParameter("CoordY", Y);
                dbClient.RunQuery();
            }

            List<RandomSpeech> BotSpeechList = new List<RandomSpeech>();

            //TODO: Grab data?
            DataRow GetData = null;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `ai_type`,`rotation`,`walk_mode`,`automatic_chat`,`speaking_interval`,`mix_sentences`,`chat_bubble` FROM `bots` WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", Bot.Id);
                GetData = dbClient.getRow();

                dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = @BotId");
                dbClient.AddParameter("BotId", Bot.Id);
                DataTable BotSpeech = dbClient.getTable();

                foreach (DataRow Speech in BotSpeech.Rows)
                {
                    BotSpeechList.Add(new RandomSpeech(Convert.ToString(Speech["text"]), Bot.Id));
                }
            }

            RoomUser BotUser = Room.GetRoomUserManager().DeployBot(new RoomBot(Bot.Id, Session.GetHabbo().CurrentRoomId, Convert.ToString(GetData["ai_type"]), Convert.ToString(GetData["walk_mode"]), Bot.Name, "", Bot.Figure, X, Y, 0, 4, 0, 0, 0, 0, ref BotSpeechList, "", 0, Bot.OwnerId, MoonEnvironment.EnumToBool(GetData["automatic_chat"].ToString()), Convert.ToInt32(GetData["speaking_interval"]), MoonEnvironment.EnumToBool(GetData["mix_sentences"].ToString()), Convert.ToInt32(GetData["chat_bubble"])), null);
            BotUser.Chat("¡Hola " + Session.GetHabbo().Username + "!", false, 0);

            Room.GetGameMap().UpdateUserMovement(new System.Drawing.Point(X,Y), new System.Drawing.Point(X, Y), BotUser);


            Bot ToRemove = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryRemoveBot(BotId, out ToRemove))
            {
                Console.WriteLine("Error whilst removing Bot: " + ToRemove.Id);
                return;
            }
            Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
        }
    }
}
