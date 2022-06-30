using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moon.Database.Interfaces;
using System.Data;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class OnDutyCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_vip"; }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get { return "Ve los usarios conectados ahora mismo."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
                using (IQueryAdapter Adapter = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    Adapter.SetQuery("SELECT username FROM users WHERE online = '1'");
                    Adapter.RunQuery();

                    DataTable Table = Adapter.getTable();

                    StringBuilder List = new StringBuilder("");
                    int OnlineUsers = MoonEnvironment.GetGame().GetClientManager().Count;
                    List.AppendLine("Usarios conectados: " + OnlineUsers);
                    if (Table != null)
                    {
                        foreach (DataRow Row in Table.Rows)
                        {
                            List.AppendLine(Row["Username"].ToString());
                        }
                    }
                    Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                    return;
                }
            }
        }
    }