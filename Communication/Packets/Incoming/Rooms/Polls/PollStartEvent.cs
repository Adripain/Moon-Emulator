using Moon.HabboHotel.Rooms.Polls;
using Moon.Communication.Packets.Outgoing.Rooms.Polls;

namespace Moon.Communication.Packets.Incoming.Rooms.Polls
{
    class PollStartEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            int pollId = packet.PopInt();

            RoomPoll poll = null;
            if (!MoonEnvironment.GetGame().GetPollManager().TryGetPoll(pollId, out poll))
                return;

            session.SendMessage(new PollContentsComposer(poll));
        }
    }
}
