using Moon.Communication.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Incoming.Talents
{
    class CheckQuizType : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            string HabboType = Packet.PopString();
            if (HabboType == "HabboWay1")
            {
                Session.GetHabbo()._HabboQuizQuestions = new List<int>(5);

                var quiz = new ServerPacket(ServerPacketHeader.QuizDataMessageComposer);
                quiz.WriteString(HabboType);
                quiz.WriteInteger(5); // longitud.                
                for (int i = 0; i < 5; i++)
                {
                    int rndNumber = new Random().Next(10);
                    if (Session.GetHabbo()._HabboQuizQuestions.Contains(rndNumber))
                    {
                        for (int ii = 0; ii < 10; ii++)
                        {
                            if (!Session.GetHabbo()._HabboQuizQuestions.Contains(ii))
                            {
                                rndNumber = ii;
                                break;
                            }
                        }
                    }
                    Session.GetHabbo()._HabboQuizQuestions.Add(rndNumber);
                    quiz.WriteInteger(rndNumber);
                }
                Session.SendMessage(quiz);
            }
            else if (HabboType == "SafetyQuiz1")
            {
                MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SafetyQuizGraduate", 1);
            }
        }
    }
}
