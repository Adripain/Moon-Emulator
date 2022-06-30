using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;

using Moon.HabboHotel.Users.Inventory.Bots;
using Moon.Communication.Packets.Outgoing.Inventory.Bots;

using Moon.Database.Interfaces;


namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class KickBotsCommand : IChatCommand
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
            get { return "Expulsa todos os BOTS dentro do quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, ao que parece o dono da sala não permite que usem este comando");
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                    continue;

                RoomUser BotUser = null;
                if (!Room.GetRoomUserManager().TryGetBot(User.BotData.Id, out BotUser))
                    return;

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", User.BotData.Id);
                    dbClient.RunQuery();
                }

                Session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(Convert.ToInt32(BotUser.BotData.Id), Convert.ToInt32(BotUser.BotData.ownerID), BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender));
                Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
            }

            Session.SendWhisper("Todos os bots foram removidos.");
        }
    }
}
