using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class EmptyItems : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[YES]"; }
        }

        public string Description
        {
            get { return "Limpa seu inventário."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(new RoomNotificationComposer("Limpar inventario:", "<font color='#B40404'><b>ATENÇÃO!</b></font>\n\n<font size=\"11\" color=\"#1C1C1C\">O comando de limpar inventário limpa todos seus mobis.\n" +
                 "Para confirmar, digite <font color='#B40404'> <b> :empty yes</b></font>. \n\nUma vez que digitado, não poderá voltar atras.\n\n<font color='#B40404'><i>Se não deseja limpar seu inventario, ignore esta mensagem.</i></font>\n\n" +
                 "Supondo que tenha mais de 3000 mobis, aqueles não visiveis no seu inventario tambem serão deletados.", "builders_club_room_locked", ""));
                return;
            }
            else
            {
                if (Params.Length == 2 && Params[1].ToString() == "yes")
                {
                    Session.GetHabbo().GetInventoryComponent().ClearItems();
                    Session.SendNotification("Seu inventario foi limpo corretamente.");
                    return;
                }
                else if (Params.Length == 2 && Params[1].ToString() != "yes")
                {
                    Session.SendNotification("Para confirmar escreva, :empty yes.");
                    return;
                }
            }
        }
    }
}
