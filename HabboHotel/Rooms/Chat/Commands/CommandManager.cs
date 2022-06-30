using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.Utilities;
using Moon.HabboHotel.Rooms;
using Moon.HabboHotel.GameClients;

using Moon.HabboHotel.Rooms.Chat.Commands.User;
using Moon.HabboHotel.Rooms.Chat.Commands.User.Fun;
using Moon.HabboHotel.Rooms.Chat.Commands.Moderator;
using Moon.HabboHotel.Rooms.Chat.Commands.Moderator.Fun;
using Moon.HabboHotel.Rooms.Chat.Commands.Administrator;

using Moon.Communication.Packets.Outgoing.Rooms.Chat;
using Moon.Communication.Packets.Outgoing.Notifications;
using Moon.Database.Interfaces;
using Moon.HabboHotel.Rooms.Chat.Commands.Events;
using Moon.HabboHotel.Items.Wired;
using Moon.Communication.Packets.Outgoing.Rooms.Notifications;
using Moon.HabboHotel.Rooms.Chat.Commands.User.Fan;

namespace Moon.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private string _prefix = ":";

        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;
        public  Dictionary<string, string> _commands2;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string Prefix)
        {
            this._prefix = Prefix;
            this._commands = new Dictionary<string, IChatCommand>();
            

            this.RegisterVIP();
            this.RegisterUser();
            this.RegisterEvents();
            this.RegisterModerator();
            this.RegisterAdministrator();
            //this.UpDateCommands2();
          
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="Session">Session calling this method.</param>
        /// <param name="Message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(GameClient Session, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null || MoonStaticGameSettings.IsGoingToBeClose)
                return false;

            if (!Message.StartsWith(_prefix))
                return false;

            Room room = Session.GetHabbo().CurrentRoom;

            if (room.GetFilter().CheckCommandFilter(Message))
                return false;

            if (Message == _prefix + "comandos" || Message == _prefix + "commands")
            {
                StringBuilder List = new StringBuilder();
                List<string> Comandos = new List<string>();
                List.Append("--------------------------------------\n");
                List.Append("I Comandos disponiveis para você [Usuário] I\n");
                List.Append("--------------------------------------\n");

                Comandos = MoonEnvironment.GetGame().GetPermissionManager().GetCommandsForID(1);
                foreach (string Comando in Comandos)
                {
                    foreach (var CmdList in _commands.ToList())
                    {

                        if (CmdList.Value.PermissionRequired == Comando) { List.Append("\n:" + CmdList.Key + " " + CmdList.Value.Parameters + " - " + CmdList.Value.Description + "\n"); }else { continue; }



                    }
                }
                
                if (Session.GetHabbo().Rank > 2)
                {

                    for (int i = 2; i <= Session.GetHabbo().Rank; i++) {

                        List.Append("\n---------------------------------------------------------------");
                        List.Append("\nComandos disponibles para [" + GetRankName(i) + "] ID [" + i + "]");
                        List.Append("\n---------------------------------------------------------------");
                        List.Append("\n");

                        Comandos = MoonEnvironment.GetGame().GetPermissionManager().GetCommandsForID(i);
                        foreach (string Comando in Comandos)
                        {
                            foreach (var CmdList in _commands.ToList())
                            {

                                if (CmdList.Value.PermissionRequired == Comando) { List.Append("\n:" + CmdList.Key + " " + CmdList.Value.Parameters + " - " + CmdList.Value.Description + "\n"); } else { continue; }



                            }
                        }


                    }

                }

                List.Append("\n Todos os comandos são registrados no banco de dados para evitar o abuso deles");



                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));



                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            IChatCommand Cmd = null;
            if (_commands.TryGetValue(Split[0].ToLower(), out Cmd))
            {
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    this.LogCommand(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId, Session.GetHabbo().Username, Session.GetHabbo().Look);

                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetHabbo().GetPermissions().HasCommand(Cmd.PermissionRequired))
                        return false;
                }


                Session.GetHabbo().IChatCommand = Cmd;
                Session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), this);

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            return false;
        }

        private string GetRankName( int i)
        {
            string RankName = "Undefined";
            #region RankNames (Switch)
            switch (i)
            {
                case 2:
                    RankName = "VIP";
                    break;
                case 3:
                    RankName = "PUB";
                    break;
                case 4:
                    RankName = "EDP";
                    break;
                case 5:
                    RankName = "LNC";
                    break;
                case 6:
                    RankName = "HELP";
                    break;
                case 7:
                    RankName = "EMB";
                    break;
                case 8:
                    RankName = "DJ";
                    break;
                case 9:
                    RankName = "BAW";
                    break;
                case 10:
                    RankName = "MD";
                    break;
                case 11:
                    RankName = "EDS";
                    break;
                case 12:
                    RankName = "EB";
                    break;
                case 13:
                    RankName = "CADM";
                    break;
                case 14:
                    RankName = "EDC";
                    break;
                case 15:
                    RankName = "ADM";
                    break;
                case 16:
                    RankName = "GRN";
                    break;
                case 17:
                    RankName = "FUN";
                    break;
                case 18:
                    RankName = "HIDE";
                    break;

            }
            #endregion
            return RankName;
        }
        /// <summary>
        /// Registers the VIP set of commands.
        /// </summary>
        private void RegisterVIP()
        {
            //USERS VIP
            this.Register("superpuxar", new SuperPullCommand());

            this.Register("superempurrar", new SuperPushCommand());

            this.Register("Moonwalk", new MoonwalkCommand());

            this.Register("bolhabot", new BubbleBotCommand());

            //Custom
            this.Register("nometamanho", new ChatHTMLSizeCommand());
            this.Register("emoji", new EmojiCommand());
            this.Register("tagremover", new PrefixCommand2());

            this.Register("bemvindo", new WelcomeCommand());
            this.Register("chatalerta", new ChatAlertCommand());

            this.Register("disparar", new CutCommand());
            this.Register("peido", new FartFaceCommand());
            this.Register("cortarcabeca", new CortarCabezaCommand());
            this.Register("queimar", new BurnCommand());

            this.Register("andarrapido", new FastwalkCommand());

            this.Register("usuarioson", new OnDutyCommand());

            this.Register("goto", new GOTOCommand());
            this.Register("ir", new GOTOCommand());

            this.Register("selfie", new SelfieCommand());

            this.Register("bolhas", new BubbleCommand());
        }

        /// <summary>
        /// Registers the Events set of commands.
        /// </summary>
        private void RegisterEvents()
        {
            this.Register("eha", new EventAlertCommand());
            this.Register("evento", new EventAlertCommand());

            this.Register("pha", new PublicityAlertCommand());

            this.Register("eventoda2", new Da2AlertCommand());

            this.Register("poll", new PollCommand());
            this.Register("pesquisa", new PollCommand());
            this.Register("endpoll", new EndPollCommand());

            this.Register("dj", new DJAlert());

            this.Register("notievento", new EventoExpress());

            this.Register("masspoll", new MassPollCommand());

            this.Register("notifica", new NotificaCommand());

            this.Register("megaoferta", new MegaOferta());

            this.Register("especial", new SpecialEvent());
            this.Register("ee", new SpecialEvent());

            this.Register("fbconcurso", new FacebookCommand());
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterUser()
        {
            //Nuevos - RANK: 1
            this.Register("chatdegrupo", new GroupChatCommand());
            //this.Register("chatdegrupo", new GroepChatCommand());

            this.Register("convertercreditos", new ConvertCreditsCommand());

            this.Register("converterdiamantes", new ConvertDiamondsCommand());

            this.Register("convertduckets", new ConvertDucketsCommand());

            this.Register("ocultarwired", new HideWiredCommand());

            this.Register("tipoevento", new EventSwapCommand());

            //this.Register("vipstatus", new ViewVIPStatusCommand());
  

            this.Register("cor", new ColourCommand());

            //this.Register("prefix", new PrefixCommand());



            //Normales
            this.Register("itemnamao", new CarryCommand());

            this.Register("sobre", new InfoCommand());

            this.Register("onlines", new OnlineCommand());

            this.Register("desativarsusurros", new DisableWhispersCommand());

            this.Register("copiar", new MimicCommand());

            this.Register("desativarcopiar", new DisableMimicCommand());

            this.Register("pet", new PetCommand());

            this.Register("mutarpets", new MutePetsCommand());

            this.Register("mutarbots", new MuteBotsCommand());

            this.Register("dancar", new DanceCommand());

            this.Register("empurrar", new PushCommand());

            this.Register("puxar", new PullCommand());

            this.Register("efeito", new EnableCommand());

            this.Register("seguir", new FollowCommand());

            this.Register("limparinventario", new EmptyItems());

            this.Register("desativarpedidos", new DisableFriends());

            this.Register("ativarpedidos", new EnableFriends());

            this.Register("desativarpresentes", new DisableGiftsCommand());

            this.Register("deitar", new LayCommand());

            this.Register("sentar", new SitCommand());

            this.Register("levantar", new StandCommand());

            this.Register("estatisticas", new StatsCommand());

            this.Register("dnd", new DNDCommand());


            //Como dueño de sala
            this.Register("pickall", new PickAllCommand());

            this.Register("ejectall", new EjectAllCommand());

            this.Register("construtor", new Builder());

            this.Register("recarregar", new Reloadcommand());

            this.Register("fixroom", new RegenMaps());

            this.Register("setmax", new SetMaxCommand());

            this.Register("setspeed", new SetSpeedCommand());

            this.Register("desativardiagonal", new DisableDiagonalCommand());

            
            //Custom
            this.Register("ajuda", new HelpCommand());

            this.Register("beijar", new KissCommand());

            this.Register("golpear", new GolpeCommand());

            this.Register("curar", new CurarCommand());

            this.Register("comprarquarto", new BuyRoomCommand());

            this.Register("venderquarto", new SellRoomCommand());

            this.Register("matar", new KillCommand());

            this.Register("ausente", new AfkCommand());

            //this.Register("violar", new SexCommand());

            this.Register("sexo", new SexoCommand());

            this.Register("wired", new WiredVariable());

            this.Register("desativarspam", new DisableSpamCommand());
            //this.Register("build", new BuildCommand());
            //this.Register("vipstatus", new ViewVIPStatusCommand());

            //this.Register("precios", new PriceList());
            //this.Register("custom", new CustomLegit());

            //Juegos Customs
            this.Register("apostar", new SetBetCommand());

            this.Register("casino", new CasinoCommand());

            this.Register("cerrardice", new CloseDiceCommand());

            //this.Register("flagme", new FlagMeCommand());
        }

        private void Register(string v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the moderator set of commands.
        /// </summary>
        private void RegisterModerator()
        {
            //RANK - 4
            this.Register("bp", new BanPubliCommand());

            this.Register("tag", new PrefixNameCommand());

            this.Register("sa", new StaffAlertCommand());

            this.Register("coordenadas", new CoordsCommand());

            this.Register("ignorarsusurro", new IgnoreWhispersCommand());

            this.Register("desativarefeitos", new DisableForcedFXCommand());


            //RANK - 6
            this.Register("troll", new TrollAlert());

            this.Register("trollusers", new TrollAlertUser());

            this.Register("tele", new TeleportCommand());

            this.Register("override", new OverrideCommand());

            this.Register("andarsuperrapido", new SuperFastwalkCommand());

            this.Register("forcarsentar", new ForceSitCommand());

            this.Register("forcardeitar", new ForceLay());

            this.Register("forcarstand", new ForceStand());

            this.Register("userson", new ViewOnlineCommand());

            this.Register("boom", new GoBoomCommand());

            this.Register("spam", new spamCommand());


            //RANK - 7
            this.Register("summon", new SummonCommand());
            this.Register("trazer", new SummonCommand());


            //RANK - 10
            this.Register("ui", new UserInfoCommand());
            this.Register("userinfo", new UserInfoCommand());

            this.Register("desmutarsala", new RoomUnmuteCommand());

            this.Register("mutarsala", new RoomMuteCommand());

            this.Register("roomalert", new RoomAlertCommand());
            this.Register("quartoalerta", new RoomAlertCommand());

            this.Register("roomkick", new RoomKickCommand());
            this.Register("kickartodos", new RoomKickCommand());

            this.Register("mutar", new MuteCommand());

            this.Register("desmutar", new UnmuteCommand());

            this.Register("kickar", new KickCommand());
            this.Register("kick", new KickCommand());

            this.Register("dc", new DisconnectCommand());
            this.Register("desconectar", new DisconnectCommand());

            this.Register("alertar", new AlertCommand());

            this.Register("tradeban", new TradeBanCommand());

            this.Register("congelar", new FreezeCommand());

            this.Register("descongelar", new UnFreezeCommand());


            //RANK - 11
            this.Register("customalert", new customalertCommand());
            
            this.Register("ban", new BanCommand());

            this.Register("mip", new MIPCommand());
            this.Register("ipban", new IPBanCommand());

            this.Register("ha", new HotelAlertCommand());
            this.Register("hotelalert", new HotelAlertCommand());

            this.Register("lastmsg", new LastMessagesCommand());
            this.Register("verhistorico", new LastMessagesCommand());

            this.Register("lastconsolemsg", new LastConsoleMessagesCommand());
            this.Register("verhistoricodoconsole", new LastConsoleMessagesCommand());

            this.Register("enviarusuario", new SendUserCommand());

            this.Register("makesay", new MakeSayCommand());

            this.Register("mudardenome", new FlagUserCommand());

            this.Register("quartopublico", new MakePublicCommand());

            this.Register("quartoprivado", new MakePrivateCommand());


            //RANK - 12
            this.Register("roombadge", new RoomBadgeCommand());
            this.Register("quartoemblema", new RoomBadgeCommand());

            this.Register("givebadge", new GiveBadgeCommand());
            this.Register("daremblema", new GiveBadgeCommand());

            this.Register("give", new GiveCommand());
            this.Register("dar", new GiveCommand());

            this.Register("massenable", new MassEnableCommand());
            this.Register("efeitostodos", new MassEnableCommand());

            this.Register("massdance", new MassDanceCommand());
            this.Register("dancartodos", new MassDanceCommand());

            this.Register("premiar", new PremiarCommand());


            //RANK - 13
            this.Register("roomgive", new RoomGiveCommand());

            this.Register("massbadge", new MassBadgeCommand());
            this.Register("hotelemblema", new MassBadgeCommand());

            this.Register("massgive", new MassGiveCommand());
            this.Register("hoteleconomia", new MassGiveCommand());
            this.Register("daratodos", new MassGiveCommand());

            this.Register("hal", new HALCommand());
            this.Register("alertaurl", new HALCommand());


            //RANK - 14
            this.Register("unban", new UnBanCommand());
            this.Register("removerban", new UnBanCommand());


            //RANK - 15
            this.Register("addblackword", new FilterCommand());
            this.Register("filtro", new FilterCommand());


            //RANK - 16
            this.Register("rank", new GiveRanksCommand());
        }

        /// <summary>
        /// Registers the administrator set of commands.
        /// </summary>
        private void RegisterAdministrator()
        {
            //RANK - 10
            this.Register("verclones", new VerClonesCommand());

            this.Register("verinventario", new ViewInventaryCommand());


            //RANK - 11
            this.Register("deletegroup", new DeleteGroupCommand());
            this.Register("deletargrupo", new DeleteGroupCommand());

            this.Register("addtag", new AddTagsToUserCommand());

            this.Register("ca", new CustomizedHotelAlert());


            //RANK - 13
            this.Register("summonall", new SummonAll());
            this.Register("trazertodos", new SummonAll());

            this.Register("givespecial", new GiveSpecialReward());

            this.Register("massevent", new MassiveEventCommand());

            this.Register("removeremblema", new RemoveBadgeCommand());

            this.Register("ia", new SendGraphicAlertCommand());

            this.Register("iau", new SendImageToUserCommand());

            this.Register("viewevents", new ViewStaffEventListCommand());
            this.Register("vereventos", new ViewStaffEventListCommand());


            //RANK - 14
            this.Register("staffson", new StaffInfo());
            this.Register("staffons", new StaffInfo());

            this.Register("emptyuser", new EmptyUser());
            this.Register("limparinventariode", new EmptyUser());


            //RANK - 15
            this.Register("darvip", new ReloadUserrVIPRankCommand());

            this.Register("alerttype", new AlertSwapCommand());


            //RANK - 16
            this.Register("addpredesigned", new AddPredesignedCommand());
            this.Register("addpackdesala", new AddPredesignedCommand());

            this.Register("removepredesigned", new RemovePredesignedCommand());
            this.Register("removerpackdoquarto", new RemovePredesignedCommand());

            this.Register("update", new UpdateCommand());
            this.Register("atualizar", new UpdateCommand());

            this.Register("item", new UpdateFurniture());

            this.Register("dcall", new DesconectarnAll());

            this.Register("shutdown", new ShutdownCommand());
            this.Register("reiniciar", new RestartCommand());

            this.Register("voucher", new VoucherCommand());


            //NUEVOS
            this.Register("forcebox", new ForceFurniMaticBoxCommand());

            this.Register("mw", new MultiwhisperModeCommand());

            this.Register("progresso", new ProgressAchievementCommand());

            this.Register("controlar", new ControlCommand());

            this.Register("dice", new ForceDiceCommand());

            this.Register("link", new LinkStaffCommand());
        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="CommandText">Text to type for this command.</param>
        /// <param name="Command">The command to execute.</param>
        public void Register(string CommandText, IChatCommand Command)
        {
            this._commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(" ");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void LogCommand(int UserId, string Data, string MachineId, string Username, string Look)
        {
            using (IQueryAdapter dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", MoonEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }

            if (Data == "regenmaps" || Data.StartsWith("c") || Data == "sa" || Data == "ga" || Data == "info" || UserId == 3)
            { return; }

            else
            MoonEnvironment.GetGame().GetClientManager().ManagerAlert(RoomNotificationComposer.SendBubble("fig/" + Look, "" + Username + "\n\nUsou o comando:\n:" + Data + ".", ""));
        }

        public bool TryGetCommand(string Command, out IChatCommand IChatCommand)
        {
            return this._commands.TryGetValue(Command, out IChatCommand);
        }
    }
}
