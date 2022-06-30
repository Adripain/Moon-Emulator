using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class WiredVariable : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Lista de Wireds :."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
           
            Session.SendMessage(new MassEventComposer("habbopages/chat/wiredvars.txt"));
            return;

        }
    }
}
