using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Handshake;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class FlagUserCommand : IChatCommand
    {
        public string PermissionRequired => "user_11";
        public string Parameters => "[USUARIO]";
        public string Description => "Renomear um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome do usuário para quem você quer mudar o nome", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um problema ao procurar o usuário ou talvez não esteja online", 34);
                return;
            }

            else if (TargetClient.GetHabbo()._changename != 1)
            {
                Session.SendNotification("O usuário " + TargetClient.GetHabbo().Username + " você não pode receber a mudança de nome, porque você já esgotou a mudança permitida.");
                TargetClient.SendNotification("Oops, uma das nossas equipes tentou mudar o seu nome, mas desde que você mudou a menos de um mês atrás, não podemos prosseguir com a sua mudança, se desejar, você pode comprar uma mudança adicional dentro do catálogo");
                return;
            }


            else if (TargetClient.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Você não pode escolher um nome.", 34);
                return;
            }
            else
            {
                TargetClient.GetHabbo().LastNameChange = 0;
                TargetClient.GetHabbo().ChangingName = true;
                TargetClient.SendNotification("Por favor, foi determinado que seu nome de usuário não está correto ou é inadequado\r\rUma equipe decidiu dar a você a oportunidade de mudar seu nome, para evitar a expulsão do hotel.\r\rFechar esta janela, e você clica em si mesmo e você terá a opção de mudar seu nome, Mudar isso! \n\n <b><u>Lembre-se que você só tem uma mudança de nome, pense bem antes de escolher</ b></u>");
                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE users SET changename = '0' WHERE id = " + TargetClient.GetHabbo().Id + "");
                }
                TargetClient.GetHabbo()._changename = 0;
                TargetClient.SendMessage(new UserObjectComposer(TargetClient.GetHabbo()));
            }

        }
    }
}
