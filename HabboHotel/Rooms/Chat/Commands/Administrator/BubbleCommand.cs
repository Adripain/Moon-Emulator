using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Rooms.Chat.Styles;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class BubbleCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }

        public string Parameters
        {
            get { return "[ID]"; }
        }

        public string Description
        {
            get { return "Altere a bolha do chat de um usuário"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, você não inseriu o ID", 34);
                return;
            }

            int Bubble = 0;
            if (!int.TryParse(Params[1].ToString(), out Bubble))
            {
                Session.SendWhisper("Por favor insira um número valido.", 34);
                return;
            }

            ChatStyle Style = null;
            if (!MoonEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Bubble, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
            {
                Session.SendWhisper("Oops, não pode usar isso por causa do bloqueio de Ranks [ Raros: 32, 28]!", 34);
                return;
            }

            User.LastBubble = Bubble;
            Session.GetHabbo().CustomBubbleId = Bubble;
            Session.SendWhisper("Bolha escolhida como: " + Bubble);
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `users` SET `bubble_id` = '" + Bubble + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }
        }
    }
}