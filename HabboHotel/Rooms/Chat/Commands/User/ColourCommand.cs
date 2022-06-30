using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Database.Interfaces;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class ColourCommand : IChatCommand
    {

        public string PermissionRequired => "user_normal";
        public string Parameters => "";
        public string Description => "off/red/green/blue/cyan/purple";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(new RoomNotificationComposer("Lista de cores:",
                     "<font color='#FF8000'><b>LISTA DE CORES:</b>\n" +
                     "<font size=\"12\" color=\"#1C1C1C\">O comando :color permite alterar a cor do chat, para escolher sua cor deverá usar o comando, como por exemplo:\r\r" +
                     "<font size =\"11\" color=\"#FE2E2E\"><b>:color red</b> » Bem-vindos ao inferno</font>\r\n" +
                     "<font size =\"11\" color=\"#8904B1\"><b>:color purple</b> » Um toque de glamour</font>\r\n" +
                     "<font size =\"11\" color=\"#2ECCFA\"><b>:color cyan</b> » Aquele muro cinza</font>\r\n" +
                     "<font size =\"11\" color=\"#0174DF\"><b>:color blue</b> » Tão belo quanto o mar..</font>\r\n" +
                     "<font size =\"11\" color=\"#31B404\"><b>:color green</b> » Junte-se ao movimento ecologico</font>\r\n" +
                     "", "", ""));
                return;
            }
            string chatColour = Params[1];
            string Colour = chatColour.ToUpper();
            switch (chatColour)
            {
                case "none":
                case "black":
                case "off":
                    Session.GetHabbo().chatColour = "";
                    Session.SendWhisper("Você desativou sua cor de chat", 34);
                    break;
                case "blue":
                case "red":
                case "green":
                case "cyan":
                case "purple":
                case "yellow":
                case "orange":
                    Session.GetHabbo().chatColour = chatColour;
                    Session.SendWhisper("@" + chatColour + "@Sua cor de chat foi ativada: " + chatColour + "", 34);
                    using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("UPDATE `users` SET `bubble_color` = '" + chatColour + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }
                    break;
                default:
                    Session.SendWhisper("A cor de chat: " + Colour + " não existe!", 34);
                    break;
            }
            return;
        }
    }
}