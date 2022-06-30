using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Games;
using Moon.HabboHotel.Users;

namespace Moon.Communication.Packets.Outgoing.GameCenter
{
    public class GameCenterLuckyLoosersWinnersComposer : ServerPacket
    {
        public GameCenterLuckyLoosersWinnersComposer(int GameId)
            : base(ServerPacketHeader.GameCenterLuckyLoosersWinnersComposer)
        {
            base.WriteInteger(GameId);
            base.WriteInteger(1);
            base.WriteString("PutoAmo");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()  
            base.WriteInteger(1);//Rank       
            base.WriteInteger(99999999);//Score

            //base.WriteString("Custom");//Username
            //base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            //base.WriteString("m");//Gender .ToLower()  
            //base.WriteInteger(365);//Rank       
            //base.WriteInteger(99999999);//Score

            //base.WriteString("Custom");//Username
            //base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            //base.WriteString("m");//Gender .ToLower()  
            //base.WriteInteger(25);//Rank       
            //base.WriteInteger(99999999);//Score

            //base.WriteString("Custom");//Username
            //base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            //base.WriteString("m");//Gender .ToLower()  
            //base.WriteInteger(14);//Rank       
            //base.WriteInteger(99999999);//Score
        }
    }
}
