using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon.HabboHotel.Items.Wired
{
    static class WiredBoxTypeUtility
    {
        public static WiredBoxType FromWiredId(int Id)
        {
            switch (Id)
            {
                default:
                    return WiredBoxType.None;
                case 1:
                    return WiredBoxType.TriggerUserSays;
                case 2:
                    return WiredBoxType.TriggerStateChanges;
                case 3:
                    return WiredBoxType.TriggerRepeat;
                case 4:
                    return WiredBoxType.TriggerRoomEnter;
                case 5:
                    return WiredBoxType.EffectShowMessage;
                case 6:
                    return WiredBoxType.EffectTeleportToFurni;
                case 7:
                    return WiredBoxType.EffectToggleFurniState;
                case 8:
                    return WiredBoxType.TriggerWalkOnFurni;
                case 9:
                    return WiredBoxType.TriggerWalkOffFurni;
                case 10:
                    return WiredBoxType.EffectKickUser;
                case 11:
                    return WiredBoxType.ConditionFurniHasUsers;
                case 12:
                    return WiredBoxType.ConditionFurniHasFurni;
                case 13:
                    return WiredBoxType.ConditionTriggererOnFurni;
                case 14:
                    return WiredBoxType.EffectMatchPosition;
                case 21:
                    return WiredBoxType.ConditionIsGroupMember;
                case 22:
                    return WiredBoxType.ConditionIsNotGroupMember;
                case 23:
                    return WiredBoxType.ConditionTriggererNotOnFurni;
                case 24:
                    return WiredBoxType.ConditionFurniHasNoUsers;
                case 25:
                    return WiredBoxType.ConditionIsWearingBadge;
                case 26:
                    return WiredBoxType.ConditionIsWearingFX;
                case 27:
                    return WiredBoxType.ConditionIsNotWearingBadge;
                case 28:
                    return WiredBoxType.ConditionIsNotWearingFX;
                case 29:
                    return WiredBoxType.ConditionMatchStateAndPosition;
                case 30:
                    return WiredBoxType.ConditionUserCountInRoom;
                case 31:
                    return WiredBoxType.ConditionUserCountDoesntInRoom;
                case 32:
                    return WiredBoxType.EffectMoveAndRotate;
                case 33:
                    return WiredBoxType.ConditionDontMatchStateAndPosition;
                case 34:
                    return WiredBoxType.ConditionFurniTypeMatches;
                case 35:
                    return WiredBoxType.ConditionFurniTypeDoesntMatch;
                case 36:
                    return WiredBoxType.ConditionFurniHasNoFurni;
                case 37:
                    return WiredBoxType.EffectMoveFurniToNearestUser;
                case 38:
                    return WiredBoxType.EffectMoveFurniFromNearestUser;
                case 39:
                    return WiredBoxType.EffectMuteTriggerer;
                case 40:
                    return WiredBoxType.EffectGiveReward;
                case 41:
                    return WiredBoxType.AddonRandomEffect;
                case 42:
                    return WiredBoxType.TriggerGameStarts;
                case 43:
                    return WiredBoxType.TriggerGameEnds;
                case 44:
                    return WiredBoxType.TriggerUserFurniCollision;
                case 45:
                    return WiredBoxType.EffectMoveFurniToNearestUser;
                case 46:
                    return WiredBoxType.EffectExecuteWiredStacks;
                case 47:
                    return WiredBoxType.EffectTeleportBotToFurniBox;
                case 48:
                    return WiredBoxType.EffectBotChangesClothesBox;
                case 49:
                    return WiredBoxType.EffectBotMovesToFurniBox;
                case 50:
                    return WiredBoxType.EffectBotCommunicatesToAllBox;
                case 51:
                    return WiredBoxType.EffectBotCommunicatesToUserBox;
                case 52:
                    return WiredBoxType.EffectBotFollowsUserBox;
                case 53:
                    return WiredBoxType.EffectBotGivesHanditemBox;
                case 54:
                    return WiredBoxType.ConditionActorHasHandItemBox;
                case 55:
                    return WiredBoxType.ConditionActorIsInTeamBox;
                case 56:
                    return WiredBoxType.EffectAddActorToTeam;
                case 57:
                    return WiredBoxType.EffectRemoveActorFromTeam;
                case 58:
                    return WiredBoxType.TriggerUserSaysCommand;
                case 59:
                    return WiredBoxType.EffectSetRollerSpeed;
                case 60:
                    return WiredBoxType.EffectRegenerateMaps;
                case 61:
                    return WiredBoxType.EffectGiveUserBadge;
                case 62:
                    return WiredBoxType.EffectAddScore;
                case 63:
                    return WiredBoxType.TriggerLongRepeat;
                case 64:
                    return WiredBoxType.EffectGiveUserHanditem;
                case 65:
                    return WiredBoxType.EffectGiveUserEnable;
                case 66:
                    return WiredBoxType.EffectGiveUserDance;
                case 67:
                    return WiredBoxType.EffectGiveUserFreeze;
                case 68:
                    return WiredBoxType.EffectGiveUserFastwalk;
                case 69:
                    return WiredBoxType.SendCustomMessageBox;
                case 70:
                    return WiredBoxType.ConditionActorIsNotInTeamBox;
                case 72:
                    return WiredBoxType.ConditionActorHasNotHandItemBox;
                case 73:
                    return WiredBoxType.EffectAddRewardPoints;
                case 74:
                    return WiredBoxType.ConditionDateRangeActive;
                case 75:
                    return WiredBoxType.EffectApplyClothes;
                case 76:
                    return WiredBoxType.ConditionWearingClothes;
                case 77:
                    return WiredBoxType.ConditionNotWearingClothes;
                case 78:
                    return WiredBoxType.TriggerAtGivenTime;
                case 79:
                    return WiredBoxType.EffectMoveUser;
                case 80:
                    return WiredBoxType.EffectTimerReset;
                case 81:
                    return WiredBoxType.EffectMoveToDir;
                case 82:
                    return WiredBoxType.EffectToggleNegativeFurniState;
                case 83:
                    return WiredBoxType.EffectProgressUserAchievement;
                case 84:
                    return WiredBoxType.TotalUsersCoincidence;
                case 85:
                    return WiredBoxType.TriggerBotReachedAvatar;
                case 86:
                    return WiredBoxType.EffectRaiseFurni;
                case 87:
                    return WiredBoxType.EffectLowerFurni;
                case 88:
                    return WiredBoxType.EffectRoomForward;
                case 89:
                    return WiredBoxType.ConditionActorHasDiamonds;
                case 90:
                    return WiredBoxType.ConditionActorHasNotDiamonds;
                case 91:
                    return WiredBoxType.ConditionActorHasDuckets;
                case 92:
                    return WiredBoxType.ConditionActorHasNotDuckets;
                case 93:
                    return WiredBoxType.ConditionActorHasRank;
                case 94:
                    return WiredBoxType.ConditionActorHasNotRank;
                case 95:
                    return WiredBoxType.ConditionActorHasNotCredits;
                case 96:
                    return WiredBoxType.EffectGiveUserDiamonds;
                case 97:
                    return WiredBoxType.EffectGiveUserDuckets;
                case 98:
                    return WiredBoxType.EffectGiveUserCredits;
                case 99:
                    return WiredBoxType.EffectSendYouTubeVideo;
                case 100:
                    return WiredBoxType.EffectAddScore2;
                case 101:
                    return WiredBoxType.EffectShowMessageNux;
                case 102:
                    return WiredBoxType.EffectShowMessageCustom;
            }
        }

        public static int GetWiredId(WiredBoxType Type)
        {
            switch (Type)
            {
                case WiredBoxType.TriggerUserSays:
                case WiredBoxType.TriggerUserSaysCommand:
                case WiredBoxType.ConditionMatchStateAndPosition:
                    return 0;
                case WiredBoxType.TriggerWalkOnFurni:
                case WiredBoxType.TriggerWalkOffFurni:
                case WiredBoxType.ConditionFurniHasUsers:
                case WiredBoxType.TotalUsersCoincidence:
                case WiredBoxType.ConditionTriggererOnFurni:
                case WiredBoxType.EffectTimerReset:
                    return 1;
                case WiredBoxType.EffectMatchPosition:
                case WiredBoxType.TriggerAtGivenTime:
                    return 3;
                case WiredBoxType.EffectMoveAndRotate:
                case WiredBoxType.TriggerStateChanges:
                case WiredBoxType.EffectMoveUser:
                    return 4;
                case WiredBoxType.ConditionUserCountInRoom:
                    return 5;
                case WiredBoxType.ConditionActorIsInTeamBox:
                case WiredBoxType.ConditionActorIsNotInTeamBox:
                case WiredBoxType.TriggerRepeat:
                case WiredBoxType.TriggerLongRepeat:
                case WiredBoxType.EffectAddScore:
                case WiredBoxType.EffectAddScore2:
                case WiredBoxType.EffectAddRewardPoints:
                    return 6;
                case WiredBoxType.TriggerRoomEnter:
                case WiredBoxType.EffectShowMessage:
                case WiredBoxType.EffectShowMessageNux:
                case WiredBoxType.EffectShowMessageCustom:
                case WiredBoxType.SendCustomMessageBox:
                case WiredBoxType.EffectProgressUserAchievement:
                case WiredBoxType.ConditionFurniHasFurni:
                case WiredBoxType.EffectSendYouTubeVideo:
                    return 7;
                case WiredBoxType.TriggerGameStarts:
                case WiredBoxType.TriggerGameEnds:
                case WiredBoxType.EffectTeleportToFurni:
                case WiredBoxType.EffectToggleFurniState:
                case WiredBoxType.EffectRaiseFurni:
                case WiredBoxType.EffectLowerFurni:
                case WiredBoxType.ConditionFurniTypeMatches:
                    return 8;
                case WiredBoxType.EffectGiveUserBadge:
                case WiredBoxType.EffectGiveUserDiamonds:
                case WiredBoxType.EffectGiveUserDuckets:
                case WiredBoxType.EffectGiveUserCredits:
                case WiredBoxType.EffectRoomForward:
                case WiredBoxType.EffectGiveUserFreeze:
                case WiredBoxType.EffectGiveUserFastwalk:
                case WiredBoxType.EffectGiveUserEnable:
                case WiredBoxType.EffectGiveUserDance:
                case WiredBoxType.EffectGiveUserHanditem:
                case WiredBoxType.EffectRegenerateMaps:
                case WiredBoxType.EffectKickUser:
                case WiredBoxType.EffectSetRollerSpeed:
                    return 7;
                case WiredBoxType.EffectAddActorToTeam:
                    return 9;
                case WiredBoxType.EffectRemoveActorFromTeam:
                case WiredBoxType.ConditionIsGroupMember:
                    return 10;
                case WiredBoxType.TriggerUserFurniCollision:

                case WiredBoxType.ConditionIsWearingBadge:
                case WiredBoxType.EffectMoveFurniToNearestUser:
                    return 11;
                case WiredBoxType.ConditionIsWearingFX:
                case WiredBoxType.EffectMoveFurniFromNearestUser:
                    return 12;
                case WiredBoxType.EffectMoveToDir:
                case WiredBoxType.ConditionDontMatchStateAndPosition:
                    return 13;
                case WiredBoxType.ConditionFurniHasNoUsers:
                case WiredBoxType.TriggerBotReachedAvatar:
                    return 14;
                case WiredBoxType.ConditionTriggererNotOnFurni:
                    return 15;
                case WiredBoxType.ConditionUserCountDoesntInRoom:
                    return 16;
                case WiredBoxType.EffectGiveReward:
                    return 17;
                case WiredBoxType.EffectExecuteWiredStacks:
                case WiredBoxType.ConditionFurniHasNoFurni:
                    return 18;
                case WiredBoxType.ConditionFurniTypeDoesntMatch:
                    return 19;
                case WiredBoxType.EffectMuteTriggerer:
                    return 20;
                case WiredBoxType.ConditionIsNotGroupMember:
                case WiredBoxType.EffectTeleportBotToFurniBox:
                    return 21;
                case WiredBoxType.ConditionIsNotWearingBadge:
                case WiredBoxType.ConditionActorHasHandItemBox:
                case WiredBoxType.ConditionActorHasNotDiamonds:
                case WiredBoxType.ConditionActorHasDiamonds:
                case WiredBoxType.ConditionActorHasNotHandItemBox:
                case WiredBoxType.ConditionActorHasDuckets:
                case WiredBoxType.ConditionActorHasNotDuckets:
                case WiredBoxType.ConditionActorHasRank:
                case WiredBoxType.ConditionActorHasNotRank:
                case WiredBoxType.ConditionActorHasNotCredits:
                case WiredBoxType.ConditionWearingClothes:
                case WiredBoxType.ConditionNotWearingClothes:
                case WiredBoxType.EffectBotMovesToFurniBox:
                    return 22;
                case WiredBoxType.ConditionIsNotWearingFX:
                case WiredBoxType.EffectBotCommunicatesToAllBox:
                    return 23;
                case WiredBoxType.EffectBotGivesHanditemBox:
                case WiredBoxType.ConditionDateRangeActive:
                    return 24;
                case WiredBoxType.EffectBotFollowsUserBox:
                    return 25;
                case WiredBoxType.EffectBotChangesClothesBox:
                case WiredBoxType.EffectApplyClothes:
                    return 26;
                case WiredBoxType.EffectBotCommunicatesToUserBox:
                    return 27;
            }
            return 0;
        }

        public static List<int> ContainsBlockedTrigger(IWiredItem Box, ICollection<IWiredItem> Triggers)
        {
            List<int> BlockedItems = new List<int>();

            if (Box.Type != WiredBoxType.EffectShowMessage && Box.Type != WiredBoxType.EffectMuteTriggerer && Box.Type != WiredBoxType.EffectTeleportToFurni && Box.Type != WiredBoxType.EffectKickUser && Box.Type != WiredBoxType.ConditionTriggererOnFurni)
                return BlockedItems;

            foreach (IWiredItem Item in Triggers)
            {
                if (Item.Type == WiredBoxType.TriggerRepeat || Item.Type == WiredBoxType.TriggerLongRepeat)
                {
                    if (!BlockedItems.Contains(Item.Item.GetBaseItem().SpriteId))
                        BlockedItems.Add(Item.Item.GetBaseItem().SpriteId);
                    else continue;
                }
                else continue;
            }

            return BlockedItems;
        }

        public static List<int> ContainsBlockedEffect(IWiredItem Box, ICollection<IWiredItem> Effects)
        {
            List<int> BlockedItems = new List<int>();

            if (Box.Type != WiredBoxType.TriggerRepeat || Box.Type != WiredBoxType.TriggerLongRepeat)
                return BlockedItems;

            bool HasMoveRotate = Effects.Where(x => x.Type == WiredBoxType.EffectMoveAndRotate).ToList().Count > 0;
            bool HasMoveNear = Effects.Where(x => x.Type == WiredBoxType.EffectMoveFurniToNearestUser).ToList().Count > 0;
            bool HasMoveToDir = Effects.Where(x => x.Type == WiredBoxType.EffectMoveToDir).ToList().Count > 0;

            foreach (IWiredItem Item in Effects)
            {
                if (Item.Type == WiredBoxType.EffectKickUser || Item.Type == WiredBoxType.EffectMuteTriggerer || Item.Type == WiredBoxType.EffectShowMessage || Item.Type == WiredBoxType.SendCustomMessageBox || Item.Type == WiredBoxType.EffectProgressUserAchievement || Item.Type == WiredBoxType.EffectTeleportToFurni || Item.Type == WiredBoxType.EffectBotFollowsUserBox)
                {
                    if (!BlockedItems.Contains(Item.Item.GetBaseItem().SpriteId))
                        BlockedItems.Add(Item.Item.GetBaseItem().SpriteId);
                    else continue;
                }
                else if ((Item.Type == WiredBoxType.EffectMoveFurniToNearestUser && HasMoveRotate) || (Item.Type == WiredBoxType.EffectMoveAndRotate && HasMoveNear) || (Item.Type == WiredBoxType.EffectMoveToDir && (HasMoveNear || HasMoveRotate)))
                {
                    if (!BlockedItems.Contains(Item.Item.GetBaseItem().SpriteId))
                        BlockedItems.Add(Item.Item.GetBaseItem().SpriteId);
                    else continue;
                }
            }

            return BlockedItems;
        }
    }
}