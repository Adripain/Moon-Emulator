using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Users;

namespace Moon.Communication.Packets.Incoming.Moderation
{
    class ModerationMuteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_mute"))
                return;

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            double Length = (Packet.PopInt() * 60);
            string Unknown1 = Packet.PopString();
            string Unknown2 = Packet.PopString();

            Habbo Habbo = MoonEnvironment.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocurrio un error mientras se realiza la busqueda de este usuario en la DB");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_mute") && !Session.GetHabbo().GetPermissions().HasRight("mod_mute_any"))
            {
                Session.SendWhisper("Oops, No puede silenciar a este usuario");
                return;
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `time_muted` = '" + Length + "' WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
            }

            if (Habbo.GetClient() != null)
            {
                Habbo.TimeMuted = Length;
                Habbo.GetClient().SendNotification("Usted ha sido silenciado por tener un mal comportamiento en el Hotel. Cómportese para evitar futuras sanciones");
                //Habbo.GetClient().SendNotification("usted ha sido silenciado por un moderador por  " + Length + " segundos!");
            }
        }
    }
}

