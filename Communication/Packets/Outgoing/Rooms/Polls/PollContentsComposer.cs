using Moon.HabboHotel.Rooms.Polls;

namespace Moon.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollContentsComposer : ServerPacket
    {
        public PollContentsComposer(RoomPoll poll)
            : base(ServerPacketHeader.PollContentsMessageComposer)
        {
            base.WriteInteger(poll.Id);
            base.WriteString(poll.Headline);
            base.WriteString(poll.CompletionMessage);

            base.WriteInteger(poll.Questions.Count);
            foreach (RoomPollQuestion question in poll.Questions.Values)
            {
                base.WriteInteger(question.Id);
                base.WriteInteger(question.SeriesOrder);
                base.WriteInteger(RoomPollQuestionTypeUtility.GetQuestionType(question.Type));
                base.WriteString(question.Question);

                base.WriteInteger(0); // ??
                base.WriteInteger(question.MinimumSlections);// Min selections

                base.WriteInteger(question.Selections.Count);
                foreach (RoomPollQuestionSelection Selection in question.Selections.Values)
                {
                    base.WriteString(Selection.Value);
                    base.WriteString(Selection.Text);
                    base.WriteInteger(Selection.Id);
                }

                base.WriteInteger(0);//??
            }
            base.WriteBoolean(true);//No idea
        }
    }
}