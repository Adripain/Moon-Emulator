using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Moon.HabboHotel.GameClients;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class ForceDiceCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }

        public string Parameters
        {
            get { return "%number%"; }
        }

        public string Description
        {
            get { return "Permite que você carregue um item de mão"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve colocar uma figura para ir nos dados de 1 a 6.");
                return;
            }

            GameClient Target = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("¡Oops, este usuário não foi encontrado!");
                return;
            }

            int Number;
            if (!int.TryParse(Params[2], out Number))
            {
                Session.SendWhisper("Por favor insira um número válido.");
                return;
            }

            if(Number > 6 || Number < 1)
            {
                Session.SendWhisper("A figura deve estar entre 1 e 6.");
                return;
            }

            Target.GetHabbo().RigDice = true;
            Target.GetHabbo().DiceNumber = Number;
            Session.SendWhisper("Você acabou de ativar o número " + Number + " de modo que sai no dado de " + Target.GetHabbo().Username + ".");
        }
    }
}

