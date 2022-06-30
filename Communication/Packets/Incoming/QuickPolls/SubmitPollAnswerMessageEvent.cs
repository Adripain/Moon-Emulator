using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Communication.Packets.Outgoing.Rooms.Poll;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Rooms.Polls;
using Moon.Utilities;
using System;

namespace Moon.Communication.Packets.Incoming.QuickPolls
{
    class SubmitPollAnswerMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int pollId = Packet.PopInt();
            int questionId = Packet.PopInt();
            int count = Packet.PopInt();


            RoomPoll poll = null;

            if (Session == null || Session.GetHabbo() == null)
                return;

            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            if (questionId == -1)
            {
                String answer = Packet.PopString();
                if (room.poolQuestion != string.Empty)
                {
                    if (room.yesPoolAnswers.Contains(Session.GetHabbo().Id) || room.noPoolAnswers.Contains(Session.GetHabbo().Id))
                    {
                        return;
                    }

                    if (answer.Equals("1"))
                    {
                        room.yesPoolAnswers.Add(Session.GetHabbo().Id);
                        MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PollVote", 1);
                    }
                    else
                    {
                        room.noPoolAnswers.Add(Session.GetHabbo().Id);
                        MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PollVote", 1);
                    }

                    room.SendMessage(new QuickPollResultMessageComposer(Session.GetHabbo().Id, answer, room.yesPoolAnswers.Count, room.noPoolAnswers.Count));
                    return;
                }
            }
            else if (MoonEnvironment.GetGame().GetPollManager().TryGetPollForRoom(room.Id, out poll))
            {
                RoomPollQuestion question = null;
                if (!poll.Questions.TryGetValue(questionId, out question))
                    return;

                string answer = "";

                switch (question.Type)
                {
                    case RoomPollQuestionType.Textbox:
                        answer = "" + Packet.PopString();
                        break;

                    case RoomPollQuestionType.Radio:
                        answer = "" + Packet.PopString();
                        break;

                    case RoomPollQuestionType.Checkbox:
                        for (int i = 0; i < count; ++i)
                        {
                            answer = "" + answer + ":" + Packet.PopString();
                        }
                        break;
                }

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_room_poll_results` (`user_id`,`poll_id`,`question_id`,`answer`,`timestamp`) VALUES (@uid,@pid,@qid,@answer,@timestamp);");
                    dbClient.AddParameter("uid", Session.GetHabbo().Id);
                    dbClient.AddParameter("pid", poll.Id);
                    dbClient.AddParameter("qid", question.Id);
                    dbClient.AddParameter("answer", answer);
                    dbClient.AddParameter("timestamp", UnixTimestamp.GetNow());
                    dbClient.RunQuery();
                }

                if (question.SeriesOrder >= poll.LastQuestionId)
                {
                    Session.GetHabbo().GetPolls().TryAdd(poll.Id);

                    if (!string.IsNullOrEmpty(poll.BadgeReward))
                    {
                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(poll.BadgeReward))
                            Session.GetHabbo().GetBadgeComponent().GiveBadge(poll.BadgeReward, true, Session);
                    }

                    if (poll.CreditReward > 0)
                    {
                        Session.GetHabbo().Credits += poll.CreditReward;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }

                    if (poll.PixelReward > 0)
                    {
                        Session.GetHabbo().Duckets += poll.PixelReward;
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, poll.PixelReward));
                    }
                }
            }
            else
                return;
        }
    }
}