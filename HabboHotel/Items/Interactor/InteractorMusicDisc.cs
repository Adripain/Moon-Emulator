//using Moon.Communication.Packets.Outgoing.Rooms.Music;
//using Moon.Communication.Packets.Outgoing.Sound;
//using Moon.HabboHotel.GameClients;
//using Moon.HabboHotel.Rooms;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Moon.HabboHotel.Items.Interactor
//{
//    class InteractorMusicDisc : IFurniInteractor
//    {

//        public void OnRemove(GameClient Session, Item Item)
//        {
//            var room = Item.GetRoom();
//            var cd = Item.GetRoom().GetTraxManager().GetDiscItem(Item.Id);
//            if (cd != null)
//            {
//                room.GetTraxManager().StopPlayList();
//                room.GetTraxManager().RemoveDisc(Item);
//            }
//            //else
//            {
//                var Items = room.GetRoomMusicManager().RemoveDisk(Item);
//                Items.Remove(Item);
//                room.SendMessage(new GetJukeboxDisksComposer(Items));
//            }
//        }

//        public void OnTrigger(GameClient Client, Item Item, int sla, bool sla2)
//        {

//        }
//        public void OnWiredTrigger(Item Item)
//        {

//        }

//        public void OnPlace(GameClient Session, Item Item)
//        {
//            var room = Item.GetRoom();
//            var Items = room.GetTraxManager().GetAvaliableSongs();
//            if (!Items.Contains(Item) && !room.GetTraxManager().Playlist.Contains(Item))
//                Items.Add(Item);
//            room.SendMessage(new GetJukeboxDisksComposer(Items));
//        }
//    }
//}
