using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Database.Interfaces;
using System;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveRanksCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "[USUARIO] [TIPO] [RANK]";
        public string Description => "Escreva :rank para ver a explicação.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendMessage(new MassEventComposer("habbopages/chat/giverankinfo.txt"));
                return;
            }

            GameClient Target = MoonEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, não foi possivel encontrar esse usuário.");
                return;
            }


            string RankType = Params[2];
            switch (RankType.ToLower())
            {
                case "Administrador":
                case "administrador":
                case "adm":
                    {
                        int NewRank = 15;
                        if (Session.GetHabbo().Rank < 12)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 16)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, o usuário já tem esse rank!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops,o usuário tem um rank maior que o seu!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rank entregue a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encarregado da parte administrativa.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '15' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                        }
                        Target.Disconnect();
                        break;
                    }

                case "encargado":
                case "concursos":
                    {
                        int NewRank = 14;
                        if (Session.GetHabbo().Rank < 12)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 15)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, o usuário ja tem esse rank!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, o rank do usuário é maior que o seu!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rank entregue a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encarregado do Hotel.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '15' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }
                case "coadm":
                case "coadministrador":
                    {
                        int NewRank = 13;
                        if (Session.GetHabbo().Rank < 12)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 15)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, o usuário já tem esse rank!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, o usuário tem um rank maior que o seu!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rank entregue a  " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encarregado das noticias.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '15' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }
                case "gm":
                case "gamemaster":
                    {
                        int NewRank = 12;
                        if (Session.GetHabbo().Rank < 12)
                        {
                            Session.SendWhisper("Você não tem permissão para usar este comando");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 15)
                        {
                            Session.SendWhisper("Oops,você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, o usuário já tem esse rank!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, o usuário tem um rank maior que o seu!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rank entregue a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encarregado da diversão.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '15' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }

                case "eds":
                case "seguridad":
                    {
                        int NewRank = 11;
                        if (Session.GetHabbo().Rank < 12)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 13)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, o usuário já tem esse rank!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, o usuário tem um rank maior que o seu!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rank entregue a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encargado de la seguridad.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '10' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }

                case "mod":
                case "moderador":
                    {
                        int NewRank = 10;
                        if (Session.GetHabbo().Rank < 12)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 13)
                        {
                            Session.SendWhisper("Oops, você não tem permissão para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, o usuário já tem esse rank!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, El usuario tiene un rango mayor que usted y no lo puede modificar!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rango entregado satisfactoriamente a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encargado de los casinos.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '10' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }

                case "builder":
                case "baw":
                    {
                        int NewRank = 9;
                        if (Session.GetHabbo().Rank < 11)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 11)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, El usuario ya tiene este rango!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, El usuario tiene un rango mayor que usted y no lo puede modificar!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rango entregado satisfactoriamente a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Constructor oficial del Hotel.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '10' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");


                        }
                        Target.Disconnect();
                        break;
                    }

                case "dj":
                case "radio":
                    {
                        int NewRank = 8;
                        if (Session.GetHabbo().Rank < 11)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 11)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, El usuario ya tiene este rango!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, El usuario tiene un rango mayor que usted y no lo puede modificar!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rango entregado satisfactoriamente a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Estoy para servirle a los usuarios.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '5' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }

                case "emb":
                case "embajador":
                    {
                        int NewRank = 7;
                        if (Session.GetHabbo().Rank < 11)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 11)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, El usuario ya tiene este rango!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, El usuario tiene un rango mayor que usted y no lo puede modificar!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rango entregado satisfactoriamente a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encargado de la Publicidad.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '5' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }

                case "helper":
                case "help":
                    {
                        int NewRank = 6;
                        if (Session.GetHabbo().Rank < 8)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 8)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, El usuario ya tiene este rango!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, El usuario tiene un rango mayor que usted y no lo puede modificar!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rango entregado satisfactoriamente a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Miembro del HabboVisa.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '5' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }


                case "lince":
                case "lnc":
                    {
                        int NewRank = 5;
                        if (Session.GetHabbo().Rank < 8)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Session.GetHabbo().Rank < 8)
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        if (Target.GetHabbo().Rank == NewRank)
                        {
                            Session.SendWhisper("Oops, El usuario ya tiene este rango!");
                            break;
                        }
                        if (Target.GetHabbo().Rank >= Session.GetHabbo().Rank)
                        {
                            Session.SendWhisper("Oops, El usuario tiene un rango mayor que usted y no lo puede modificar!");
                            break;
                        }
                        //Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " acaba de darte el rango de Fundador. Reinicia para aplicar los cambios respectivos.\n\nRecuerda que hemos depositado nuestra confianza en tí y que todo esfuerzo tiene su recompensa.", ""));
                        Session.SendWhisper("Rango entregado satisfactoriamente a " + Target.GetHabbo().Username + ".");
                        Target.GetHabbo().Rank = NewRank;
                        using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.runFastQuery("UPDATE `users` SET `rank` = '" + NewRank + "', cms_role = 'Encargado de subir los ons.' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1");
                            dbClient.runFastQuery("UPDATE `users` SET `respetos` = '5' WHERE `id` = '" + Target.GetHabbo().Id + "'");
                            dbClient.runFastQuery("UPDATE `users` SET `tag` = '®' WHERE `id` = '" + Target.GetHabbo().Id + "'");

                        }
                        Target.Disconnect();
                        break;
                    }

                default:
                    Session.SendWhisper(RankType + "' no es un rango disponible para otorgar.");
                    break;
            }


        }
    }
}