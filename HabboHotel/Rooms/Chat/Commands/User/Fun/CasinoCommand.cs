using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class CasinoCommand : IChatCommand
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
            get { return "start/pl. Matenha sua conta do jogo."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, você deve especificar se quer começar modo casino ou dar pl! Escreva :casino start o :casino pl", 34);
                return;
            }
            string query = Params[1];

            RoomUser roomUser = Room?.GetRoomUserManager()?.GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUser == null)
            {
                return;
            }

            List<Items.Item> userBooth = Room.GetRoomItemHandler().GetFloor.Where(x => x != null && Gamemap.TilesTouching(
                x.Coordinate, roomUser.Coordinate) && x.Data.InteractionType == Items.InteractionType.DICE).ToList();

            if (userBooth.Count != 5)
            {
                Session.SendWhisper("Você deve ter 5 dados para começar um jogo", 34);
                return;
            }

            if (query == "pl" || query == "PL")
            {
                Room.SendMessage(RoomNotificationComposer.SendBubble("ganador", "O usuario " + Session.GetHabbo().Username + " tirou " + Session.GetHabbo().casinoCount + " nos dados (PL Automatico)", ""));
                Session.GetHabbo().casinoEnabled = false;
                Session.GetHabbo().casinoCount = 0;
            }
            else if (query == "start" || query == "START")
            {
                Session.SendWhisper("Você iniciou o modo casino. O contador de dados está ativado", 34);
                Session.GetHabbo().casinoEnabled = true;

            }
            else
            {
                Session.SendWhisper("Oops, você deve especificar se quer começar modo casino ou dar pl! Escreva :casino start o :casino pl");
            }


        }
    }
}