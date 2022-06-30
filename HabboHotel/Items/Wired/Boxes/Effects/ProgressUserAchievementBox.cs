using System.Collections.Concurrent;

using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.Users;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Messenger;

namespace Moon.HabboHotel.Items.Wired.Boxes.Effects
{
    class ProgressUserAchievementBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectProgressUserAchievement; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public ProgressUserAchievementBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();

            this.StringData = Message;


            Habbo Owner = MoonEnvironment.GetHabboById(Item.UserID);
            if (Owner == null || Owner.Rank < 6)
            {
                this.StringData = "";
                Owner.GetClient().SendWhisper("No sé quién te ha dado esto pero no deberías estar jugando con juguetes de mayores.", 34);
                MoonEnvironment.GetGame().GetClientManager().StaffAlert1(new RoomInviteComposer(int.MinValue, Owner.Username + " está utilizando sin permiso un Wired de Puntos de Recompensa."));
            }
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null || string.IsNullOrWhiteSpace(StringData))
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            var Message = StringData.Split('-');
            MoonEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_" + Message[0], int.Parse(Message[1]));            
            return true;
        }
    }
}