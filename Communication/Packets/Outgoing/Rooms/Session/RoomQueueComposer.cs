namespace Moon.Communication.Packets.Outgoing.Rooms.Session
{
    public class RoomQueueComposer : ServerPacket
    {
        public RoomQueueComposer(int UsersQueued)
            : base(ServerPacketHeader.RoomQueueComposer)
        {
            base.WriteInteger(2); // 1: Solo Cola; 2: Cola con Opción Espectador
            {
                base.WriteString("visitors");
                base.WriteInteger(2); // 1: Cola para Modo Espectador; 2: Cola normal

                base.WriteInteger(1);
                {
                    base.WriteString("visitors");
                    base.WriteInteger(UsersQueued); // tu puesto en la cola
                }

                base.WriteString("spectators");
                base.WriteInteger(1); // Bool para boton espectador

                base.WriteInteger(1);
                {
                    base.WriteString("spectators");
                    base.WriteInteger(0); // specs?
                }
            }
        }
    }
}