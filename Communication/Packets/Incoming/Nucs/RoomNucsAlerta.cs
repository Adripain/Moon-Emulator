using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Communication.Packets.Outgoing.Nux;
using Moon.Communication.Packets.Outgoing;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.Nucs
{
    class RoomNucsAlerta : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            var habbo = Session.GetHabbo();

            if (habbo == null) return;

            if (!habbo.PassedNuxNavigator && !habbo.PassedNuxCatalog && !habbo.PassedNuxItems && !habbo.PassedNuxMMenu && !habbo.PassedNuxChat && !habbo.PassedNuxCredits && !habbo.PassedNuxDuckets)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/Este é o navegador. Use-o para explorar os quartos no " + MoonEnvironment.HotelName + "."));
                habbo.PassedNuxNavigator = true;
            }

            else if (habbo.PassedNuxNavigator && !habbo.PassedNuxCatalog && !habbo.PassedNuxItems && !habbo.PassedNuxMMenu && !habbo.PassedNuxChat && !habbo.PassedNuxCredits && !habbo.PassedNuxDuckets)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_CATALOGUE/Esta é a loja. Aqui você encontrará elementos impressionantes e únicos que fazem o " + MoonEnvironment.HotelName + " ser ainda mais divertido. Experimente!"));
                habbo.PassedNuxCatalog = true;
            }

            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && !habbo.PassedNuxItems && !habbo.PassedNuxMMenu && !habbo.PassedNuxChat && !habbo.PassedNuxCredits && !habbo.PassedNuxDuckets)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_INVENTORY/Este é o inventário.Para colocar os seus mobis, você só tem que arrastá-los para o chão."));
                habbo.PassedNuxItems = true;
            }

            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxItems && !habbo.PassedNuxMMenu && !habbo.PassedNuxChat && !habbo.PassedNuxCredits && !habbo.PassedNuxDuckets)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/MEMENU_CLOTHES/Aqui estão os ajustes. Você pode trocar de roupa e modificar aspectos do seu personagem."));
                habbo.PassedNuxMMenu = true;
            }

            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxItems && habbo.PassedNuxMMenu && !habbo.PassedNuxChat && !habbo.PassedNuxCredits && !habbo.PassedNuxDuckets)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/CHAT_INPUT/Fale com os outros " + MoonEnvironment.HotelName + "'s escrevendo aqui."));
                habbo.PassedNuxChat = true;
            }

            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxItems && habbo.PassedNuxMMenu && habbo.PassedNuxChat && !habbo.PassedNuxCredits && !habbo.PassedNuxDuckets)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/CREDITS_BUTTON/Nesta seção você pode ver a quantidade de créditos que você tem."));
                habbo.PassedNuxCredits = true;
            }

            else if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxItems && habbo.PassedNuxMMenu && habbo.PassedNuxChat && habbo.PassedNuxCredits && !habbo.PassedNuxDuckets)
            {
                Session.SendMessage(new NuxAlertComposer("helpBubble/add/DIAMONDS_BUTTON/Nesta seção você pode ver os diamantes  do " + MoonEnvironment.HotelName + ", que você terá que ganhar."));
                habbo.PassedNuxDuckets = true;
                string figure = Session.GetHabbo().Look;
                MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("fig/" + figure, "O usuário " + Session.GetHabbo().Username + " se registrou e passou pelo Nux Alerts no " + MoonEnvironment.HotelName + ".", ""));

            }

            if (habbo.PassedNuxNavigator && habbo.PassedNuxCatalog && habbo.PassedNuxItems && habbo.PassedNuxMMenu && habbo.PassedNuxChat && habbo.PassedNuxCredits && habbo.PassedNuxDuckets)
            {
                habbo._NUX = false;
                using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.runFastQuery("UPDATE users SET nux_user = 'false' WHERE id = " + Session.GetHabbo().Id + ";");
            }
        }
    }
}