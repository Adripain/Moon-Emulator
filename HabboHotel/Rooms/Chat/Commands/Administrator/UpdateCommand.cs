using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Communication.Packets.Outgoing.Catalog;
using Moon.Core;
using Moon.HabboHotel.GameClients;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.Communication.Packets.Outgoing.Inventory.Achievements;
using Moon.Communication.Packets.Incoming.LandingView;

namespace Moon.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class UpdateCommand : IChatCommand
    {
        public string PermissionRequired => "user_16";
        public string Parameters => "[VARIABLE]";
        public string Description => "Atualiza uma parte do hotel.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve inserir algo para atualizar, ex. :update catalog");
                return;
            }


            string UpdateVariable = Params[1];
            switch (UpdateVariable.ToLower())
            {
                case "calendar":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oops, surgiu um erro.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetCalendarManager().Init();
                        Session.SendWhisper("Calendario atualizado.");
                        break;
                    }
                case "ecotron":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oops, surgiu um erro.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetFurniMaticRewardsMnager().Initialize(MoonEnvironment.GetDatabaseManager().GetQueryReactor());
                        Session.SendWhisper("Premios ecotrón atualizados.");
                        break;
                    }
                case "grupos":
                case "groups":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o grupos.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        MoonEnvironment.GetGame().GetGroupManager().Init();

                        break;
                    }

                case "cata":
                case "catalog":
                case "catalogue":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o catalogo.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        MoonEnvironment.GetGame().GetCatalogFrontPageManager().LoadFrontPage();
                        MoonEnvironment.GetGame().GetCatalog().Init(MoonEnvironment.GetGame().GetItemManager());
                        MoonEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "¡El catálogo ha sido atualizado, échale un vistazo!", "catalog/open/" + Message + ""));

                        break;
                    }

                case "halloffame":
                case "salondelafama":
                case "hall":
                    {
                        GetHallOfFame.getInstance().Load();
                        Session.SendWhisper("Hall of Fame atualizado com exito.", 34);
                        break;
                    }

                case "goals":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tempermissão para atualizar o LandingCommunityGoalVS.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        MoonEnvironment.GetGame().GetCommunityGoalVS().LoadCommunityGoalVS();

                        Session.SendWhisper("Você atualizou os LandingCommunityGoalVS.", 34);

                        break;
                    }

                case "pinatas":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o premios de las piñatas.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetPinataManager().Initialize(MoonEnvironment.GetDatabaseManager().GetQueryReactor());
                        MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "Se han atualizado los premios de las piñatas.", ""));
                        break;
                    }

                case "polls":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o premios de las piñatas.");
                            break;
                        }
                        MoonEnvironment.GetGame().GetPollManager().Init();
                        break;
                    }

                case "list":
                    {
                        StringBuilder List = new StringBuilder("");
                        List.AppendLine("Lista de comandos para actualizar");
                        List.AppendLine("---------------------------------");
                        List.AppendLine(":update catalog = Atualizar o cátalogo.");
                        List.AppendLine(":update items = Atualiza os ítems, se mudou algo em 'furniture'");
                        List.AppendLine(":update models =No caso de você adicionar qualquer modelo de sala manualmente");
                        List.AppendLine(":update promotions = Atualize as notícias que estão na vista do hotel 'Server Landinds'");
                        List.AppendLine(":update filter = Atualiza o filtro, 'sempre execute se uma palavra for adicionada'");
                        List.AppendLine(":update navigator = Atualiza o Navegador");
                        List.AppendLine(":update rights = Atualiza os Permisos");
                        List.AppendLine(":update configs = Atualiza a configuração do hotel");
                        List.AppendLine(":update bans = Atualiza os banidos");
                        List.AppendLine(":update tickets = Atualiza os tickets de mod");
                        List.AppendLine(":update badge_definitions = Atualiza os emblemas adicionados");
                        List.AppendLine(":update vouchers = Acualiza os vouchers adicionadoa");
                        List.AppendLine(":update characters = Atualiza os carácteres do filtro.");
                        List.AppendLine(":update offers = Atualiza as ofertas relámpago do hotel.");
                        List.AppendLine(":update nux = Atualiza os premios nux do hotel.");
                        List.AppendLine(":update polls = Atualiza os polls do hotel.");
                        Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                        break;
                    }

                
                case "characters":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o carácteres do filtro");
                            break;
                        }

                        MoonEnvironment.GetGame().GetChatManager().GetFilter().InitCharacters();
                        Session.SendWhisper("Carácteres do filtro actualizado correctamente.");
                        break;
                    }

                case "items":
                case "furni":
                case "furniture":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o furnis");
                            break;
                        }

                        MoonEnvironment.GetGame().GetItemManager().Init();
                        Session.SendWhisper("Items atualizados correctamente.");
                        break;
                    }

                case "crafting":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar o crafting.");
                        break;
                    }

                    MoonEnvironment.GetGame().GetCraftingManager().Init();
                    Session.SendWhisper("Crafting atualizado corretamente.");
                    break;

                case "offers":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar o furnis");
                        break;
                    }

                    MoonEnvironment.GetGame().GetTargetedOffersManager().Initialize(MoonEnvironment.GetDatabaseManager().GetQueryReactor());
                    Session.SendWhisper("Ofertas relámpago atualizadas correctamente.");
                    break;

                case "songs":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar as músicas.");
                        break;
                    }

                    MoonEnvironment.GetGame().GetMusicManager().Init();
                    Session.SendWhisper("Você recarregou todas as músicas.");
                    break;

                case "models":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_models"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o Models");
                            break;
                        }

                        MoonEnvironment.GetGame().GetRoomManager().LoadModels();
                        Session.SendWhisper("Modelos de sala atualizados corretamente.");
                        break;
                    }

                case "promotions":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_promotions"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar as promoções.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetLandingManager().LoadPromotions();
                        Session.SendWhisper("Noticias de vista do Hotel atualizadas corretamente.");
                        break;
                    }

                case "youtube":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_youtube"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o videos de Youtube TV.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetTelevisionManager().Init();
                        Session.SendWhisper("Youtube televisão atualizado corretamente");
                        break;
                    }

                case "filter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oops, Você não tem permissão para atualizar o filtro");
                            break;
                        }

                        MoonEnvironment.GetGame().GetChatManager().GetFilter().InitWords();
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("filters", Session.GetHabbo().Username + " ha atualizado el filtro del hotel.", ""));
                        break;
                    }

                case "navigator":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_navigator"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o navegador.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetNavigator().Init();
                        MoonEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("newuser", Session.GetHabbo().Username + " ha modificado las salas públicas del hotel.", ""));
                        break;
                    }

                case "ranks":
                case "rights":
                case "permissions":
                case "commands":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rights"))
                        {
                            Session.SendWhisper("Oops, você não tem direito para atualizar os direitos e permissões.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetPermissionManager().Init();

                        foreach (GameClient Client in MoonEnvironment.GetGame().GetClientManager().GetClients.ToList())
                        {
                            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetPermissions() == null)
                                continue;

                            Client.GetHabbo().GetPermissions().Init(Client.GetHabbo());
                        }

                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " ha atualizado todos los permisos, comandos y rangos del hotel.", ""));
                        break;
                    }

                case "config":
                case "settings":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_configuration"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar a configuração do Hotel");
                            break;
                        }

                        MoonEnvironment.ConfigData = new ConfigData();
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " ha recargado la configuración del hotel.", ""));
                        break;
                    }

                case "bans":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bans"))
                        {
                            Session.SendWhisper("Oops, você não tem permissões para atualizar a lista de banidos");
                            break;
                        }

                        MoonEnvironment.GetGame().GetModerationManager().ReCacheBans();
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " ha atualizado la lista de baneos de " + MoonEnvironment.GetDBConfig().DBData["hotel.name"] + ".", ""));
                        break;
                    }

                case "quests":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_quests"))
                        {
                            Session.SendWhisper("Oops, você não tem permissões para atualizar as missões.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetQuestManager().Init();
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " ha atualizado todas las misiones y retos de " + MoonEnvironment.GetDBConfig().DBData["hotel.name"] + ".", ""));
                        break;
                    }

                case "achievements":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_achievements"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o logros.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetAchievementManager().LoadAchievements();
                        MoonEnvironment.GetGame().GetClientManager().SendMessage(new BadgeDefinitionsComposer(MoonEnvironment.GetGame().GetAchievementManager()._achievements));
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " ha recargado todos los desafíos y achievements del hotel satisfactoriamente.", ""));
                        break;
                    }



                case "moderation":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_moderation"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_moderation' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetModerationManager().Init();
                        MoonEnvironment.GetGame().GetClientManager().ModAlert("Moderation presets have been updated. Please reload the client to view the new presets.");
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " ha atualizado la configuración de los permisos de moderación.", ""));
                        break;
                    }


                case "vouchers":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_vouchers"))
                        {
                            Session.SendWhisper("Oops, você não tem permissões suficientes para atualizar os vouchers.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetCatalog().GetVoucherManager().Init();
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " ha atualizado los códigos voucher del hotel.", ""));
                        break;
                    }

                case "gc":
                case "games":
                case "gamecenter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_game_center"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_game_center' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetGameDataManager().Init();
                        MoonEnvironment.GetGame().GetLeaderBoardDataManager().Init();
                        Session.SendWhisper("Game Center cache successfully updated.");
                        break;
                    }

                case "pet_locale":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_pet_locale"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_pet_locale' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetChatManager().GetPetLocale().Init();
                        Session.SendWhisper("Pet locale cache successfully updated.");
                        break;
                    }

                case "locale":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_locale"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_locale' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetLanguageLocale().Init();
                        Session.SendWhisper("Locale cache successfully updated.");
                        break;
                    }

                case "mutant":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_anti_mutant"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_anti_mutant' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetAntiMutant().Init();
                        Session.SendWhisper("Anti mutant successfully reloaded.");
                        break;
                    }

                case "bots":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bots"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_bots' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetBotManager().Init();
                        Session.SendWhisper("Bot recargados correctamente");
                        break;
                    }

                case "rewards":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_rewards' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetRewardManager().Reload();
                        Session.SendWhisper("Rewards managaer successfully reloaded.");
                        break;
                    }

                case "chat_styles":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_chat_styles"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_chat_styles' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetChatManager().GetChatStyles().Init();
                        Session.SendWhisper("Chat Styles successfully reloaded.");
                        break;
                    }

                case "badges":
                case "badge_definitions":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_badge_definitions"))
                        {
                            Session.SendWhisper("Oops, you do not have the 'command_update_badge_definitions' permission.");
                            break;
                        }

                        MoonEnvironment.GetGame().GetBadgeManager().Init();
                        MoonEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("definitions", Session.GetHabbo().Username + " ha atualizado las definiciones de placas.", ""));
                        break;
                    }
                default:
                    Session.SendWhisper("'" + UpdateVariable + "' é um comando inválido, escreva - o corretamente");
                    break;
            }
        }
    }
}
