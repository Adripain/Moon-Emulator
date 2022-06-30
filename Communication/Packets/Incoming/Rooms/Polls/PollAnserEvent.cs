//using Moon.Utilities;
//using Moon.HabboHotel.Rooms;
//using Moon.Database.Interfaces;
//using Moon.HabboHotel.GameClients;
//using Moon.HabboHotel.Rooms.Polls;
//using Moon.Communication.Packets.Outgoing.Inventory.Purse;

//namespace Moon.Communication.Packets.Incoming.Rooms.Polls
//{
//    class PollAnswerEvent : IPacketEvent
//    {
//        public void Parse(GameClient session, ClientPacket packet)
//        {
//            if (!session.GetHabbo().InRoom)
//                return;

//            Room room = session.GetHabbo().CurrentRoom;
//            if (room == null)
//                return;

//            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
//            if (user == null)
//                return;

//            RoomPoll poll = null;
//            if (!MoonEnvironment.GetGame().GetPollManager().TryGetPoll(packet.PopInt(), out poll))
//                return;

//            RoomPollQuestion question = null;
//            if (!poll.Questions.TryGetValue(packet.PopInt(), out question))
//                return;

//            string answer = "";
//            for (int i = 0; i < packet.PopInt(); ++i)
//            {
//                answer = answer + ":" + packet.PopString();
//            }

//            answer = answer.Substring(1);


//            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
//            {
//                dbClient.SetQuery("INSERT INTO `user_room_poll_results` (`user_id`,`poll_id`,`question_id`,`answer`,`timestamp`) VALUES (@uid,@pid,@qid,@answer,@timestamp);");
//                dbClient.AddParameter("uid", session.GetHabbo().Id);
//                dbClient.AddParameter("pid", poll.Id);
//                dbClient.AddParameter("qid", question.Id);
//                dbClient.AddParameter("answer", answer);
//                dbClient.AddParameter("timestamp", UnixTimestamp.GetNow());
//                dbClient.RunQuery();
//            }

//            if (question.SeriesOrder >= poll.LastQuestionId)
//            {
//                session.GetHabbo().GetPolls().TryAdd(poll.Id);

//                if (!string.IsNullOrEmpty(poll.BadgeReward))
//                {
//                    if (!session.GetHabbo().GetBadgeComponent().HasBadge(poll.BadgeReward))
//                        session.GetHabbo().GetBadgeComponent().GiveBadge(poll.BadgeReward, true, session);
//                }

//                if (poll.CreditReward > 0)
//                {
//                    session.GetHabbo().Credits += poll.CreditReward;
//                    session.SendMessage(new CreditBalanceComposer(session.GetHabbo().Credits));
//                }

//                if (poll.PixelReward > 0)
//                {
//                    session.GetHabbo().Duckets += poll.PixelReward;
//                    session.SendMessage(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, poll.PixelReward));
//                }
//            }
//        }
//    }
//}