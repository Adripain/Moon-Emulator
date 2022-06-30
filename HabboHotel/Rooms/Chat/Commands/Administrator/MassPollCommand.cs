using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Polls;
using Moon.HabboHotel.Rooms.Polls;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class MassPollCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_13"; }
        }

        public string Parameters
        {
            get { return "[ID]"; }
        }

        public string Description
        {
            get { return "Envia uma poll a todo o hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor introduza o ID da Poll que quer realizar.", 34);
                return;
            }

            RoomPoll poll = null;
            if (MoonEnvironment.GetGame().GetPollManager().TryGetPollForHotel(int.Parse(Params[1]), out poll))
            {
                if (poll.Type == RoomPollType.Poll)
                {
                    MoonEnvironment.GetGame().GetClientManager().SendMessage(new PollOfferComposer(poll));
                }
            }
            return;
        }
    }
}
