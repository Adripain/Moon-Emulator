using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.HabboHotel.Groups.Forums
{
    public class GroupForumManager
    {
        List<GroupForum> Forums;

        public GroupForumManager()
        {
            Forums = new List<GroupForum>();

        }

        public GroupForum GetForum(int GroupId)
        {
            GroupForum f = null;
            return TryGetForum(GroupId, out f) ? f : null;
        }

        public GroupForum CreateGroupForum(Group Gp)
        {
            GroupForum GF;
            if (TryGetForum(Gp.Id, out GF))
                return GF;

            using (var adap = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("REPLACE INTO group_forums_settings (group_id) VALUES (@gp)");
                adap.AddParameter("gp", Gp.Id);
                adap.RunQuery();

                adap.SetQuery("UPDATE groups SET has_forum = '1' WHERE id = @id");
                adap.AddParameter("id", Gp.Id);
                adap.RunQuery();
            }

            GF = new GroupForum(Gp);
            Gp.HasForum = true;
            Forums.Add(GF);
            return GF;
        }

        public bool TryGetForum(int Id, out GroupForum Forum)
        {
            if ((Forum = Forums.FirstOrDefault(c => c.Id == Id)) != null)
                return true;

            Group Gp;
            if (!MoonEnvironment.GetGame().GetGroupManager().TryGetGroup(Id, out Gp))
                return false;

            if (!Gp.HasForum)
                return false;

            Forum = new GroupForum(Gp);
            Forums.Add(Forum);
            return true;
        }

        public List<GroupForum> GetForumsByUserId(int Userid)
        {
            GroupForum F;
            return MoonEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Userid).Where(c => TryGetForum(c.Id, out F)).Select(c => GetForum(c.Id)).ToList();
        }

        public void RemoveGroup(Group Group)
        {
            using (var dbClient = MoonEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `group_forums_settings` WHERE `group_id` = '" + Group.Id + "'");

                dbClient.RunQuery("DELETE post FROM group_forums_thread_posts post INNER JOIN group_forums_threads threads ON threads.forum_id = '" + Group.Id + "' WHERE threads.id = post.thread_id");

                dbClient.RunQuery("DELETE v FROM group_forums_thread_views v INNER JOIN group_forums_threads threads ON threads.forum_id = '" + Group.Id + "' WHERE v.thread_id = threads.id");

                dbClient.RunQuery("DELETE t FROM group_forums_threads t WHERE t.forum_id = '" + Group.Id + "'");
            }
        }

        public int GetUnreadThreadForumsByUserId(int Id)
        {
            return this.GetForumsByUserId(Id).Where(c => c.UnreadMessages(Id) > 0).Count();
        }
    }
}
