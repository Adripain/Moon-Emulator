using Moon.HabboHotel.Rooms.Chat.Styles;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class SetBetCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[DIAMANTES]"; }
        }

        public string Description
        {
            get { return "Defina quantos diamantes você deseja apostar nos slots."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve inserir um valor em diamantes, por exemplo :apostar 50.", 34);
                return;
            }

            int Bet = 0;
            if (!int.TryParse(Params[1].ToString(), out Bet))
            {
                Session.SendWhisper("Por favor insira um número válido.", 34);
                return;
            }

            Session.GetHabbo()._bet = Bet;
            Session.SendWhisper("Você definiu suas apostas para " + Bet + " diamantes. Aposte com cabeça!", 34);
        }
    }
}