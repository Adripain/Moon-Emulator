namespace Moon.Communication.Packets.Outgoing.Navigator
{
    class NavigatorPreferencesComposer : ServerPacket
    {
        public NavigatorPreferencesComposer()
            : base(ServerPacketHeader.NavigatorPreferencesMessageComposer)
        {
            base.WriteInteger(95);
            base.WriteInteger(60);
            base.WriteInteger(425);
            base.WriteInteger(450);
            base.WriteBoolean(false);
            base.WriteInteger(0);
        }
    }
}