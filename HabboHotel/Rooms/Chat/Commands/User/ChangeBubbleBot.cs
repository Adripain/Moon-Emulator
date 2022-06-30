using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Bots;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class BubbleBotCommand : IChatCommand
    {
        public string PermissionRequired => "user_vip";
        public string Parameters => "[BOTNOME] [BOLHAID]";
        public string Description => "Troque a bolha do seu bot.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            string BotName = CommandManager.MergeParams(Params, 1);
            string Bubble = CommandManager.MergeParams(Params, 2);
            int BubbleID = 0;

            long nowTime = MoonEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000 && Session.GetHabbo().Rank == 1)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("abuse", "Espera 1 minuto para voltarr a mudar a bolha do  tu Bot.\n\nCompra la suscripción VIP de "+MoonEnvironment.HotelName+" haciendo click aquí para evitar esta espera.", "catalog/open/clubVIP"));
                return;
            }

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Você não colocou o nome do bot válido.", 34);
                return;
            }

            RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Params[1]);
            if (Bot == null)
            {
                Session.SendWhisper("Não há nenhum boto chamado "+ Params[1] +" no quarto.", 34);
                return;
            }

            if (Bot.BotData.ownerID != Session.GetHabbo().Id)
                {
                Session.SendWhisper("Você está mudando a bolha de um bot que não é seu, crack, máquina, figura.", 34);
                return;
                }

            if (Bubble == "1" || Bubble == "23" || Bubble == "34" || Bubble == "37")
            {
                Session.SendWhisper("Você está colocando uma bolha proibida.");
                return;
            }

                if (Params.Length == 2)
                {
                    Session.SendWhisper("Uy, Você esqueceu de inserir uma ID de bolha.", 34);
                    return;
                }
                
                if (int.TryParse(Bubble, out BubbleID))
                {
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("UPDATE `bots` SET `chat_bubble` =  '" + BubbleID + "' WHERE `name` =  '" + Bot.BotData.Name + "' AND  `room_id` =  '" + Session.GetHabbo().CurrentRoomId + "'");
                        Bot.Chat("Você colocou uma bolha " + BubbleID + ".", true, BubbleID);
                        Bot.BotData.ChatBubble = BubbleID;
                }
                }

            return;
        }
    }
}