using System.Collections.Generic;
using System.Linq;
using Moon.Communication.Packets.Outgoing.Messenger;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class GroupChatCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "Delete seu chat de grupo.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
                if (Params.Length < 2)
                {
                    Session.SendWhisper("Ocorreu um erro, tente deletar", 34);
                    return;
                }

                if (!Room.CheckRights(Session, true))
                {
                    Session.SendWhisper("No tienes permisos.", 34);
                    return;
                }

                if (Room.Group == null)
                {
                    Session.SendWhisper("Esta sala não tem grupo, se acabou de criar um digite :unload", 34);
                    return;
                }

                var mode = Params[1].ToLower();
                var group = Room.Group;

                if (mode == "borrar")
                {
                    if (group.HasChat == false)
                    {
                        Session.SendWhisper("Este grupo não tem chat ainda.", 34);
                        return;
                    }

                    using (var adap = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        adap.SetQuery("UPDATE groups SET has_chat = '0' WHERE id = @id");
                        adap.AddParameter("id", group.Id);
                        adap.RunQuery();
                    }
                    group.HasChat = false;
                    List<GameClient> GroupMembers = (from Client in MoonEnvironment.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null select Client).ToList();
                    foreach (GameClient Client in GroupMembers)
                    {
                        if (Client != null)
                            continue;
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