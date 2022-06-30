using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveRoom : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[CANTIDAD]";
        public string Description => "Dar créditos a todos.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre del identificador que le gustaría dar a la habitación.");
                return;
            }
            int Amount;
            if (int.TryParse(Params[1], out Amount))

                foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
                {
                    if (RoomUser == null || RoomUser.GetClient() == null || Session.GetHabbo().Id == RoomUser.UserId)
                        continue;
                    RoomUser.GetClient().GetHabbo().Credits += Amount;
                    RoomUser.GetClient().SendMessage(new CreditBalanceComposer(RoomUser.GetClient().GetHabbo().Credits));
                }
        }
    }
}
