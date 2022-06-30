using System;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Misc;
using Moon.Communication.Packets.Outgoing.Rooms.Freeze;
using Moon.Communication.Packets.Outgoing.Rooms.Settings;
using System.Text;
using Moon.Communication.Packets.Outgoing.Handshake;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Help.Helpers;
using Moon.HabboHotel.Moderation;
using Moon.Utilities;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using System.Data;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User
{
    class InfoCommand : IChatCommand
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
            get { return "Informações do "+Convert.ToString(MoonEnvironment.GetConfig().data["hotel.name"])+"."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - MoonEnvironment.ServerStarted;
            int OnlineUsers = MoonEnvironment.GetGame().GetClientManager().Count;
            int RoomCount = MoonEnvironment.GetGame().GetRoomManager().Count;
            DataRow Items = null, rooms = null, users = null;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT count(id) FROM users");
                users = dbClient.getRow();
                dbClient.SetQuery("SELECT count(id) FROM users WHERE DATE(FROM_UNIXTIME(account_created)) = CURDATE()");
                Items = dbClient.getRow();
                dbClient.SetQuery("SELECT count(id) FROM rooms");
                rooms = dbClient.getRow();
            }
            Session.SendMessage(new RoomNotificationComposer("MOON Server",
                "<font color='#0D0106'><b>Sobre o MOON</b>\n" +
                "<font size=\"11\" color=\"#1C1C1C\">MOON Server é um server bem estável e</font>" +
                "<font size=\"11\" color=\"#1C1C1C\"> atualizado para maior entreterimento da comunidade.\n\n" +
                "<font size =\"12\" color=\"#0B4C5F\"><b>Estatisticas:</b></font>\n" +
                "<font size =\"11\" color=\"#1C1C1C\">  <b> · Usuarios conectados</b>: " + OnlineUsers + "\r" +
                "  <b> · Salas</b>: " + RoomCount + "\r" +
                "  <b> · Tempo  online</b>: " + Uptime.Days + " dias e " + Uptime.Hours + " horas.\r" +
                "  <b> · data de hoje</b>: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + ".\n\n" +
                "<font size =\"12\" color=\"#0B4C5F\"><b> Recorde de:</b></font>\n" +
                "  <b> · Usuarios conectados</b>: " + Game.SessionUserRecord + "\r" +
                "  <b> · Usuarios registrados</b>: " + users[0] + "\r" +
                "  <b> · Registrados HOJE</b>: " + Items[0] + "\r" +
                "  <b> · Salas criadas</b>:  " + rooms[0] + ".</font>\n\n" +
                "  <b> · Desenvolvido por: KaioVDEV</b>.\n\n", "MOON"));
        }
    }
}