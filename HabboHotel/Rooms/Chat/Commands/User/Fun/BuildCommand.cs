using Moon.Communication.Packets.Outgoing.Inventory.Furni;
using Moon.Communication.Packets.Outgoing.Rooms.Engine;
using Moon.Communication.Packets.Outgoing.Rooms.Furni;
using Moon.Database.Interfaces;
using Moon.HabboHotel.GameClients;
using Moon.HabboHotel.Items;
using Moon.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moon.HabboHotel.Rooms.Chat.Commands.User.Fan
{
    internal class BuildCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_give"; }
        }

        public string Parameters
        {
            get { return "%height%"; }
        }

        public string Description
        {
            get { return "Habilita o teletransporte do quarto para construir com mais facilidade"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            string height = Params[1];
            if (Session.GetHabbo().Id == Room.OwnerId)
            {
                if (!Room.CheckRights(Session, true))
                    return;
                Item[] items = Room.GetRoomItemHandler().GetFloor.ToArray();
                foreach (Item Item in items.ToList())
                {
                    GameClient TargetClient = MoonEnvironment.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (Item.GetBaseItem().InteractionType == InteractionType.STACKTOOL)

                        Room.SendMessage(new UpdateMagicTileComposer(Item.Id, int.Parse(height)));
                }
            }
        }
    }
}