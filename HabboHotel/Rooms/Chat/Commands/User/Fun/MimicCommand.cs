using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Database.Interfaces;


namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MimicCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_normal"; }
        }

        public string Parameters
        {
            get { return "[USUARIO]"; }
        }

        public string Description
        {
            get { return "Copiar a roupa de outro usuário."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário que você deseja copiar a roupa.", 34);
                return;
            }

            GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente ou talvez o usuário não esteja online.", 34);
                return;
            }

            if (!TargetClient.GetHabbo().AllowMimic)
            {
                Session.SendWhisper(Params[1] + " você desativou a opção de ter sua aparência copiada.", 34);
                return;
            }

            RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Ocorreu um erro, escreva o nome corretamente ou talvez o usuário não esteja online.", 34);
                return;
            }

            Session.GetHabbo().Gender = TargetUser.GetClient().GetHabbo().Gender;
            Session.GetHabbo().Look = TargetUser.GetClient().GetHabbo().Look;

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `gender` = @gender, `look` = @look WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("gender", Session.GetHabbo().Gender);
                dbClient.AddParameter("look", Session.GetHabbo().Look);
                dbClient.AddParameter("id", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User != null)
            {
                Session.SendMessage(new AvatarAspectUpdateMessageComposer(Session.GetHabbo().Look, Session.GetHabbo().Gender));
                Session.SendMessage(new UserChangeComposer(User, true));
                Room.SendMessage(new UserChangeComposer(User, false));
            }
        }
    }
}