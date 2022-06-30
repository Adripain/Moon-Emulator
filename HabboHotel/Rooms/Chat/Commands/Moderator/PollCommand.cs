using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class PollCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            int time = int.Parse(Params[1]);
            string quest = CommandManager.MergeParams(Params, 2);

            if (Params.Length == 0)
            {
                Session.SendWhisper("Por favor faça sua pergunta");
            }
            else
            {

                if (quest == "end")
                {
                    Room.endQuestion();
                }
                else if (time != -1 || time != 0)
                {
                    Room.startQuestion(quest);
                    time = time * 864000;
                    Task t = Task.Factory.StartNew(() => TaskStopQuestion(Room, time));
                }
                else
                    Room.startQuestion(quest);

            }
        }

        public void TaskStopQuestion(Room room, int time)
        {
            Thread.Sleep(time);
            room.endQuestion();
        }

        public string Description =>
            "Realizar uma enquete rápida.";

        public string Parameters =>
            "[TEMPO] [PREGUNTA]";

        public string PermissionRequired =>
            "user_6";
    }
}