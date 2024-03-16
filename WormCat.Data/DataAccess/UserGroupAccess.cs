using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class UserGroupAccess : IUserGroupAccess
    {
        private readonly ILogger<UserGroupAccess> logger;
        private readonly WormCatRazorContext context;
        private readonly IUserAccess userAccess;

        public UserGroupAccess(ILogger<UserGroupAccess> logger, WormCatRazorContext context, IUserAccess userAccess)
        {
            this.logger = logger;
            this.context = context;
            this.userAccess = userAccess;
        }

        public async Task<UserGroup?> GetGroupByUserId(string? userId)
        {
            return await context.UserGroups.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<List<(int groupId, string rootUserId)>> GetGroupsTargetUserCanAccess(string? userId)
        {
            try
            {
                if (userId == null)
                    return new List<(int groupId, string userId)>();

                UserGroup? userGroup = await GetGroupByUserId(userId);

                if (userGroup == null)
                    return new List<(int groupId, string userId)>();

                List<(int groupId, string userId)> response = new List<(int groupId, string userId)>();

                foreach (var otherUserId in userGroup.OtherUserIds ?? new List<string>())
                {
                    //UserGroup? otherUserGroup = await GetGroupByUserId(otherUserId);

                    //if(otherUserGroup != null)
                    response.Add((userGroup.Id, otherUserId));
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new List<(int groupId, string userId)>();
            }
        }

        public async Task<List<(int groupId, string userIdWhoCanAccessThisGroup)>> RetrieveUsersWhoCanAccessTargetUserGroup(string? userId)
        {
            try
            {
                if (userId == null)
                    return new List<(int groupId, string userIdWhoCanAccessThisGroup)>();

                List<UserGroup> userGroups = await context.UserGroups.Where(x => x.OtherUserIds != null && x.OtherUserIds.Contains(userId)).ToListAsync();

                if (userGroups == null)
                    return new List<(int groupId, string userIdWhoCanAccessThisGroup)>();

                List<(int groupId, string userIdWhoCanAccessThisGroup)> response = new List<(int groupId, string userIdWhoCanAccessThisGroup)>();

                foreach (var group in userGroups)
                {
                    response.Add((group.Id, group.UserId));
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new List<(int groupId, string userId)>();
            }
        }

        public async Task<TaskResponse<bool>> RemoveUserFromGroup(int? groupId, string? userIdToRevoke)
        {
            try
            {
                if (groupId == null || userIdToRevoke == null)
                    throw new ArgumentNullException(nameof(groupId), nameof(userIdToRevoke));

                UserGroup? userGroup = await GetGroupByGroupId(groupId);

                if (userGroup == null)
                    return new TaskResponse<bool>(false, "Group does not exist");

                if (userGroup.OtherUserIds == null)
                    return new TaskResponse<bool>(false, "Group does not contain any users");

                if (userGroup.OtherUserIds.Contains(userIdToRevoke))
                {
                    context.UserGroups.Attach(userGroup);
                    userGroup.OtherUserIds.Remove(userIdToRevoke);

                    await context.SaveChangesAsync();
                    return new TaskResponse<bool>(true);
                }
                else
                {
                    return new TaskResponse<bool>(false, "User is not a member of this group");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponse<bool>(false, "An error occurred, please try again");
            }
        }

        public async Task<UserGroup?> GetGroupByGroupId(int? userGroupId)
        {
            return await context.UserGroups.FindAsync(userGroupId);
        }
    }
}
