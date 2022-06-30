using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Moderation;
using Moon.HabboHotel.Items;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Database.Interfaces;
using System.Data;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class UpdateFurniture : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "user_16"; }
        }


        public string Parameters
        {
            get { return "(tipo) (quantidade)"; }
        }

        public string Description
        {
            get { return "Envie um alerta para todo o hotel."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser RUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            List<Item> Items = Room.GetGameMap().GetRoomItemForSquare(RUser.X, RUser.Y);
            if (Params.Length == 1 || Params[1] == "utilidad")
            {
                StringBuilder Lista = new StringBuilder();
                Lista.Append("Lista de usos: (Nota: Edita TODOS os items abaixo): \r\r");
                Lista.Append("1) :item width numero (Editar para largura do item) \r");
                Lista.Append("2) :item length numero (Editar para comprimento do item) \r");
                Lista.Append("3) :item height numero (Editar a altura do Item) \r");
                Lista.Append("4) :item cansit si/no (Permite / Não Permite sentar sobre o Item) \r");
                Lista.Append("5) :item canwalk si/no (Permite / Não Permite caminhar sobre o Item) \r");
                Lista.Append("6) :item canstack si/no (Permite / Não Permite empilhar sobre o Item) \r");
                Lista.Append("7) :item mercadillo si/no (Permite / Não Permite a venda do Item no mercado) \r");
                Lista.Append("8) :item interaction nombre (Atribuir uma interação ao item) \r");
                Lista.Append("9) :item interactioncount numero (Atribuir o número de interações do item) \r\r");
                Lista.Append("Nota: Para atualizar o Item, você deve pegá-lo e colocá-lo na sala novamente ou atualizar o quarto.");
                Session.SendMessage(new MOTDNotificationComposer(Lista.ToString()));
                return;
            }
            String Type = Params[1].ToLower();
            int numeroint = 0, FurnitureID = 0;
            double numerodouble = 0;
            DataRow Item = null;
            String opcion = "";
            switch (Type)
            {
                case "width":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `width` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Anchura del Item: " + FurnitureID + " editada com éxito (Valor de anchura ingresado: " + numeroint.ToString() + ")");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro (Ingrese números válidos)");
                        }
                    }
                    break;
                case "length":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `length` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Longitud del Item: " + FurnitureID + " editada con éxito (Valor de longitud ingresado: " + numeroint.ToString() + ")");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro (Ingrese números válidos)");
                        }
                    }
                    break;
                case "height":
                    {
                        try
                        {
                            numerodouble = Convert.ToDouble(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `stack_height` = '" + numerodouble + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Altura del Item: " + FurnitureID + " editada con éxito (Valor de altura ingresado: " + numerodouble.ToString() + ")");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro (Ingrese números válidos)");
                        }
                    }
                    break;
                case "interactioncount":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `interaction_modes_count` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Numero de interacciones del Item: " + FurnitureID + " editado con éxito (Valor ingresado: " + numeroint.ToString() + ")");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro (Ingrese números válidos)");
                        }
                    }
                    break;
                case "cansit":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `can_sit` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("can_sit del Item: " + FurnitureID + " editado con éxito");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "canstack":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `can_stack` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("can_stack del Item: " + FurnitureID + " editado con éxito");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "canwalk":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `is_walkable` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("can_walk del Item: " + FurnitureID + " editado con éxito");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "mercadillo":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                Session.SendWhisper("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `is_rare` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Opción de venta en el mercadillo del Item: " + FurnitureID + " editado con éxito");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                case "interaction":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `interaction_type` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Interacción del Item: " + FurnitureID + " editada con éxito. (Valor ingresado: " + opcion + ")");
                            }
                            MoonEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ocorreu um erro.");
                        }
                    }
                    break;
                default:
                    {
                        Session.SendNotification("A opção digitada não existe, para saber as opções digite :item utilidad");
                        return;
                    }
                    break;
            }


        }
    }
}