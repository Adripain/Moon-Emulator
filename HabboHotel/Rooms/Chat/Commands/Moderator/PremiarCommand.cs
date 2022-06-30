using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Inventory.Purse;
using Moon.Database.Interfaces;
using System.Data;
using Moon.Communication.Packets.Outgoing.Users;
using Moon.HabboHotel.Quests;
using Moon.Core;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class PremiarCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_12"; }
        }

        public string Parameters
        {
            get { return "[USUARIO]"; }
        }

        public string Description
        {
            get { return "Todas as funções para recompensar um vencedor do evento."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o usuário que deseja recompensar!", 34);
                return;
            }

            GameClient Target = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Opa, não foi possível encontrar esse usuário!", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Target.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Usuário não encontrado! Talvez ele não esteja online ou neste quarto.", 34);
                return;
            }

            if (Target.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Você não pode se recompensar!", 34);
                return;
            }

            // Comando editaveu abaixo mais cuidado pra não faze merda

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }
            else
            {
                Target.GetHabbo().Diamonds += Convert.ToInt32(MoonEnvironment.GetConfig().data["Diamantespremiar"]);
                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, 1, 5));

                Session.SendMessage(RoomNotificationComposer.SendBubble("moedas", "Você ganhou " + Convert.ToInt32(MoonEnvironment.GetConfig().data["Diamantespremiar"]) + " Diamante(s)! Parabéns " + Target.GetHabbo().Username + "!", ""));

                if (Target.GetHabbo().Rank >= 0)
                {
                    DataRow dFurni = null;
                    using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        //BUscame WMTOTEM
                        dbClient.SetQuery("SELECT public_name FROM furniture WHERE id = '42636366'");
                        dFurni = dbClient.getRow();
                    }
                    Target.GetHabbo().GetInventoryComponent().AddNewItem(0, 42636366, Convert.ToString(dFurni["public_name"]), 1, true, false, 0, 0);
                    Target.GetHabbo().GetInventoryComponent().UpdateItems(false);

                }

                if (Session.GetHabbo().Rank >= 0)
                {
                    DataRow nivel = null;
                    using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT premio FROM users WHERE id = '" + Target.GetHabbo().Id + "'");
                        nivel = dbClient.getRow();
                        dbClient.RunQuery("UPDATE users SET premio = premio + '1' WHERE id = '" + Target.GetHabbo().Id + "'");
                        dbClient.RunQuery("UPDATE users SET puntos_eventos = puntos_eventos + '1' WHERE id = '" + Target.GetHabbo().Id + "'");
                        dbClient.RunQuery();
                    }

                    if (Convert.ToString(nivel["premio"]) != MoonEnvironment.GetConfig().data["NiveltotalGames"])
                    {
                        string emblegama = "NV" + Convert.ToString(nivel["premio"]);

                        if (!Target.GetHabbo().GetBadgeComponent().HasBadge(emblegama))
                        {
                            Target.GetHabbo().GetBadgeComponent().GiveBadge(emblegama, true, Target);
                            if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                Target.SendMessage(RoomNotificationComposer.SendBubble("badge/" + emblegama, "Você acabou de receber um emblema de jogo de nível: " + emblegama + " !", ""));
                            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Target, "ACH_Evento", 1);
                            string figure = Target.GetHabbo().Look;
                            MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("fig/" + figure, TargetUser.GetUsername() + "ganhou um evento no hotel. Parabéns!", "Nivel do usuário: NIVEL " + Convert.ToString(nivel["premio"]) + "!"));
                        }
                        else
                            Session.SendWhisper("Ops, houve um erro no sistema de dar crachás automáticos! Erro no emblema: (" + emblegama + ") !", 34);
                        Session.SendWhisper("Comando (premiado) realizado com sucesso!", 34);
                    }
                }
            }
        }

        private void SendMessage(RoomNotificationComposer roomNotificationComposer)
        {
            throw new NotImplementedException();
        }
    }
}
