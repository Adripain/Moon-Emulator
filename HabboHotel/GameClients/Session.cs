using Fleck;
using Moon.Communication.Packets.Incoming;
using Moon.Communication.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.GameClients
{
    public class Session
    {
        public readonly IWebSocketConnection socket;
        public readonly Guid identifier;
        public GameClient client;

        public Session(IWebSocketConnection socket)
        {
            this.socket = socket;
            this.identifier = socket.ConnectionInfo.Id;
        }

        public void handleMessage(byte[] bytes)
        {
            try
            {
                ClientPacket packet = new ClientPacket(bytes);
                Console.WriteLine("SOCKET Packet:" + packet.Id);
                if (packet.Id == 1)
                {
                    int id = packet.PopInt();
                    string ssoTicket = packet.PopString();
                    Console.WriteLine(id + "  -  " + ssoTicket);

                    GameClient client = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(id);

                    if (client == null || client.ssoTicket != ssoTicket)
                    {
                        Console.WriteLine("No coincide.");
                        Console.WriteLine(client.ssoTicket);
                        Console.WriteLine(ssoTicket);
                        socket.Close();
                        return;
                    }

                    client.wsSession = this;
                    this.client = client;

                    ServerPacket loginSso = new ServerPacket(1);
                    this.send(loginSso);

                }

                if (packet.Id == 2)
                {
                    bool test = packet.PopBoolean();

                    if (client == null)
                    {
                        socket.Close();
                        return;
                    }

                    Console.WriteLine(test);

                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public void send(ServerPacket packet )
        {
            if(this.socket.IsAvailable)
            {
                this.socket.Send(packet.GetBytes());
            }
        }

        public void onEnd()
        {
            client.wsSession = null;
            this.client = null;
        }
    }
}
