using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Users;
using Moon.HabboHotel.Support;


using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Moderation;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class ModerationBanEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_soft_ban"))
                return;

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            double Length = (Packet.PopInt() * 3600) + MoonEnvironment.GetUnixTimestamp();
            string Unknown1 = Packet.PopString();
            string Unknown2 = Packet.PopString();
            bool IPBan = Packet.PopBoolean();
            bool MachineBan = Packet.PopBoolean();

            if (MachineBan)
                IPBan = false;

            Habbo Habbo = MoonEnvironment.GetHabboById(UserId);

            if (Habbo == null)
            {
                Session.SendWhisper("Ocurrio un error mientras se buscaba este usuario en la base de datos");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_tool") && !Session.GetHabbo().GetPermissions().HasRight("mod_ban_any"))
            {
                Session.SendWhisper("Oops, no se puede banear a este usuario");
                return;
            }

            Message = (Message != null ? Message : "sin razon.");

            string Username = Habbo.Username;
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
            }

            if (IPBan == false && MachineBan == false)
                MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Message, Length);
            else if (IPBan == true)
                MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.IP, Habbo.Username, Message, Length);
            else if (MachineBan == true)
            {
                MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.IP, Habbo.Username, Message, Length);
                MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Message, Length);
                MoonEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.MACHINE, Habbo.Username, Message, Length);
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Habbo.Username);
            if (TargetClient != null)
                TargetClient.Disconnect();
        }
    }
}