using Moon;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Database.Interfaces;
using System.Data;
using System.Text;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class VerClonesCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_10"; }
        }

        public string Parameters
        {
            get { return "[USUARIO]"; }
        }

        public string Description
        {
            get { return "Ver clones."; }
        }

        public void Execute(GameClients.GameClient session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                session.SendWhisper("Por favor, digite o nome do usuário para revisar.");
                return;
            }

            string str2;
            IQueryAdapter adapter;
            string username = Params[1];
            DataTable table = null;
            StringBuilder builder = new StringBuilder();
            if (MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(username) != null)
            {
                str2 = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(username).GetConnection().getIp();
                builder.AppendLine("Username :  " + username + " - Ip : " + str2);
                using (adapter = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery("SELECT id,username FROM users WHERE ip_last = @ip OR ip_reg = @ip");
                    adapter.AddParameter("ip", str2);
                    table = adapter.getTable();
                    builder.AppendLine("Clones encontrados: " + table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        builder.AppendLine(string.Concat(new object[] { "Id : ", row["id"], " - Username: ", row["username"] }));
                    }
                }
                session.SendMessage(new MOTDNotificationComposer(builder.ToString()));
            }
            else
            {
                using (adapter = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery("SELECT ip_last FROM users WHERE username = @username");
                    adapter.AddParameter("username", username);
                    str2 = adapter.getString();
                    builder.AppendLine("Username :  " + username + " - Ip : " + str2);
                    adapter.SetQuery("SELECT id,username FROM users WHERE ip_last = @ip OR ip_reg = @ip");
                    adapter.AddParameter("ip", str2);
                    table = adapter.getTable();
                    builder.AppendLine("Clones encontrados: " + table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        builder.AppendLine(string.Concat(new object[] { "Id : ", row["id"], " - Username: ", row["username"] }));
                    }
                }

                session.SendMessage(new MOTDNotificationComposer(builder.ToString()));
            }
            return;
        }
    }
}