using Moon.HabboHotel.Catalog.PredesignedRooms;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class RemovePredesignedCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "";
        public string Description => "Remove o quarto da lista de quartos pré-designado";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null) return;
            //if (!MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().Exists((uint)Room.Id))
            //{
            //    Session.SendWhisper("O quarto não existe na lista.");
            //    return;
            //}

            var predesignedId = 0U;
            using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM catalog_predesigned_rooms WHERE room_id = " + Room.Id + ";");
                predesignedId = (uint)dbClient.getInteger();

                dbClient.runFastQuery("DELETE FROM catalog_predesigned_rooms WHERE room_id = " + Room.Id + " AND id = " +
                    predesignedId + ";");
            }

            MoonEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.Remove(predesignedId);
            Session.SendWhisper("O quarto foi removido corretamente da lista.");
        }
    }
}