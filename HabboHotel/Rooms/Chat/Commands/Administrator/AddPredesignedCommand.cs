using Moon.HabboHotel.Catalog.PredesignedRooms;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class AddPredesignedCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "";
        public string Description => "Adiciona um quarto a um quarto designado";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null) return;
            //if (MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().Exists((uint)Room.Id))
            //{
            //    Session.SendWhisper("O quarto não está selecionado na lista.");
            //    return;
            //}

            StringBuilder itemAmounts = new StringBuilder(), floorItemsData = new StringBuilder(), wallItemsData = new StringBuilder(),
                decoration = new StringBuilder();
            var floorItems = Room.GetRoomItemHandler().GetFloor;
            var wallItems = Room.GetRoomItemHandler().GetWall;
            foreach (var roomItem in floorItems)
            {
                var itemCount = floorItems.Count(item => item.BaseItem == roomItem.BaseItem);
                if (!itemAmounts.ToString().Contains(roomItem.BaseItem + "," + itemCount + ";"))
                    itemAmounts.Append(roomItem.BaseItem + "," + itemCount + ";");

                floorItemsData.Append(roomItem.BaseItem + "$$$$" + roomItem.GetX + "$$$$" + roomItem.GetY + "$$$$" + roomItem.GetZ +
                    "$$$$" + roomItem.Rotation + "$$$$" + roomItem.ExtraData + ";");
            }
            foreach (var roomItem in wallItems)
            {
                var itemCount = wallItems.Count(item => item.BaseItem == roomItem.BaseItem);
                if (!itemAmounts.ToString().Contains(roomItem.BaseItem + "," + itemCount + ";"))
                    itemAmounts.Append(roomItem.BaseItem + "," + itemCount + ";");

                wallItemsData.Append(roomItem.BaseItem + "$$$$" + roomItem.wallCoord + "$$$$" + roomItem.ExtraData + ";");
            }

            decoration.Append(Room.RoomData.FloorThickness + ";" + Room.RoomData.WallThickness + ";" +
                Room.RoomData.Model.WallHeight + ";" + Room.RoomData.Hidewall + ";" + Room.RoomData.Wallpaper + ";" +
                Room.RoomData.Landscape + ";" + Room.RoomData.Floor);

            using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO catalog_predesigned_rooms(room_model,flooritems,wallitems,catalogitems,room_id,room_decoration) VALUES('" + Room.RoomData.ModelName +
                    "', '" + floorItemsData + "', '" + wallItemsData + "', '" + itemAmounts + "', " + Room.Id + ", '" + decoration + "');");
                var predesignedId = (uint)dbClient.InsertQuery();

                MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.Add(predesignedId,
                    new PredesignedRooms(predesignedId, (uint)Room.Id, Room.RoomData.ModelName,
                        floorItemsData.ToString().TrimEnd(';'), wallItemsData.ToString().TrimEnd(';'),
                        itemAmounts.ToString().TrimEnd(';'), decoration.ToString()));
            }

            Session.SendWhisper("A sala foi adicionada corretamente a lista.");



        }
    }
}