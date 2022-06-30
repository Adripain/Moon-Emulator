using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Nux;
using Moon.Communication.Packets.Outgoing.Rooms.Nux;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveSpecialReward : IChatCommand
    {
        public string PermissionRequired => "user_13";
        public string Parameters => "[USUARIO]";
        public string Description => "";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 0)
            {
                Session.SendWhisper("Por favor, digite um nome de usuário para recompensar.", 34);
                return;
            }

            GameClient Target = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, este usuário não foi encontrado!", 34);
                return;
            }

            Target.SendMessage(new NuxItemListComposer());
            Session.SendWhisper("Você enviou corretamente o prêmio especial para " + Target.GetHabbo().Username, 34);
        }
    }
}