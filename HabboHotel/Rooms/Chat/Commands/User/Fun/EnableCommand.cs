using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Rooms.Games;
using Moon.HabboHotel.Rooms.Games.Teams;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class EnableCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "[EFECTO ID]";
        public string Description => "Habilitar um efeito em seu personagem.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve escrever um ID do efeito", 34);
                return;
            }

            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
                return;

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("Você não pode ativar um efeito montado num cavalo", 34);
                return;
            }
            else if (ThisUser.Team != TEAM.NONE)
                return;
            else if (ThisUser.isLying)
                return;

            int EffectId = 0;
            if (!int.TryParse(Params[1], out EffectId))
                return;

            if (EffectId > int.MaxValue || EffectId < int.MinValue)
                return;

            // VIP Effect - VIP 2
            if (EffectId == 44 && Session.GetHabbo().Rank < 2 || EffectId == 593 && Session.GetHabbo().Rank < 2 || EffectId == 604 && Session.GetHabbo().Rank < 2 || EffectId == 605 && Session.GetHabbo().Rank < 2 || EffectId == 606 && Session.GetHabbo().Rank < 2 || EffectId == 607 && Session.GetHabbo().Rank < 2 || EffectId == 608 && Session.GetHabbo().Rank < 2)
            { Session.SendWhisper("Desculpe, você não é VIP, então você não pode usar esse efeito.", 34); return; }

            //PUBLI - Publicistas 5
            if (EffectId == 601 && Session.GetHabbo().Rank < 5)
            {
                Session.SendWhisper("Desculpe, você não é um publicista, então você não pode usar esse efeito.", 34);
                return;
            }

            //SUPER STAR 6
            if (EffectId == 598 && Session.GetHabbo().Rank < 6)
            {
                Session.SendWhisper("Desculpe, você não é um Super Star, então você não pode usar esse efeito.", 34);
                return;
            }

            // Guia/ALfa 
            if (EffectId == 178 && Session.GetHabbo().Rank < 8)
            {
                Session.SendWhisper("Desculpe, você não é um guia, então você não pode usar esse efeito.", 34);
                return;
            }
            // Guide Effects
            if (EffectId == 592 && Session.GetHabbo()._guidelevel < 3 && Session.GetHabbo().Rank < 8 || EffectId == 595 && Session.GetHabbo()._guidelevel < 2 && Session.GetHabbo().Rank < 8 || EffectId == 597 && Session.GetHabbo()._guidelevel < 1 && Session.GetHabbo().Rank < 8)
            { Session.SendWhisper("Desculpe, você não pertence à equipe de orientação, é por isso que você não pode usar esse efeito.", 34); return; }

            // BOT
            if (EffectId == 187 && Session.GetHabbo().Rank < 8)
            {
                Session.SendWhisper("Desculpe, você não é Bot, então você não pode usar esse efeito.", 34);
                return;
            }

            //BAW
            if (EffectId == 599 && Session.GetHabbo().Rank < 9)
            {
                Session.SendWhisper("Desculpe, você não é o Official Hotel Builder, portanto você não pode usar esse efeito.", 34);
                return;
            }

            // Staff Effects
            if (EffectId == 102 && Session.GetHabbo().Rank < 10)
            {
                Session.SendWhisper("Desculpe, você não é Staff, então você não pode usar esse efeito.", 34);
                return;
            }

            //Croupier
            if (EffectId == 594 && Session.GetHabbo().Rank < 10 || EffectId == 777 && Session.GetHabbo().Rank < 10)
            {
                Session.SendWhisper("Desculpe, você não é Croupier, então você não pode usar esse efeito.", 34);
                return;
            }

            //MODS
            if (EffectId == 596 && Session.GetHabbo().Rank < 11)
            {
                Session.SendWhisper("Desculpe, você não é Moderador, então você não pode usar esse efeito.", 34);
                return;
            }

            //Game Master
            if (EffectId == 602 && Session.GetHabbo().Rank < 12)
            {
                Session.SendWhisper("Desculpe, você não é o Game Master, então você não pode usar esse efeito.", 34);
                return;
            }

            //EDC 13
            if (EffectId == 603 && Session.GetHabbo().Rank < 13)
            {
                Session.SendWhisper("Desculpe, você não é responsável por concursos, portanto, você não pode usar este efeito.", 34);
                return;
            }

            //Forbi
            if (EffectId == 609 && Session.GetHabbo().Username == "Forbi")
            {
                Session.SendWhisper("Tu no eres Forbi.", 34);
                return;
            }

            Session.GetHabbo().LastEffect = EffectId;
            Session.GetHabbo().Effects().ApplyEffect(EffectId);
        }
    }
}
