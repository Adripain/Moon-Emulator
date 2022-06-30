using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;

namespace Moon.Communication.Packets.Incoming.Rooms.FloorPlan
{
    class FloorPlanEditorRoomPropertiesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            DynamicRoomModel Model = Room.GetGameMap().Model;
            if (Model == null)
                return;

            ICollection<Item> FloorItems = Room.GetRoomItemHandler().GetFloor;

            Session.SendMessage(new FloorPlanFloorMapComposer(FloorItems));
            Session.SendMessage(new FloorPlanSendDoorComposer(Model.DoorX, Model.DoorY, Model.DoorOrientation));
            Session.SendMessage(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, MoonEnvironment.EnumToBool(Room.Hidewall.ToString())));
        }
    }
}
