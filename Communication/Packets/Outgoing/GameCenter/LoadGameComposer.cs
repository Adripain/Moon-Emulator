using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Communication.Packets.Outgoing.GameCenter
{
    class LoadGameComposer : ServerPacket
    {
        public LoadGameComposer(GameData GameData, string SSOTicket, GameClient Session)
            : base(ServerPacketHeader.LoadGameMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteString("1365260055982");
            base.WriteString("https://www.Jabboz.com/game/games/gamecenter_basejump/BaseJump.swf");
            base.WriteString("best");
            base.WriteString("showAll");
            base.WriteInteger(60);//FPS?   
            base.WriteInteger(10);
            base.WriteInteger(8);
            base.WriteInteger(6);//Asset count
            base.WriteString("assetUrl");
            base.WriteString("https://www.Jabboz.com/game/games/gamecenter_basejump/BasicAssets.swf");
            base.WriteString("habboHost");
            base.WriteString("http://fuseus-private-httpd-fe-1");
            base.WriteString("accessToken");
            base.WriteString(SSOTicket);
            base.WriteString("gameServerHost");
            base.WriteString("37.59.173.22");
            base.WriteString("gameServerPort");
            base.WriteString("3030");
            base.WriteString("socketPolicyPort");
            base.WriteString("37.59.173.22");
        }
    }
}
