using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class SetMaxCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[NUMERO]"; }
        }

        public string Description
        {
            get { return "Aumenta ou reduz o numero maximo em seu quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Insira uma quantia inteira (em números) de quantos usuários podem entrar no seu quarto.", 34);
                return;
            }

            int MaxAmount;
            if (int.TryParse(Params[1], out MaxAmount))
            {
                if (MaxAmount <= 0)
                {
                    MaxAmount = 10;
                    Session.SendWhisper("Quantidade de usuários muito baixa, a quantidade de usuários foi colocada para 10.", 34);
                }
                else if (MaxAmount > 250 && !Session.GetHabbo().GetPermissions().HasRight("override_command_setmax_limit"))
                {
                    MaxAmount = 250;
                    Session.SendWhisper("Quantidade de usuários muito alta, a quantidade de usuários foi colocada para 250.", 34);
                }
                else

                    Room.UsersMax = MaxAmount;
                Room.RoomData.UsersMax = MaxAmount;
                Room.SendMessage(RoomNotificationComposer.SendBubble("setmax", "" + Session.GetHabbo().Username + " estabeleceu um limite de uses para " + MaxAmount + ".", ""));

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `rooms` SET `users_max` = " + MaxAmount + " WHERE `id` = '" + Room.Id + "' LIMIT 1");
                }
            }
            else
                Session.SendWhisper("Quantidade invalida, só é permitido numeros.", 34);
        }
    }
}
