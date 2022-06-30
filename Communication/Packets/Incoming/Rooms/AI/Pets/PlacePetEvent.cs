using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms.AI;
using Moon.HabboHotel.Rooms;
using Moon.Communication.Packets.Outgoing.Inventory.Pets;

using Moon.HabboHotel.Rooms.AI.Speech;
using Moon.HabboHotel.Rooms.AI.Responses;
using log4net;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Moon.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class PlacePetEvent : IPacketEvent
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.Communication.Packets.Incoming.Rooms.AI.Pets.PlacePetEvent");

        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!MoonEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if ((Room.AllowPets == 0 && !Room.CheckRights(Session, true)) || !Room.CheckRights(Session, true))
            {
                Session.SendMessage(new RoomErrorNotifComposer(1));
                return;
            }

            if (Room.GetRoomUserManager().PetCount > MoonStaticGameSettings.RoomPetPlacementLimit)
            {
                Session.SendMessage(new RoomErrorNotifComposer(2));//5 = I have too many.
                return;
            }

            Pet Pet = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryGetPet(Packet.PopInt(), out Pet))
                return;

            if (Pet == null)
                return;

            if (Pet.PlacedInRoom)
            {
                Session.SendNotification("Esta mascota ya se encuentra en la sala?");
                return;
            }

            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false))
            {
                Session.SendMessage(new RoomErrorNotifComposer(4));
                return;
            }

            RoomUser OldPet = null;
            if (Room.GetRoomUserManager().TryGetPet(Pet.PetId, out OldPet))
            {
                Room.GetRoomUserManager().RemoveBot(OldPet.VirtualId, false);
            }

            Pet.X = X;
            Pet.Y = Y;

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.RoomId;

            List<RandomSpeech> RndSpeechList = new List<RandomSpeech>();
            RoomBot RoomBot = new RoomBot(Pet.PetId, Pet.RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, X, Y, 0, 0, 0, 0, 0, 0, ref RndSpeechList, "", 0, Pet.OwnerId, false, 0, false, 0);
            if (RoomBot == null)
                return;

            Room.GetRoomUserManager().DeployBot(RoomBot, Pet);

            Pet.DBState = DatabaseUpdateState.NeedsUpdate;
            Room.GetRoomUserManager().UpdatePets();

            Pet ToRemove = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryRemovePet(Pet.PetId, out ToRemove))
            {
                log.Error("Error whilst removing pet: " + ToRemove.PetId);
                return;
            }

            Session.SendMessage(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
        }
    }
}
