using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.HabboHotel.Users;

namespace Moon.Communication.Packets.Outgoing.GameCenter
{
    public class Game2CurrentWeekLeaderboardMessageComposer : ServerPacket
    {
        public Game2CurrentWeekLeaderboardMessageComposer(/*GameData Game, int Week*/)
            : base(ServerPacketHeader.Game2CurrentWeekLeaderboardComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(10000);
            base.WriteInteger(1);

            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()  
            base.WriteString("Custom");//Username
            // base.WriteInteger(2018);
            // base.WriteInteger(1);
            // base.WriteInteger(0);
            // base.WriteInteger(1);
            // base.WriteInteger(1581);


            // base.WriteInteger(Game.LeaderBoard.Count());//Count
            // int id = 0;
            // foreach(var Data in Game.LeaderBoard)
            // {
            //     id++;
            //     Habbo habbo = MoonEnvironment.GetHabboById(Data.Value.UserId);
            //     base.WriteInteger(habbo.Id);//Id
            //     base.WriteInteger(id);//Rank
            //     base.WriteInteger(Data.Value.Points);//Score
            //     base.WriteString(habbo.Username);//Username
            //     base.WriteString(habbo.Look);//Figure
            //     base.WriteString(habbo.Gender.ToLower());//Gender .ToLower()

            //     if(id == 10) { break; }
            // }
            ///* base.WriteInteger(1);//Id
            // base.WriteInteger(10);//Rank
            // base.WriteInteger(1);//Score
            // base.WriteString("Custom - Derecha");//Username
            // base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(2);//Id
            // base.WriteInteger(19999);//Score
            // base.WriteInteger(2);//Rank
            // base.WriteString("Salinas");//Username
            // base.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(3);//Id
            // base.WriteInteger(1232);//Score
            // base.WriteInteger(3);//Rank
            // base.WriteString("HiddenKey");//Username
            // base.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(4);//Id
            // base.WriteInteger(1000);//Score
            // base.WriteInteger(4);//Rank
            // base.WriteString("Custom");//Username
            // base.WriteString("fa-1201-62.sh-6102459-96-62.hr-831-1031.ch-804-1201.lg-281-110.ha-1012-78.hd-180-11");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(5);//Id
            // base.WriteInteger(1000);//Score
            // base.WriteInteger(5);//Rank
            // base.WriteString("Custom");//Username
            // base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(6);//Id
            // base.WriteInteger(19999);//Score
            // base.WriteInteger(6);//Rank
            // base.WriteString("Salinas");//Username
            // base.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(7);//Id
            // base.WriteInteger(1232);//Score
            // base.WriteInteger(7);//Rank
            // base.WriteString("HiddenKey");//Username
            // base.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(8);//Id
            // base.WriteInteger(1000);//Score
            // base.WriteInteger(8);//Rank
            // base.WriteString("Custom");//Username
            // base.WriteString("fa-1201-62.sh-6102459-96-62.hr-831-1031.ch-804-1201.lg-281-110.ha-1012-78.hd-180-11");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(9);//Id
            // base.WriteInteger(1000);//Score
            // base.WriteInteger(9);//Rank
            // base.WriteString("Custom");//Username
            // base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            // base.WriteString("m");//Gender .ToLower()

            // base.WriteInteger(10);//Id
            // base.WriteInteger(1000);//Score
            // base.WriteInteger(10);//Rank
            // base.WriteString("Custom");//Username
            // base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            // base.WriteString("m");//Gender .ToLower()*/

            // base.WriteInteger(1);//Unknown
            // base.WriteInteger(Game.GameId);//Game Id?
        }
    }
}
