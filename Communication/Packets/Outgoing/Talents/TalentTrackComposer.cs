using System;
using System.Collections.Generic;
using Moon.HabboHotel.Achievements;
using Moon.HabboHotel.GameClients;

namespace Moon.Communication.Packets.Outgoing.Talents
{
    class TalentTrackComposer : ServerPacket
    {
        public TalentTrackComposer(GameClient session, string trackType, List<Talent> talents)
            : base(ServerPacketHeader.TalentTrackMessageComposer)
        {
            base.WriteString(trackType);
            base.WriteInteger(talents.Count);

            int failLevel = -1;

            foreach (Talent current in talents)
            {
                base.WriteInteger(current.Level);
                int nm = failLevel == -1 ? 1 : 0; // TODO What does this mean?
                base.WriteInteger(nm);

                List<Talent> children = MoonEnvironment.GetGame().GetTalentManager().GetTalents(trackType, current.Id);

                base.WriteInteger(children.Count);

                foreach (Talent child in children)
                {
                    UserAchievement achievment = session.GetHabbo().GetAchievementData(child.AchievementGroup);
                    if (child.GetAchievement() == null)
                        throw new NullReferenceException(
                            string.Format("The following talent achievement can't be found: {0}",
                                child.AchievementGroup));

                    // TODO Refactor What does num mean?!
                    var num = (failLevel != -1 && failLevel < child.Level)
                       ? 0
                       : (session.GetHabbo().GetAchievementData(child.AchievementGroup) == null)
                           ? 1
                           : (session.GetHabbo().GetAchievementData(child.AchievementGroup).Level >=
                              child.AchievementLevel)
                               ? 2
                               : 1;

                    base.WriteInteger(child.GetAchievement().Id);
                    base.WriteInteger(0); // TODO Magic constant

                    base.WriteString(child.AchievementGroup + child.AchievementLevel);
                    base.WriteInteger(num);

                    base.WriteInteger(achievment != null ? achievment.Progress : 0);
                    base.WriteInteger(child.GetAchievement() == null ? 0
                        : child.GetAchievement().Levels[child.AchievementLevel].Requirement);

                    if (num != 2 && failLevel == -1)
                        failLevel = child.Level;
                }

                base.WriteInteger(0); // TODO Magic constant

                // TODO Type should be enum?
                if (current.Type == "citizenship" && current.Level == 4)
                {
                    base.WriteInteger(2);
                    base.WriteString("HABBO_CLUB_VIP_7_DAYS");
                    base.WriteInteger(7);
                    base.WriteString(current.Prize); // TODO Hardcoded stuff
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(1);
                    base.WriteString(current.Prize);
                    base.WriteInteger(0);
                }
            }
        }
    }
}
