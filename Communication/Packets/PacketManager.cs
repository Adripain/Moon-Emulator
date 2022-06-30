using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using log4net;

using Moon.Core;
using Moon.Communication.Packets.Incoming;
using Moon.HabboHotel.GameClients;

using Moon.Communication.Packets.Incoming.Catalog;
using Moon.Communication.Packets.Incoming.Handshake;
using Moon.Communication.Packets.Incoming.Navigator;
using Moon.Communication.Packets.Incoming.Quests;
using Moon.Communication.Packets.Incoming.Rooms.Avatar;
using Moon.Communication.Packets.Incoming.Rooms.Chat;
using Moon.Communication.Packets.Incoming.Rooms.Connection;
using Moon.Communication.Packets.Incoming.Rooms.Engine;
using Moon.Communication.Packets.Incoming.Rooms.Action;
using Moon.Communication.Packets.Incoming.Users;
using Moon.Communication.Packets.Incoming.Inventory.AvatarEffects;
using Moon.Communication.Packets.Incoming.Inventory.Purse;
using Moon.Communication.Packets.Incoming.Sound;
using Moon.Communication.Packets.Incoming.Misc;
using Moon.Communication.Packets.Incoming.Inventory.Badges;
using Moon.Communication.Packets.Incoming.Avatar;
using Moon.Communication.Packets.Incoming.Inventory.Achievements;
using Moon.Communication.Packets.Incoming.Inventory.Bots;
using Moon.Communication.Packets.Incoming.Inventory.Pets;
using Moon.Communication.Packets.Incoming.LandingView;
using Moon.Communication.Packets.Incoming.Messenger;
using Moon.Communication.Packets.Incoming.Groups;
using Moon.Communication.Packets.Incoming.Rooms.Settings;
using Moon.Communication.Packets.Incoming.Rooms.AI.Pets;
using Moon.Communication.Packets.Incoming.Rooms.AI.Bots;
using Moon.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using Moon.Communication.Packets.Incoming.Rooms.Furni;
using Moon.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces;
using Moon.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions;
using Moon.Communication.Packets.Incoming.Help;
using Moon.Communication.Packets.Incoming.Rooms.FloorPlan;
using Moon.Communication.Packets.Incoming.Rooms.Furni.Wired;
using Moon.Communication.Packets.Incoming.Moderation;
using Moon.Communication.Packets.Incoming.Inventory.Furni;
using Moon.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using Moon.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using Moon.Communication.Packets.Incoming.Inventory.Trading;
using Moon.Communication.Packets.Incoming.GameCenter;
using Moon.Communication.Packets.Incoming.Marketplace;
using Moon.Communication.Packets.Incoming.Rooms.Furni.LoveLocks;
using Moon.Communication.Packets.Incoming.Talents;
using Moon.Communication.Packets.Incoming.Rooms.Camera;
using Moon.Communication.Packets.Incoming.Nucs;
using Moon.Communication.Packets.Incoming.QuickPolls;
using Moon.Communication.Packets.Incoming.Help.Helpers;
using Moon.HabboHotel.Rooms.Instance;
using Moon.Communication.Packets.Incoming.Rooms.Music;
using Moon.Communication.Packets.Incoming.Rooms.Polls;
using Moon.Communication.Packets.Incoming.Calendar;
using Moon.Communication.Packets.Incoming.Groups.Forums;
using Moon.Communication.Packets.Incoming.Rooms.Nux;

namespace Moon.Communication.Packets
{
    public sealed class PacketManager
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.Communication.Packets");

        /// <summary>
        ///     Testing the Task code
        /// </summary>
        private readonly bool IgnoreTasks = true;

        /// <summary>
        ///     The maximum time a task can run for before it is considered dead
        ///     (can be used for debugging any locking issues with certain areas of code)
        /// </summary>
        private readonly int MaximumRunTimeInSec = 300; // 5 minutes

        /// <summary>
        ///     Should the handler throw errors or log and continue.
        /// </summary>
        private readonly bool ThrowUserErrors = false;

        /// <summary>
        ///     The task factory which is used for running Asynchronous tasks, in this case we use it to execute packets.
        /// </summary>
        private readonly TaskFactory _eventDispatcher;

        private readonly Dictionary<int, IPacketEvent> _incomingPackets;
        //private readonly Dictionary<int, string> _packetNames;

        /// <summary>
        ///     Currently running tasks to keep track of what the current load is
        /// </summary>
        private readonly ConcurrentDictionary<int, Task> _runningTasks;

        public PacketManager()
        {
            this._incomingPackets = new Dictionary<int, IPacketEvent>();

            this._eventDispatcher = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);
            this._runningTasks = new ConcurrentDictionary<int, Task>();
            //this._packetNames = new Dictionary<int, string>();

            this.RegisterHandshake();
            this.RegisterAdventCalendar();
            this.RegisterLandingView();
            this.RegisterCatalog();
            this.RegisterMarketplace();
            this.RegisterNavigator();
            this.RegisterNewNavigator();
            this.RegisterRoomAction();
            this.RegisterHelperTool();
            this.RegisterQuests();
            this.RegisterRoomConnection();
            this.RegisterRoomChat();
            this.RegisterRoomEngine();
            this.RegisterFurni();
            this.RegisterUsers();
            this.RegisterSound();
            this.RegisterMisc();
            this.RegisterInventory();
            this.RegisterTalents();
            this.RegisterForums();
            this.RegisterPurse();
            this.RegisterRoomAvatar();
            this.RegisterAvatar();
            this.RegisterMessenger();
            this.RegisterGroups();
            this.RegisterRoomSettings();
            this.RegisterPets();
            this.RegisterBots();
            this.RegisterHelp();
            this.FloorPlanEditor();
            this.RegisterPolls();
            this.RegisterRoomPolls();
            this.RegisterModeration();
            this.RegisterGameCenter();
            //this.RegisterNames();
            this.RegisterRoomCamera();
            this.RegistrarNucsEventos(); // engespañol
        }

        public void TryExecutePacket(GameClient Session, ClientPacket Packet)
        {
            IPacketEvent Pak = null;

            if (!_incomingPackets.TryGetValue(Packet.Id, out Pak))
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    log.Debug("Unhandled Packet: " + Packet.ToString());
                return;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                log.Debug("Handled Packet: [" + Packet.Id + "] " + Pak.GetType().Name);
            }

            if (!IgnoreTasks)
                ExecutePacketAsync(Session, Packet, Pak);
            else
                Pak.Parse(Session, Packet);
        }

        private void ExecutePacketAsync(GameClient Session, ClientPacket Packet, IPacketEvent Pak)
        {
            DateTime Start = DateTime.Now;

            var CancelSource = new CancellationTokenSource();
            CancellationToken Token = CancelSource.Token;

            Task t = _eventDispatcher.StartNew(() =>
            {
                Pak.Parse(Session, Packet);
                Token.ThrowIfCancellationRequested();
            }, Token);

            _runningTasks.TryAdd(t.Id, t);

            try
            {
                if (!t.Wait(MaximumRunTimeInSec * 1000, Token))
                {
                    CancelSource.Cancel();
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception e in ex.Flatten().InnerExceptions)
                {
                    if (ThrowUserErrors)
                    {
                        throw e;
                    }
                    else
                    {
                        //log.Fatal("Unhandled Error: " + e.Message + " - " + e.StackTrace);
                        Session.Disconnect();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Session.Disconnect();
            }
            finally
            {
                Task RemovedTask = null;
                _runningTasks.TryRemove(t.Id, out RemovedTask);

                CancelSource.Dispose();

                //log.Debug("Event took " + (DateTime.Now - Start).Milliseconds + "ms to complete.");
            }
        }

        public void WaitForAllToComplete()
        {
            foreach (Task t in this._runningTasks.Values.ToList())
            {
                t.Wait();
            }
        }

        public void UnregisterAll()
        {
            this._incomingPackets.Clear();
        }

        private void RegisterHandshake()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetClientVersionMessageEvent, new GetClientVersionEvent());
            this._incomingPackets.Add(ClientPacketHeader.InitCryptoMessageEvent, new InitCryptoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            this._incomingPackets.Add(ClientPacketHeader.UniqueIDMessageEvent, new UniqueIDEvent());
            this._incomingPackets.Add(ClientPacketHeader.SSOTicketMessageEvent, new SSOTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.InfoRetrieveMessageEvent, new InfoRetrieveEvent());
            this._incomingPackets.Add(ClientPacketHeader.PingMessageEvent, new PingEvent());

        }

        private void RegisterLandingView()
        {
            this._incomingPackets.Add(ClientPacketHeader.RefreshCampaignMessageEvent, new RefreshCampaignEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPromoArticlesMessageEvent, new GetPromoArticlesEvent());
            this._incomingPackets.Add(ClientPacketHeader.RequestBonusRareEvent, new RequestBonusRareEvent());
            this._incomingPackets.Add(ClientPacketHeader.GiveConcurrentUsersRewardEvent, new GiveConcurrentUsersReward());
            this._incomingPackets.Add(ClientPacketHeader.ConcurrentUsersCompetitionEvent, new ConcurrentUsersCompetition());
            this._incomingPackets.Add(ClientPacketHeader.VoteCommunityGoalVS, new VoteCommunityGoalVS());
            this._incomingPackets.Add(ClientPacketHeader.CommunityGoalEvent, new CommunityGoalEvent());
        }

        private void RegisterRoomCamera()
        {
            this._incomingPackets.Add(ClientPacketHeader.RenderRoomMessageComposer, new RenderRoomMessageComposerEvent());
            this._incomingPackets.Add(ClientPacketHeader.RenderRoomMessageComposerBigPhoto, new RenderRoomMessageComposerBigPhoto());
            this._incomingPackets.Add(ClientPacketHeader.BuyServerCameraPhoto, new BuyServerCameraPhoto());

        }

        private void RegistrarNucsEventos()
        {

            this._incomingPackets.Add(ClientPacketHeader.RoomNucsAlerta, new RoomNucsAlerta());
        }

        private void RegisterCatalog()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogModeMessageEvent, new GetCatalogModeEvent());
            this._incomingPackets.Add(ClientPacketHeader.BuyTargettedOfferMessageEvent, new BuyTargettedOfferMessage());
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogIndexMessageEvent, new GetCatalogIndexEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogPageMessageEvent, new GetCatalogPageEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCatalogOfferMessageEvent, new GetCatalogOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogMessageEvent, new PurchaseFromCatalogEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogAsGiftMessageEvent, new PurchaseFromCatalogAsGiftEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseRoomPromotionMessageEvent, new PurchaseRoomPromotionEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent, new GetGiftWrappingConfigurationEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceConfigurationMessageEvent, new GetMarketplaceConfigurationEvent());
            this._incomingPackets.Add(ClientPacketHeader.CheckPetNameMessageEvent, new CheckPetNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.RedeemVoucherMessageEvent, new RedeemVoucherEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetSellablePetBreedsMessageEvent, new GetSellablePetBreedsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPromotableRoomsMessageEvent, new GetPromotableRoomsEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetCatalogRoomPromotionMessageEvent, new GetCatalogRoomPromotionEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetNuxPresentEvent, new GetNuxPresentEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupFurniConfigMessageEvent, new GetGroupFurniConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.CheckGnomeNameMessageEvent, new CheckGnomeNameEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetClubGiftsMessageEvent, new GetClubGiftsEvent());
            this._incomingPackets.Add(ClientPacketHeader.LTDCountdownEvent, new LTDCountdownEvent());
            this._incomingPackets.Add(ClientPacketHeader.RedeemHCGiftEvent, new RedeemHCGiftEvent());
            //this._incomingPackets.Add(ClientPacketHeader.ConcurrentUsersCompetition, new ConcurrentUsersCompetition());
            this._incomingPackets.Add(ClientPacketHeader.FurniMaticPageEvent, new FurniMaticPageEvent());
            this._incomingPackets.Add(ClientPacketHeader.FurniMaticRecycleEvent, new FurniMaticRecycleEvent());
            this._incomingPackets.Add(ClientPacketHeader.FurniMaticRewardsEvent, new FurniMaticRewardsEvent());

        }

        private void RegisterMarketplace()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetOffersMessageEvent, new GetOffersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetOwnOffersMessageEvent, new GetOwnOffersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent, new GetMarketplaceCanMakeOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceItemStatsMessageEvent, new GetMarketplaceItemStatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.MakeOfferMessageEvent, new MakeOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelOfferMessageEvent, new CancelOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.BuyOfferMessageEvent, new BuyOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.RedeemOfferCreditsMessageEvent, new RedeemOfferCreditsEvent());
        }

        private void RegisterNavigator()
        {
            this._incomingPackets.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, new AddFavouriteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, new GetUserFlatCatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, new RemoveFavouriteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.GoToHotelViewMessageEvent, new GoToHotelViewEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, new UpdateNavigatorSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.CreateFlatMessageEvent, new CreateFlatEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGuestRoomMessageEvent, new GetGuestRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.EditRoomPromotionMessageEvent, new EditRoomEventEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetEventCategoriesMessageEvent, new GetNavigatorFlatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.StaffPickRoomEvent, new StaffPickRoomEvent());
        }

        public void RegisterNewNavigator()
        {
            this._incomingPackets.Add(ClientPacketHeader.InitializeNewNavigatorMessageEvent, new InitializeNewNavigatorEvent());
            this._incomingPackets.Add(ClientPacketHeader.NewNavigatorSearchMessageEvent, new NewNavigatorSearchEvent());
            this._incomingPackets.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, new FindRandomFriendingRoomEvent());
        }

        private void RegisterQuests()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetQuestListMessageEvent, new GetQuestListEvent());
            this._incomingPackets.Add(ClientPacketHeader.StartQuestMessageEvent, new StartQuestEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelQuestMessageEvent, new CancelQuestEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCurrentQuestMessageEvent, new GetCurrentQuestEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetDailyQuestMessageEvent, new GetDailyQuestEvent());
            //this._incomingPackets.Add(ClientPacketHeader.GetCommunityGoalHallOfFameMessageEvent, new GetCommunityGoalHallOfFameEvent());
        }

        private void RegisterHelp()
        {
            this._incomingPackets.Add(ClientPacketHeader.OnBullyClickMessageEvent, new OnBullyClickEvent());
            this._incomingPackets.Add(ClientPacketHeader.SendBullyReportMessageEvent, new SendBullyReportEvent());
            this._incomingPackets.Add(ClientPacketHeader.SubmitBullyReportMessageEvent, new SubmitBullyReportEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetSanctionStatusMessageEvent, new GetSanctionStatusEvent());
        }

        private void RegisterRoomAction()
        {
            this._incomingPackets.Add(ClientPacketHeader.LetUserInMessageEvent, new LetUserInEvent());
            this._incomingPackets.Add(ClientPacketHeader.BanUserMessageEvent, new BanUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.KickUserMessageEvent, new KickUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.AssignRightsMessageEvent, new AssignRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveRightsMessageEvent, new RemoveRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, new RemoveAllRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.MuteUserMessageEvent, new MuteUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.AmbassadorWarningMessageEvent, new AmbassadorWarningMessageEvent());
            this._incomingPackets.Add(ClientPacketHeader.GiveHandItemMessageEvent, new GiveHandItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveMyRightsMessageEvent, new RemoveMyRightsEvent());
        }
        private void RegisterHelperTool()
        {
            this._incomingPackets.Add(ClientPacketHeader.HandleHelperToolMessageEvent, new HandleHelperToolEvent());
            this._incomingPackets.Add(ClientPacketHeader.CallForHelperMessageEvent, new CallForHelperEvent());
            this._incomingPackets.Add(ClientPacketHeader.AcceptHelperSessionMessageEvent, new AcceptHelperSessionEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelCallForHelperMessageEvent, new CancelCallForHelperEvent());
            this._incomingPackets.Add(ClientPacketHeader.FinishHelperSessionMessageEvent, new FinishHelperSessionEvent());
            this._incomingPackets.Add(ClientPacketHeader.CloseHelperChatSessionMessageEvent, new CloseHelperChatSessionEvent());
            this._incomingPackets.Add(ClientPacketHeader.HelperSessioChatTypingMessageEvent, new HelperSessioChatTypingEvent());
            this._incomingPackets.Add(ClientPacketHeader.HelperSessioChatSendMessageMessageEvent, new HelperSessioChatSendMessageEvent());
            this._incomingPackets.Add(ClientPacketHeader.InvinteHelperUserSessionMessageEvent, new InvinteHelperUserSessionEvent());
            this._incomingPackets.Add(ClientPacketHeader.VisitHelperUserSessionMessageEvent, new VisitHelperUserSessionEvent());
            this._incomingPackets.Add(ClientPacketHeader.ReportBullyUserMessageEvent, new ReportBullyUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.AcceptJoinJudgeChatMessageEvent, new AcceptJoinJudgeChatEvent());
        }
        private void RegisterAvatar()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetWardrobeMessageEvent, new GetWardrobeEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, new SaveWardrobeOutfitEvent());
        }

        private void RegisterRoomAvatar()
        {
            this._incomingPackets.Add(ClientPacketHeader.ActionMessageEvent, new ActionEvent());
            this._incomingPackets.Add(ClientPacketHeader.ApplySignMessageEvent, new ApplySignEvent());
            this._incomingPackets.Add(ClientPacketHeader.DanceMessageEvent, new DanceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SitMessageEvent, new SitEvent());
            this._incomingPackets.Add(ClientPacketHeader.ChangeMottoMessageEvent, new ChangeMottoEvent());
            this._incomingPackets.Add(ClientPacketHeader.LookToMessageEvent, new LookToEvent());
            this._incomingPackets.Add(ClientPacketHeader.DropHandItemMessageEvent, new DropHandItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.GiveRoomScoreMessageEvent, new GiveRoomScoreEvent());
            this._incomingPackets.Add(ClientPacketHeader.IgnoreUserMessageEvent, new IgnoreUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.UnIgnoreUserMessageEvent, new UnIgnoreUserEvent());
        }

        private void RegisterRoomConnection()
        {
            this._incomingPackets.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, new OpenFlatConnectionEvent());
            this._incomingPackets.Add(ClientPacketHeader.GoToFlatMessageEvent, new GoToFlatEvent());
        }

        private void RegisterRoomChat()
        {
            this._incomingPackets.Add(ClientPacketHeader.ChatMessageEvent, new ChatEvent());
            this._incomingPackets.Add(ClientPacketHeader.ShoutMessageEvent, new ShoutEvent());
            this._incomingPackets.Add(ClientPacketHeader.WhisperMessageEvent, new WhisperEvent());
            this._incomingPackets.Add(ClientPacketHeader.StartTypingMessageEvent, new StartTypingEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelTypingMessageEvent, new CancelTypingEvent());
        }

        private void RegisterRoomEngine()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, new GetRoomEntryDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.GoToFlatAsSpectatorEvent, new GoToFlatAsSpectatorEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, new GetFurnitureAliasesEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveAvatarMessageEvent, new MoveAvatarEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveObjectMessageEvent, new MoveObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickupObjectMessageEvent, new PickupObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveWallItemMessageEvent, new MoveWallItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.ApplyDecorationMessageEvent, new ApplyDecorationEvent());
            this._incomingPackets.Add(ClientPacketHeader.PlaceObjectMessageEvent, new PlaceObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseFurnitureMessageEvent, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseWallItemMessageEvent, new UseWallItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.PlaceBuilderItemMessageEvent, new PlaceBuilderObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.EventTrackerMessageEvent, new EventTrackerEvent());
        }

        private void RegisterInventory()
        {
            this._incomingPackets.Add(ClientPacketHeader.InitTradeMessageEvent, new InitTradeEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingOfferItemMessageEvent, new TradingOfferItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingOfferItemsMessageEvent, new TradingOfferItemsEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingRemoveItemMessageEvent, new TradingRemoveItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingAcceptMessageEvent, new TradingAcceptEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingCancelMessageEvent, new TradingCancelEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingConfirmMessageEvent, new TradingConfirmEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingModifyMessageEvent, new TradingModifyEvent());
            this._incomingPackets.Add(ClientPacketHeader.TradingCancelConfirmMessageEvent, new TradingCancelConfirmEvent());
            this._incomingPackets.Add(ClientPacketHeader.RequestFurniInventoryMessageEvent, new RequestFurniInventoryEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBadgesMessageEvent, new GetBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetAchievementsMessageEvent, new GetAchievementsEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetActivatedBadgesMessageEvent, new SetActivatedBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBotInventoryMessageEvent, new GetBotInventoryEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPetInventoryMessageEvent, new GetPetInventoryEvent());
            this._incomingPackets.Add(ClientPacketHeader.AvatarEffectActivatedMessageEvent, new AvatarEffectActivatedEvent());
            this._incomingPackets.Add(ClientPacketHeader.AvatarEffectSelectedMessageEvent, new AvatarEffectSelectedEvent());
        }

        private void RegisterTalents()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetTalentTrackMessageEvent, new GetTalentTrackEvent());
            this._incomingPackets.Add(ClientPacketHeader.CheckQuizTypeEvent, new CheckQuizType());
            this._incomingPackets.Add(ClientPacketHeader.PostQuizAnswersMessageEvent, new PostQuizAnswersMessage());

        }

        private void RegisterPolls()
        {
            this._incomingPackets.Add(ClientPacketHeader.SubmitPollAnswerMessageEvent, new SubmitPollAnswerMessageEvent());

        }

        private void RegisterPurse()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetCreditsInfoMessageEvent, new GetCreditsInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetHabboClubWindowMessageEvent, new GetHabboClubWindowEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetHabboClubCenterInfoMessageEvent, new GetHabboClubCenterInfoMessageEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCameraPriceEvent, new GetCameraPriceEvent());
        }

        private void RegisterUsers()
        {
            this._incomingPackets.Add(ClientPacketHeader.ScrGetUserInfoMessageEvent, new ScrGetUserInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetChatPreferenceMessageEvent, new SetChatPreferenceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetUserFocusPreferenceEvent, new SetUserFocusPreferenceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetMessengerInviteStatusMessageEvent, new SetMessengerInviteStatusEvent());
            this._incomingPackets.Add(ClientPacketHeader.RespectUserMessageEvent, new RespectUserEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, new UpdateFigureDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.OpenPlayerProfileMessageEvent, new OpenPlayerProfileEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, new GetSelectedBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRelationshipsMessageEvent, new GetRelationshipsEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetRelationshipMessageEvent, new SetRelationshipEvent());
            this._incomingPackets.Add(ClientPacketHeader.CheckValidNameMessageEvent, new CheckValidNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.ChangeNameMessageEvent, new ChangeNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetUsernameMessageEvent, new SetUsernameEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, new GetHabboGroupBadgesEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetUserTagsMessageEvent, new GetUserTagsEvent());
        }

        private void RegisterSound()
        {
            this._incomingPackets.Add(ClientPacketHeader.SetSoundSettingsMessageEvent, new SetSoundSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.AddPlaylistItemMessageEvent, new AddPlaylistItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetJukeboxDisksMessageEvent, new GetJukeboxDisksEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetJukeboxPlaylistsMessageEvent, new GetJukeboxPlaylistsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMusicDataMessageEvent, new GetMusicDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemovePlaylistItemMessageEvent, new RemovePlaylistItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.SyncMusicMessageEvent, new SyncMusicEvent());
        }


        private void RegisterMisc()
        {
            this._incomingPackets.Add(ClientPacketHeader.UnknownQuestMessageEvent, new GetQuestListEvent());
            this._incomingPackets.Add(ClientPacketHeader.ClientVariablesMessageEvent, new ClientVariablesEvent());
            this._incomingPackets.Add(ClientPacketHeader.DisconnectionMessageEvent, new DisconnectEvent());
            this._incomingPackets.Add(ClientPacketHeader.LatencyTestMessageEvent, new LatencyTestEvent());
            this._incomingPackets.Add(ClientPacketHeader.MemoryPerformanceMessageEvent, new MemoryPerformanceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetFriendBarStateMessageEvent, new SetFriendBarStateEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetAdsOfferEvent, new GetAdsOfferEvent());
        }


        private void RegisterMessenger()
        {
            this._incomingPackets.Add(ClientPacketHeader.MessengerInitMessageEvent, new MessengerInitEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, new GetBuddyRequestsEvent());
            this._incomingPackets.Add(ClientPacketHeader.FollowFriendMessageEvent, new FollowFriendEvent());
            this._incomingPackets.Add(ClientPacketHeader.FindNewFriendsMessageEvent, new FindNewFriendsEvent());
            this._incomingPackets.Add(ClientPacketHeader.FriendListUpdateMessageEvent, new FriendListUpdateEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveBuddyMessageEvent, new RemoveBuddyEvent());
            this._incomingPackets.Add(ClientPacketHeader.RequestBuddyMessageEvent, new RequestBuddyEvent());
            this._incomingPackets.Add(ClientPacketHeader.SendMsgMessageEvent, new SendMsgEvent());
            this._incomingPackets.Add(ClientPacketHeader.SendRoomInviteMessageEvent, new SendRoomInviteEvent());
            this._incomingPackets.Add(ClientPacketHeader.HabboSearchMessageEvent, new HabboSearchEvent());
            this._incomingPackets.Add(ClientPacketHeader.AcceptBuddyMessageEvent, new AcceptBuddyEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeclineBuddyMessageEvent, new DeclineBuddyEvent());
        }

        public void RegisterAdventCalendar()
        {
            this._incomingPackets.Add(ClientPacketHeader.OpenCalendarBoxMessageEvent, new OpenCalendarBoxEvent());
        }

        private void RegisterGroups()
        {
            this._incomingPackets.Add(ClientPacketHeader.JoinGroupMessageEvent, new JoinGroupEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveGroupFavouriteMessageEvent, new RemoveGroupFavouriteEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetGroupFavouriteMessageEvent, new SetGroupFavouriteEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupInfoMessageEvent, new GetGroupInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupMembersMessageEvent, new GetGroupMembersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupCreationWindowMessageEvent, new GetGroupCreationWindowEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetBadgeEditorPartsMessageEvent, new GetBadgeEditorPartsEvent());
            this._incomingPackets.Add(ClientPacketHeader.PurchaseGroupMessageEvent, new PurchaseGroupEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupIdentityMessageEvent, new UpdateGroupIdentityEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupBadgeMessageEvent, new UpdateGroupBadgeEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupColoursMessageEvent, new UpdateGroupColoursEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateGroupSettingsMessageEvent, new UpdateGroupSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.ManageGroupMessageEvent, new ManageGroupEvent());
            this._incomingPackets.Add(ClientPacketHeader.GiveAdminRightsMessageEvent, new GiveAdminRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.TakeAdminRightsMessageEvent, new TakeAdminRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveGroupMemberMessageEvent, new RemoveGroupMemberEvent());
            this._incomingPackets.Add(ClientPacketHeader.AcceptGroupMembershipMessageEvent, new AcceptGroupMembershipEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeclineGroupMembershipMessageEvent, new DeclineGroupMembershipEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteGroupMessageEvent, new DeleteGroupEvent());
        }

        private void RegisterForums()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetForumsListDataMessageEvent, new GetForumsListDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetForumStatsMessageEvent, new GetForumStatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetThreadsListDataMessageEvent, new GetThreadsListDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetThreadDataMessageEvent, new GetThreadDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.PostGroupContentMessageEvent, new PostGroupContentEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteGroupThreadMessageEvent, new DeleteGroupThreadEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateForumSettingsMessageEvent, new UpdateForumSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateThreadMessageEvent, new UpdateForumThreadStatusEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteGroupPostMessageEvent, new DeleteGroupPostEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupForumsMessageEvent, new GetForumsListDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetForumUserProfileMessageEvent, new GetForumUserProfileEvent());
        }


        /* private void RegisterForums()
           {
               this._incomingPackets.Add(ClientPacketHeader.AlterForumThreadStateMessageEvent, new GetForumsListDataEvent());
               this._incomingPackets.Add(ClientPacketHeader.GetGroupForumDataMessageEvent, new GetForumsListDataEvent());
               
               this._incomingPackets.Add(ClientPacketHeader.GetGroupForumThreadRootMessageEvent, new GetGroupInfoEvent());
               this._incomingPackets.Add(ClientPacketHeader.PublishForumThreadMessageEvent, new PostGroupContentEvent());
               this._incomingPackets.Add(ClientPacketHeader.ReadForumThreadMessageEvent, new UpdateForumReadMarkerEvent());
               this._incomingPackets.Add(ClientPacketHeader.UpdateForumSettingsMessageEvent, new UpdateForumSettingsEvent());
               this._incomingPackets.Add(ClientPacketHeader.UpdateThreadMessageEvent, new UpdateForumThreadStatusEvent());
               this._incomingPackets.Add(ClientPacketHeader.GetThreadsListDataMessageEvent, new GetThreadsListDataEvent());
           }*/


        private void RegisterRoomSettings()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, new GetRoomSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, new SaveRoomSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteRoomMessageEvent, new DeleteRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.ToggleMuteToolMessageEvent, new ToggleMuteToolEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRoomFilterListMessageEvent, new GetRoomFilterListEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModifyRoomFilterListMessageEvent, new ModifyRoomFilterListEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRoomRightsMessageEvent, new GetRoomRightsEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRoomBannedUsersMessageEvent, new GetRoomBannedUsersEvent());
            this._incomingPackets.Add(ClientPacketHeader.UnbanUserFromRoomMessageEvent, new UnbanUserFromRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveEnforcedCategorySettingsMessageEvent, new SaveEnforcedCategorySettingsEvent());
        }

        private void RegisterPets()
        {
            this._incomingPackets.Add(ClientPacketHeader.RespectPetMessageEvent, new RespectPetEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPetInformationMessageEvent, new GetPetInformationEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickUpPetMessageEvent, new PickUpPetEvent());
            this._incomingPackets.Add(ClientPacketHeader.PlacePetMessageEvent, new PlacePetEvent());
            this._incomingPackets.Add(ClientPacketHeader.RideHorseMessageEvent, new RideHorseEvent());
            this._incomingPackets.Add(ClientPacketHeader.ApplyHorseEffectMessageEvent, new ApplyHorseEffectEvent());
            this._incomingPackets.Add(ClientPacketHeader.RemoveSaddleFromHorseMessageEvent, new RemoveSaddleFromHorseEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent, new ModifyWhoCanRideHorseEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, new GetPetTrainingPanelEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPetCommandsEvent, new GetPetCommandsEvent());
        }

        private void RegisterBots()
        {
            this._incomingPackets.Add(ClientPacketHeader.PlaceBotMessageEvent, new PlaceBotEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickUpBotMessageEvent, new PickUpBotEvent());
            this._incomingPackets.Add(ClientPacketHeader.OpenBotActionMessageEvent, new OpenBotActionEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveBotActionMessageEvent, new SaveBotActionEvent());
        }

        private void RegisterFurni()
        {
            //this._incomingPackets.Add(ClientPacketHeader.GetHCCatalogGiftsEvent, new GetHCCatalogGiftsEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateMagicTileMessageEvent, new UpdateMagicTileEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetYouTubeTelevisionMessageEvent, new GetYouTubeTelevisionEvent());
            this._incomingPackets.Add(ClientPacketHeader.ToggleYouTubeVideoMessageEvent, new ToggleYouTubeVideoEvent());
            this._incomingPackets.Add(ClientPacketHeader.YouTubeVideoInformationMessageEvent, new YouTubeVideoInformationEvent());
            this._incomingPackets.Add(ClientPacketHeader.YouTubeGetNextVideo, new YouTubeGetNextVideo());
            this._incomingPackets.Add(ClientPacketHeader.SaveWiredTriggerConfigMessageEvent, new SaveWiredConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, new SaveWiredConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, new SaveWiredConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.SaveBrandingItemMessageEvent, new SaveBrandingItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetTonerMessageEvent, new SetTonerEvent());
            this._incomingPackets.Add(ClientPacketHeader.DiceOffMessageEvent, new DiceOffEvent());
            this._incomingPackets.Add(ClientPacketHeader.ThrowDiceMessageEvent, new ThrowDiceEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetMannequinNameMessageEvent, new SetMannequinNameEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetMannequinFigureMessageEvent, new SetMannequinFigureEvent());
            this._incomingPackets.Add(ClientPacketHeader.CreditFurniRedeemMessageEvent, new CreditFurniRedeemEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetStickyNoteMessageEvent, new GetStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.AddStickyNoteMessageEvent, new AddStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.UpdateStickyNoteMessageEvent, new UpdateStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.DeleteStickyNoteMessageEvent, new DeleteStickyNoteEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMoodlightConfigMessageEvent, new GetMoodlightConfigEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoodlightUpdateMessageEvent, new MoodlightUpdateEvent());
            this._incomingPackets.Add(ClientPacketHeader.ToggleMoodlightMessageEvent, new ToggleMoodlightEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseOneWayGateMessageEvent, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseHabboWheelMessageEvent, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.OpenGiftMessageEvent, new OpenGiftEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGroupFurniSettingsMessageEvent, new GetGroupFurniSettingsEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseSellableClothingMessageEvent, new UseSellableClothingEvent());
            this._incomingPackets.Add(ClientPacketHeader.ConfirmLoveLockMessageEvent, new ConfirmLoveLockEvent());
            this._incomingPackets.Add(ClientPacketHeader.FootballGateComponent, new FootballGateComponent());

            // CRAFT
            this._incomingPackets.Add(ClientPacketHeader.CraftSecretMessageEvent, new CraftSecretEvent());
            this._incomingPackets.Add(ClientPacketHeader.ExecuteCraftingRecipeMessageEvent, new ExecuteCraftingRecipeEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCraftingItemMessageEvent, new GetCraftingItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetCraftingRecipesAvailableMessageEvent, new GetCraftingRecipesAvailableEvent());
            this._incomingPackets.Add(ClientPacketHeader.SetCraftingRecipeMessageEvent, new SetCraftingRecipeEvent());

            //RENTABLE
            this._incomingPackets.Add(ClientPacketHeader.BuyRentableSpaceEvent, new BuyRentableSpaceEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelRentableSpaceEvent, new CancelRentableSpaceEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetRentableSpaceMessageEvent, new GetRentableSpaceEvent());
        }

        private void RegisterRoomPolls()
        {
            this._incomingPackets.Add(ClientPacketHeader.PollStartMessageEvent, new PollStartEvent());
            //this._incomingPackets.Add(ClientPacketHeader.PollAnswerMessageEvent, new PollAnswerEvent());
            this._incomingPackets.Add(ClientPacketHeader.PollRejectMessageEvent, new PollRejectEvent());
        }

        private void FloorPlanEditor()
        {
            this._incomingPackets.Add(ClientPacketHeader.SaveFloorPlanModelMessageEvent, new SaveFloorPlanModelEvent());
            this._incomingPackets.Add(ClientPacketHeader.InitializeFloorPlanSessionMessageEvent, new InitializeFloorPlanSessionEvent());
            this._incomingPackets.Add(ClientPacketHeader.FloorPlanEditorRoomPropertiesMessageEvent, new FloorPlanEditorRoomPropertiesEvent());
        }

        private void RegisterModeration()
        {
            this._incomingPackets.Add(ClientPacketHeader.OpenHelpToolMessageEvent, new OpenHelpToolEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, new GetModeratorRoomInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, new GetModeratorUserInfoEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorUserRoomVisitsMessageEvent, new GetModeratorUserRoomVisitsEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerateRoomMessageEvent, new ModerateRoomEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModeratorActionMessageEvent, new ModeratorActionEvent());
            this._incomingPackets.Add(ClientPacketHeader.SubmitNewTicketMessageEvent, new SubmitNewTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorRoomChatlogMessageEvent, new GetModeratorRoomChatlogEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorUserChatlogMessageEvent, new GetModeratorUserChatlogEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetModeratorTicketChatlogsMessageEvent, new GetModeratorTicketChatlogsEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickTicketMessageEvent, new PickTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.ReleaseTicketMessageEvent, new ReleaseTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.CloseTicketMesageEvent, new CloseTicketEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationMuteMessageEvent, new ModerationMuteEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationKickMessageEvent, new ModerationKickEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationBanMessageEvent, new ModerationBanEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationMsgMessageEvent, new ModerationMsgEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationCautionMessageEvent, new ModerationCautionEvent());
            this._incomingPackets.Add(ClientPacketHeader.ModerationTradeLockMessageEvent, new ModerationTradeLockEvent());
        }

        public void RegisterGameCenter()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetGameListingMessageEvent, new GetGameListingEvent());
            this._incomingPackets.Add(ClientPacketHeader.InitializeGameCenterMessageEvent, new InitializeGameCenterEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetPlayableGamesMessageEvent, new GetPlayableGamesEvent());
            this._incomingPackets.Add(ClientPacketHeader.JoinPlayerQueueMessageEvent, new JoinPlayerQueueEvent());
            this._incomingPackets.Add(ClientPacketHeader.Game2GetWeeklyLeaderboardMessageEvent, new Game2GetWeeklyLeaderboardEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetGameCenterLeaderboardsEvent, new GetGameCenterLeaderboardsEvent());
            this._incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent, new UnknownGameCenterEvent());
            this._incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent2, new UnknownGameCenterEvent2());
            this._incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent3, new UnknownGameCenterEvent3());
            this._incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent4, new UnknownGameCenterEvent4());
            this._incomingPackets.Add(ClientPacketHeader.UnknownGameCenterEvent5, new UnknownGameCenterEvent5());
            this._incomingPackets.Add(ClientPacketHeader.GetWeeklyLeaderBoardEvent, new GetWeeklyLeaderBoardEvent());
        }
        

        
    }
}