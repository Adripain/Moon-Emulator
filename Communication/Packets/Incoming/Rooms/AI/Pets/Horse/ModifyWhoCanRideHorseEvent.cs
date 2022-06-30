using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Moon.Database.Interfaces;


namespace Moon.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class ModifyWhoCanRideHorseEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int PetId = Packet.PopInt();
           
            RoomUser Pet = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out Pet))
                return;

            if (Pet.PetData.AnyoneCanRide == 1)
                Pet.PetData.AnyoneCanRide = 0;
            else
                Pet.PetData.AnyoneCanRide = 1;


            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `bots_petdata` SET `anyone_ride` = '" + Pet.PetData.AnyoneCanRide + "' WHERE `id` = '" + PetId + "' LIMIT 1");
            }

            Room.SendMessage(new PetInformationComposer(Pet.PetData));
        }
    }
}
