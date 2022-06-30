using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items.Wired;
using System;

namespace Moon.HabboHotel.Items.Interactor
{
    public class InteractorJukebox : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            if (Item.GetRoom().GetRoomMusicManager().LinkedItemId != Item.Id)
            {
                Item.GetRoom().GetRoomMusicManager().Reset();
                Item.GetRoom().GetRoomMusicManager().LinkRoomOutputItem(Item);
                Item.GetRoom().LoadMusic();

                if (Item.ExtraData == "1")
                {
                    if (Item.GetRoom().GetRoomMusicManager().PlaylistSize > 0)
                        if (!Item.GetRoom().GetRoomMusicManager().IsPlaying)
                            Item.GetRoom().GetRoomMusicManager().Start();
                        else
                        {
                            Item.ExtraData = "0";
                            Item.UpdateState();
                        }
                }
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            if (Item.GetRoom().GetRoomMusicManager().IsPlaying)
            {
                Item.GetRoom().GetRoomMusicManager().Stop();
                Item.GetRoom().GetRoomMusicManager().BroadcastCurrentSongData(Session.GetHabbo().CurrentRoom);
            }

            Item.GetRoom().GetRoomMusicManager().Reset();
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (!HasRights)
                return;

            switch (Request)
            {
                case -1:

                    break;

                default:
                    if (Item.ExtraData == "1")
                    {
                        Item.GetRoom().GetRoomMusicManager().Stop();
                        Item.ExtraData = "0";
                    }
                    else
                    {
                        Item.GetRoom().GetRoomMusicManager().Start();
                        Item.ExtraData = "1";
                    }

                    Item.UpdateState();

                    Item.GetRoom().GetRoomMusicManager().BroadcastCurrentSongData(Item.GetRoom());
                    break;
            }
        }

        public void OnWiredTrigger(Item Item)
        {
            return;
        }
    }
}