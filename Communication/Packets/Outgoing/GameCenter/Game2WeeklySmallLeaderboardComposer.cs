using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.HabboHotel.Users;

namespace Moon.Communication.Packets.Outgoing.GameCenter
{
    public class Game2WeeklySmallLeaderboardComposer : ServerPacket
    {
        public Game2WeeklySmallLeaderboardComposer(int GameId/*, ICollection<Habbo> Habbos*/)
            : base(ServerPacketHeader.Game2WeeklySmallLeaderboardComposer)
        {
            base.WriteInteger(2014);
            base.WriteInteger(41);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(1581);

            //Used to generate the ranking numbers.
            int num = 0;

            //base.WriteInteger(Habbos.Count);//Count
            //foreach (Habbo Habbo in Habbos.ToList())
            //{
            //    num++;
            //    base.WriteInteger(Habbo.Id);//Id
            //    base.WriteInteger(Habbo.FastfoodScore);//Score
            //    base.WriteInteger(num);//Rank
            //   base.WriteString(Habbo.Username);//Username
            //   base.WriteString(Habbo.Look);//Figure
            //   base.WriteString(Habbo.Gender.ToLower());//Gender .ToLower()
            //}

            //base.WriteInteger(0);//
            //base.WriteInteger(GameData.GameId);//Game Id?

            base.WriteInteger(3);//Count
            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(1);//Rank
            base.WriteString("X");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(2);//Rank
            base.WriteString("Salinas");//Username
            base.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(1);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(3);//Rank
            base.WriteString("HiddenKey");//Username
            base.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            base.WriteString("m");//Gender .ToLower()


            base.WriteInteger(1);//
            base.WriteInteger(GameId);//Game Id?
        }
    }
}
