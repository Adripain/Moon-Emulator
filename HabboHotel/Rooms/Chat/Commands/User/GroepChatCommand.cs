using Moon;
using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Messenger;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class GroepChatCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_groepchat"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ativa os grupos ON/OFF"; }
        }


        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Ocorreu um erro, escolha ON / OFF");
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Você não tem permissão.");
                return;
            }

            if (Room.Group == null)
            {
                Session.SendWhisper("Está sala não tem grupo, se você acabou de criar um esreva :unload");
                return;
            }

            if (Room.Group.Id != Session.GetHabbo().GetStats().FavouriteGroupId)
            {
                Session.SendWhisper("Só pode criar um grupo se você o tiver como favorito.");
                return;
            }

            var mode = Params[1].ToLower();
            var group = Room.Group;

            if (mode == "on")
            {
                if (group.HasChat)
                {
                    Session.SendWhisper("Este grupo já tem chat.");
                    return;
                }

                group.HasChat = true;

                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE groups SET has_chat = '1' WHERE id = @id");
                    dbClient.AddParameter("id", group.Id);
                    dbClient.RunQuery();
                }

                var Client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().Id);
                if (Client != null)
                {
                    Client.SendMessage(new FriendListUpdateComposer(group, 1));
                }

            }
            else if (mode == "off")
            {
                if (!group.HasChat)
                {
                    Session.SendWhisper("Este grupo não tem chat ainda.");
                    return;
                }

                group.HasChat = false;

                using (var adap = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    adap.SetQuery("UPDATE groups SET has_chat = '0' WHERE id = @id");
                    adap.AddParameter("id", group.Id);
                    adap.RunQuery();
                }
                var Client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().Id);
                if (Client != null)
                {
                    Client.SendMessage(new FriendListUpdateComposer(group, -1));
                }
            }
            else
            {
                Session.SendNotification("Ocorreu um erro");
            }


        }
    }
}
