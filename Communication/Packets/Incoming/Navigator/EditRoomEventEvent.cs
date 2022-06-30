using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Navigator
{
    class EditRoomEventEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            string word;
            string Name = Packet.PopString();
            Name = MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out word) ? "Spam" : Name;
            string Desc = Packet.PopString();
            Desc = MoonEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Desc, out word) ? "Spam" : Desc;

            RoomData Data = MoonEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (Data == null)
                return;

            if (Data.OwnerId != Session.GetHabbo().Id)
                return; //HAX

            if (Data.Promotion == null)
            {
                Session.SendNotification("Oops, al parecer no hay una promoción en esta sala.");
                return;
            }

            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `room_promotions` SET `title` = @title, `description` = @desc WHERE `room_id` = " + RoomId + " LIMIT 1");
                dbClient.AddParameter("title", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.RunQuery();
            }

            Room Room;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Convert.ToInt32(RoomId), out Room))
                return;

            Data.Promotion.Name = Name;
            Data.Promotion.Description = Desc;
            Room.SendMessage(new RoomEventComposer(Data, Data.Promotion));
        }
    }
}
