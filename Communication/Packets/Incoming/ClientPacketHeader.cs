namespace Moon.Communication.Packets.Incoming
{
    public static class ClientPacketHeader
    {

        // PRODUCTION-201609061203-93549713
        public const int MakeOfferMessageEvent = 3464;
        public const int GetRoomBannedUsersMessageEvent = 2038;
        public const int GetPetInventoryMessageEvent = 1277;
        public const int DropHandItemMessageEvent = 2840;
        public const int ReleaseTicketMessageEvent = 3514;
        public const int GetModeratorRoomInfoMessageEvent = 1468;
        public const int KickUserMessageEvent = 329;
        public const int SaveWiredEffectConfigMessageEvent = 1674;
        public const int RespectPetMessageEvent = 512;
        public const int GenerateSecretKeyMessageEvent = 1838;
        public const int GetModeratorTicketChatlogsMessageEvent = 2325;
        public const int PingMessageEvent = 1181;
        public const int GetAchievementsMessageEvent = 1810;
        public const int SaveWiredTriggerConfigMessageEvent = 1772;
        public const int AcceptGroupMembershipMessageEvent = 1446;
        public const int GetGroupFurniSettingsMessageEvent = 2508;
        public const int TakeAdminRightsMessageEvent = 3171;
        public const int RemoveAllRightsMessageEvent = 2852;
        public const int UpdateThreadMessageEvent = 3228;
        public const int TourRequestEvent = 2528;
        public const int ManageGroupMessageEvent = 3526;
        public const int ModifyRoomFilterListMessageEvent = 2082;
        public const int SSOTicketMessageEvent = 2060;
        public const int AvatarEffectActivatedMessageEvent = 670;
        public const int JoinGroupMessageEvent = 1077;
        public const int DeclineGroupMembershipMessageEvent = 1157;
        public const int UniqueIDMessageEvent = 3229;
        public const int RemoveMyRightsMessageEvent = 3388;
        public const int ApplyHorseEffectMessageEvent = 3553;
        public const int GetPetInformationMessageEvent = 3413;
        public const int GetGameListingMessageEvent = 19;
        public const int GiveHandItemMessageEvent = 367;
        public const int GetHabboGroupBadgesMessageEvent = 3844;
        public const int UpdateFigureDataMessageEvent = 2113;
        public const int TradingRemoveItemMessageEvent = 2390;
        public const int RemoveGroupMemberMessageEvent = 2249;
        public const int EventLogMessageEvent = -1;
        public const int RefreshCampaignMessageEvent = 2647;
        public const int GetPromotableRoomsMessageEvent = 1921;
        public const int StaffPickRoomEvent = 235;
        public const int UseOneWayGateMessageEvent = 2018;
        public const int AddStickyNoteMessageEvent = 3024;
        public const int GetSelectedBadgesMessageEvent = 2821;
        public const int ModerationTradeLockMessageEvent = 1104;
        public const int UpdateStickyNoteMessageEvent = 2535;
        public const int GuideSessionOnDutyUpdateMessageEvent = 3480;
        public const int GuideSessionIsTypingMessageEvent = 1545;
        public const int CloseTicketMesageEvent = 1113;
        public const int RequestBuddyMessageEvent = 3524;
        public const int GetOwnOffersMessageEvent = 3457;
        public const int FloorPlanEditorRoomPropertiesMessageEvent = 1700;
        public const int GetFurnitureAliasesMessageEvent = 1627;
        public const int GetRoomSettingsMessageEvent = 3742;
        public const int RequestFurniInventoryMessageEvent = 3434;
        public const int DeleteGroupPostMessageEvent = 2474;
        public const int ModerationKickMessageEvent = 1288;
        public const int OpenFlatConnectionMessageEvent = 3077;
        public const int DanceMessageEvent = 3390;
        public const int RemoveBuddyMessageEvent = 2056;
        public const int LatencyTestMessageEvent = 397;
        public const int InfoRetrieveMessageEvent = 333;
        public const int YouTubeGetNextVideo = 3495;
        public const int SetObjectDataMessageEvent = 696;
        public const int MessengerInitMessageEvent = 3111;
        public const int GuideSessionCreateMessageEvent = 2460;
        public const int PickUpBotMessageEvent = 2397;
        public const int ActionMessageEvent = 2019;
        public const int LookToMessageEvent = 1166;
        public const int ToggleMoodlightMessageEvent = 2682;
        public const int FollowFriendMessageEvent = 1557;
        public const int PickUpPetMessageEvent = 511;
        public const int GetSellablePetBreedsMessageEvent = 3983;
        public const int IgnoreUserMessageEvent = 190;
        public const int DeleteRoomMessageEvent = 1347;
        public const int StartQuestMessageEvent = 3706;
        public const int GetBuddyRequestsMessageEvent = 1718;
        public const int SaveRoomThumbnailEvent = 927;
        public const int GetGiftWrappingConfigurationMessageEvent = 2068;
        public const int TradingAcceptMessageEvent = 1120;
        public const int UpdateGroupIdentityMessageEvent = 1263;
        public const int GetHabboClubCenterInfoMessageEvent = 1433;
        public const int RideHorseMessageEvent = 1752;
        public const int ApplySignMessageEvent = 3115;
        public const int FindRandomFriendingRoomMessageEvent = 2366;
        public const int GetModeratorUserChatlogMessageEvent = 2555;
        public const int TradingOfferItemMessageEvent = 1050;
        public const int AmbassadorWarningMessageEvent = 489;
        public const int GetWardrobeMessageEvent = 2250;
        public const int MuteUserMessageEvent = 1691;
        public const int UpdateForumSettingsMessageEvent = 282;
        public const int GuideSessionInviteRequesterMessageEvent = 2270;
        public const int ApplyDecorationMessageEvent = 3090;
        public const int GetBotInventoryMessageEvent = 301;
        public const int UseHabboWheelMessageEvent = 3662;
        public const int EditRoomPromotionMessageEvent = 1374;
        public const int GetCurrentQuestMessageEvent = 1906;
        public const int GetModeratorUserInfoMessageEvent = 3178;
        public const int PlaceBotMessageEvent = 2484;
        public const int GetCatalogPageMessageEvent = 2861;
        public const int GetThreadsListDataMessageEvent = 1281;
        public const int ShoutMessageEvent = 2884;
        public const int GetTalentTrackMessageEvent = 3913;
        public const int DiceOffMessageEvent = 1952;
        public const int SetUserFocusPreferenceEvent = 3250;
        public const int TradingModifyMessageEvent = 1331;
        public const int LetUserInMessageEvent = 436;
        public const int SetActivatedBadgesMessageEvent = 3257;
        public const int UpdateGroupSettingsMessageEvent = 545;
        public const int ApproveNameMessageEvent = 1019;
        public const int CancelOfferMessageEvent = 300;
        public const int GetBadgeEditorPartsMessageEvent = 3639;
        public const int TradingCancelMessageEvent = 1007;
        public const int DeleteGroupMessageEvent = 994;
        public const int DeleteStickyNoteMessageEvent = 2626;
        public const int TradingCancelConfirmMessageEvent = 522;
        public const int GetGroupInfoMessageEvent = 3820;
        public const int GetStickyNoteMessageEvent = 806;
        public const int DeclineBuddyMessageEvent = 3604;
        public const int OpenGiftMessageEvent = 1002;
        public const int GiveRoomScoreMessageEvent = 1816;
        public const int SetGroupFavouriteMessageEvent = 3378;
        public const int SetMannequinNameMessageEvent = 3788;
        public const int CallForHelpMessageEvent = 759;
        public const int RoomDimmerSavePresetMessageEvent = 3167;
        public const int UpdateGroupBadgeMessageEvent = 3645;
        public const int PickTicketMessageEvent = 1953;
        public const int SetTonerMessageEvent = 960;
        public const int RespectUserMessageEvent = 3814;
        public const int PostQuizAnswersMessageEvent = 3563;
        public const int YouTubeVideoInformationMessageEvent = 3379;
        public const int DeleteGroupThreadMessageEvent = 2646;
        public const int DeleteFavouriteRoomMessageEvent = 2148;
        public const int InitializeGameCenterMessageEvent = 3657;
        public const int CreditFurniRedeemMessageEvent = 3320;
        public const int ModerationMsgMessageEvent = 1016;
        public const int ToggleYouTubeVideoMessageEvent = 3617;
        public const int UpdateNavigatorSettingsMessageEvent = 1480;
        public const int UseSellableClothingMessageEvent = 430;
        public const int HabboCameraEvent = 3221;
        public const int BuyOfferMessageEvent = 3564;
        public const int PerformanceLogMessageEvent = 3119;
        public const int ToggleMuteToolMessageEvent = 1748;
        public const int InitTradeMessageEvent = 3951;
        public const int ChatMessageEvent = 2901;
        public const int SaveRoomSettingsMessageEvent = 72;
        public const int GetMarketplaceOffersMessageEvent = 3834;
        public const int PurchaseFromCatalogAsGiftMessageEvent = 2312;
        public const int CheckGnomeNameMessageEvent = 2988;
        public const int GetGroupCreationWindowMessageEvent = 3421;
        public const int SubmitBullyReportMessageEvent = 1657;
        public const int GiveAdminRightsMessageEvent = 195;
        public const int GetGroupMembersMessageEvent = 1846;
        public const int ModerateRoomMessageEvent = 1301;
        public const int GetForumStatsMessageEvent = 602;
        public const int GetPromoArticlesMessageEvent = 2319;
        public const int CallForHelpFromForumThreadMessageEvent = 1008;
        public const int SitMessageEvent = 2114;
        public const int SetSoundSettingsMessageEvent = 943;
        public const int ModerationCautionMessageEvent = 2958;
        public const int InitializeFloorPlanSessionMessageEvent = 1971;
        public const int ModeratorActionMessageEvent = 2905;
        public const int PostGroupContentMessageEvent = 2005;
        public const int GetModeratorRoomChatlogMessageEvent = 2407;
        public const int SetFriendBarStateMessageEvent = 1574;
        public const int GetUserFlatCatsMessageEvent = 3082;
        public const int SendBullyReportMessageEvent = 3060;
        public const int RemoveRightsMessageEvent = 2971;
        public const int ModerationBanMessageEvent = 3800;
        public const int GetUserTagsMessageEvent = 3693;
        public const int CanCreateRoomMessageEvent = 462;
        public const int UseWallItemMessageEvent = 2415;
        public const int OpenCalendarBoxMessageEvent = 1954;
        public const int PlaceObjectMessageEvent = 1736;
        public const int OpenBotActionMessageEvent = 3269;
        public const int GetMarketplaceConfigurationMessageEvent = 86;
        public const int GetCreditsInfoMessageEvent = 1796;
        public const int OnBullyClickMessageEvent = 215;
        public const int GetEventCategoriesMessageEvent = 809;
        public const int GetRoomEntryDataMessageEvent = 2480;
        public const int MoveWallItemMessageEvent = 3191;
        public const int ModerationDefaultSanctionMessageEvent = 2673;
        public const int CallForHelpFromIMMessageEvent = 2625;
        public const int UpdateGroupColoursMessageEvent = 705;
        public const int HabboSearchMessageEvent = 2351;
        public const int JoinPlayerQueueMessageEvent = 2461;
        public const int CommandBotMessageEvent = 2787;
        public const int ForceOpenCalendarBoxMessageEvent = 790;
        public const int GetCameraPriceEvent = 2850;
        public const int SetCustomStackingHeightMessageEvent = 1237;
        public const int UnIgnoreUserMessageEvent = 534;
        public const int GetGuestRoomMessageEvent = 1334;
        public const int SetMannequinFigureMessageEvent = 1250;
        public const int DisconnectionMessageEvent = 29;
        public const int GetIgnoredUsersMessageEvent = 3010;
        public const int AssignRightsMessageEvent = 128;
        public const int GetClubOffersMessageEvent = 2454;
        public const int GetYouTubeTelevisionMessageEvent = 1894;
        public const int SetMessengerInviteStatusMessageEvent = 2805;
        public const int UpdateFloorPropertiesMessageEvent = 3824;
        public const int GetMoodlightConfigMessageEvent = 2037;
        public const int PurchaseRoomPromotionMessageEvent = 1939;
        public const int AddFavouriteRoomMessageEvent = 3694;
        public const int SendRoomInviteMessageEvent = 1770;
        public const int SaveEnforcedCategorySettingsMessageEvent = 3145;
        public const int ModerationMuteMessageEvent = 405;
        public const int SetRelationshipMessageEvent = 3323;
        public const int ChangeMottoMessageEvent = 1442;
        public const int UnbanUserFromRoomMessageEvent = 3915;
        public const int GetRoomRightsMessageEvent = 3036;
        public const int PurchaseGroupMessageEvent = 457;
        public const int CreateFlatMessageEvent = 1655;
        public const int OpenHelpToolMessageEvent = 3864;
        public const int FriendListUpdateMessageEvent = 1724;
        public const int GetMarketplaceCanMakeOfferMessageEvent = 2556;
        public const int ThrowDiceMessageEvent = 550;
        public const int SaveWiredConditionConfigMessageEvent = 2417;
        public const int GetCameraRequestEvent = 1405;
        public const int GetCatalogOfferMessageEvent = 2736;
        public const int GetHCCatalogGiftsEvent = 245;
        public const int PurchaseFromCatalogMessageEvent = 2316;
        public const int GetCatalogModeMessageEvent = 1901;
        public const int PickupObjectMessageEvent = 1560;
        public const int GetMarketplaceItemStatsMessageEvent = 2391;
        public const int GetRecipeConfigMessageEvent = 1245;
        public const int FindNewFriendsMessageEvent = 2622;
        public const int CancelQuestMessageEvent = 411;
        public const int RedeemOfferCreditsMessageEvent = 3426;
        public const int RedeemVoucherMessageEvent = 2453;
        public const int NavigatorSearchMessageEvent = 460;
        public const int MoveAvatarMessageEvent = 3349;
        public const int GetSoundSettingsMessageEvent = 3640;
        public const int GetClientVersionMessageEvent = 4000;
        public const int GuideSessionResolvedMessageEvent = 463;
        public const int InitializeNavigatorMessageEvent = 2083;
        public const int TradingOfferItemsMessageEvent = 1749;
        public const int GetRoomFilterListMessageEvent = 1027;
        public const int WhisperMessageEvent = 804;
        public const int InitCryptoMessageEvent = 1961;
        public const int GetPetTrainingPanelMessageEvent = 2579;
        public const int MoveObjectMessageEvent = 2374;
        public const int GetPlayableGamesMessageEvent = 3889;
        public const int StartTypingMessageEvent = 637;
        public const int GetSongInfoMessageEvent = -1;
        public const int GoToHotelViewMessageEvent = 878;
        public const int GetExtendedProfileMessageEvent = 2666;
        public const int SendMsgMessageEvent = 2193;
        public const int CancelTypingMessageEvent = 957;
        public const int GuideSessionMsgMessageEvent = 3000;
        public const int GetGroupFurniConfigMessageEvent = 3330;
        public const int TradingConfirmMessageEvent = 312;
        public const int RemoveGroupFavouriteMessageEvent = 3696;
        public const int VersionCheckMessageEvent = 3270;
        public const int PlacePetMessageEvent = 3477;
        public const int CheckValidNameMessageEvent = 1831;
        public const int ModifyWhoCanRideHorseMessageEvent = 1675;
        public const int GetRelationshipsMessageEvent = 3731;
        public const int GetCatalogIndexMessageEvent = 3714;
        public const int ScrGetUserInfoMessageEvent = 1086;
        public const int AvatarEffectSelectedMessageEvent = 3562;
        public const int ConfirmLoveLockMessageEvent = 2758;
        public const int RemoveSaddleFromHorseMessageEvent = 218;
        public const int GuideSessionGetRequesterRoomMessageEvent = 2446;
        public const int SaveNavigatorPositionEvent = 3165;
        public const int AcceptBuddyMessageEvent = 2206;
        public const int GetQuestListMessageEvent = 344;
        public const int SaveWardrobeOutfitMessageEvent = 1029;
        public const int BanUserMessageEvent = 3240;
        public const int GetThreadDataMessageEvent = 674;
        public const int ChangeNameMessageEvent = 459;
        public const int MySanctionStatusMessageEvent = 2660;
        public const int GetBadgesMessageEvent = 3155;
        public const int UseFurnitureMessageEvent = 819;
        public const int GoToFlatMessageEvent = 289;
        public const int GetSanctionStatusMessageEvent = 2660;
        public const int GuideHelpMessageEvent = 2528;
        public const int SubmitNewTicketMessageEvent = 759;
        public const int NewNavigatorSearchMessageEvent = 460;
        public const int GetModeratorUserRoomVisitsMessageEvent = 923;
        public const int SetChatPreferenceMessageEvent = 1254;
        public const int InitializeNewNavigatorMessageEvent = 2083;
        public const int OpenPlayerProfileMessageEvent = 2666;
        public const int UpdateMagicTileMessageEvent = 1237;
        public const int EventTrackerMessageEvent = 3466;
        public const int SaveBotActionMessageEvent = 2787;
        public const int SetUsernameMessageEvent = 3010;
        public const int SaveBrandingItemMessageEvent = 696;
        public const int CheckQuizTypeEvent = 2358;
        public const int MemoryPerformanceMessageEvent = 3119;
        public const int CheckPetNameMessageEvent = 1019;
        public const int SaveFloorPlanModelMessageEvent = 3824;
        public const int GetOffersMessageEvent = 3834;
        public const int ClientVariablesMessageEvent = 3270;
        public const int MoodlightUpdateMessageEvent = 3167;
        public const int UnknownQuestMessageEvent = 982;

        // Habbo Club
        public const int GetHabboClubWindowMessageEvent = 2454;

        // Camera
        public const int RenderRoomEvent = -1;
        public const int PurchasePhotoEvent = -1;
        public const int PublishPhotoEvent = -1;
        public const int ReportCameraPhotoEvent = -1;

        public const int RenderRoomMessageComposer = 927;
        public const int RenderRoomMessageComposerBigPhoto = 1405;
        public const int BuyServerCameraPhoto = 3221;

        // Nux Systeem
        public const int RoomNucsAlerta = 3749;
        public const int LTDCountdownEvent = 3123;
        public const int ConcurrentUsersCompetitionEvent = 3969;
        public const int GiveConcurrentUsersRewardEvent = 901;
        public const int VoteCommunityGoalVS = 131;
        public const int CommunityGoalEvent = 2342;
        public const int GetNuxPresentEvent = 1761;

        // Recycler
        public const int FurniMaticPageEvent = 3848;
        public const int FurniMaticRecycleEvent = 3098;
        public const int FurniMaticRewardsEvent = 425;

        // Forums
        public const int GetGroupForumsMessageEvent = 375;
        public const int PublishForumThreadMessageEvent = 2005;
        public const int AlterForumThreadStateMessageEvent = 2646;
        public const int ReadForumThreadMessageEvent = 674;
        public const int CallForHelpFromForumMessageMessageEvent = 1449;
        public const int GetGroupForumDataMessageEvent = 602;
        public const int GetForumUserProfileMessageEvent = 3312;
        public const int GetGroupForumThreadRootMessageEvent = 1281;
        public const int GetForumsListDataMessageEvent = 1004;

        // Quick Polls
        public const int SubmitPollAnswerMessageEvent = 1774;


        // Helper Tool
        public const int HandleHelperToolMessageEvent = 3480;
        public const int AcceptJoinJudgeChatMessageEvent = 2229;
        public const int CancelCallForHelperMessageEvent = 1871;
        public const int CallForHelperMessageEvent = 2528;
        public const int AcceptHelperSessionMessageEvent = 2460;
        public const int HelperSessioChatTypingMessageEvent = 1545;
        public const int HelperSessioChatSendMessageMessageEvent = 3000;
        public const int CloseHelperChatSessionMessageEvent = 463;
        public const int VisitHelperUserSessionMessageEvent = 2446;
        public const int InvinteHelperUserSessionMessageEvent = 2270;
        public const int FinishHelperSessionMessageEvent = 2547;
        public const int ReportHelperSessionMessageEvent = 2625;
        public const int ReportBullyUserMessageEvent = 3520;

        // Jukebox
        public const int GetJukeboxDisksMessageEvent = 800;
        public const int GetJukeboxPlaylistsMessageEvent = 2231;
        public const int GetMusicDataMessageEvent = 283;
        public const int SyncMusicMessageEvent = 594;
        public const int AddPlaylistItemMessageEvent = 3789;
        public const int RemovePlaylistItemMessageEvent = 2939;


        // Targetted Offers
        public const int BuyTargettedOfferMessageEvent = 3539;

        // Football Gate
        public const int FootballGateComponent = 3483;

        // Bonus Rare
        public const int RequestBonusRareEvent = 2894;

        // Crafting de Devbest y Ragesone jaja saludos
        public const int GetCraftingItemMessageEvent = 1407; // UP
        public const int SetCraftingRecipeMessageEvent = 1245; // UP
        public const int ExecuteCraftingRecipeMessageEvent = 3448; // UP
        public const int CraftSecretMessageEvent = 1666; // UP
        public const int GetCraftingRecipesAvailableMessageEvent = 2959; // UP

        // Poll
        public const int PollRejectMessageEvent = 369;
        public const int PollStartMessageEvent = 765;
        //public const int PollAnswerMessageEvent = 1774;

        //RentableSpace
        public const int GetRentableSpaceMessageEvent = 219;
        public const int BuyRentableSpaceEvent = 1241;
        public const int CancelRentableSpaceEvent = 1311;

        //Builder Club
        public const int PlaceBuilderItemMessageEvent = 2204;

        //Staff Selection
        public const int ModifyNavigatorStaffRoomEvent = 235;

        // Spectating
        public const int GoToFlatAsSpectatorEvent = 2117;

        public const int RedeemHCGiftEvent = 919;
        public const int GetAdsOfferEvent = 63;
        public const int GetGameCenterLeaderboardsEvent = 833;
        public const int UnknownGameCenterEvent = 922;
        public const int UnknownGameCenterEvent2 = 2245;
        public const int UnknownGameCenterEvent3 = 2183;
        public const int UnknownGameCenterEvent4 = 2088;

        public const int GetWeeklyLeaderBoardEvent = 954;
        public const int Game2GetWeeklyLeaderboardMessageEvent = 2379;

        public const int UnknownGameCenterEvent5 = 760;

        // Pet Related / Monsterplants?
        public const int GetPetCommandsEvent = 2410;

        // Ciudadanía Habbo
        public const int GetCitizenshipTypeEvent = 2041;
    }
}
