using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Moon
{
    class MoonStaticGameSettings
    {
        /// <summary>
        ///     The amount of credits a user will recieve every x minutes
        /// </summary>
        public const int UserCreditsUpdateAmount = 5000;
        public const int UserPixelsUpdateAmount = 1000;
        public const int UserDiamantesUpdateAmount = 1;
        public const int UserGOTWUpdateAmount = 0;

        public const int UserVipCreditsUpdateAmount = 10000;
        public const int UserVipPixelsUpdateAmount = 5000;
        public const int UserVipDiamantesUpdateAmount = 3;
        public const int UserVipGOTWUpdateAmount = 1;


        /// <summary>
        ///     The time a user will have to wait for Credits/Pixels update in minutes
        /// </summary>
        public const int halloffame = 15;

        /// <summary>
        ///     The time a user will have to wait for Credits/Pixels update in minutes
        /// </summary>
        public const int UserDiamantesUpdateTimer = 60;

        /// <summary>
        ///     The time a user will have to wait for Credits/Pixels update in minutes
        /// </summary>
        public const int UserCreditsUpdateTimer = 15;

        /// <summary>
        /// 
        /// </summary>
        public const int BonusRareUpdateTimer = 120;

        /// <summary>
        ///     The maximum amount of furniture that can be placed in a room.
        /// </summary>
        public const int RoomFurnitureLimit = 1000;

        /// <summary>
        ///     The maximum amount of pet instances that can be placed into a room.
        /// </summary>
        public const int RoomPetPlacementLimit = 25;

        /// <summary>
        ///     The amount of pets a user can place in a room that isn't owned by them.
        /// </summary>
        public const int UserPetPlacementRoomLimit = 5;

        /// <summary>
        ///     The maximum life time of a room promotion.
        /// </summary>
        public const int RoomPromotionLifeTime = 120;

        /// <summary>
        ///     The maximum amount of friends a user can have.
        /// </summary>
        public const int MessengerFriendLimit = 1000;

        /// <summary>
        ///     The amount of credits a group costs.
        /// </summary>
        public const int GroupPurchaseAmount = 5000;

        /// <summary>
        ///     The maximum amount of banned words that a client can send per session.
        /// </summary>
        public const int BannedPhrasesAmount = 4;

        /// <summary>
        /// The maximum amount of members a group can exceed before being eligible for deletion.
        /// </summary>
        public const int GroupMemberDeletionLimit = 500;

        /// <summary>
        /// The hotel status
        /// </summary>
        public static bool IsGoingToBeClose = false;

        /// <summary>
        /// Hotel is open for users
        /// </summary>
        public static bool HotelOpenForUsers = true;
    }
}
