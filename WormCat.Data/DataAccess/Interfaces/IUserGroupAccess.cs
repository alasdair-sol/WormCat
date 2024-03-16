using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IUserGroupAccess
    {
        /// <summary>
        /// Returns a list of groupIds and userIds that a given user has access to
        /// The groupId is the id of the group the given user has access to
        /// The userId belongs to the user who owns that group
        /// </summary>
        /// <param name="userId">The given user to find all access ids for</param>
        /// <returns>Returns a list of groupIds and userIds that a given user has access to</returns>
        Task<List<(int groupId, string rootUserId)>> GetGroupsTargetUserCanAccess(string? userId);
        Task<List<(int groupId, string userIdWhoCanAccessThisGroup)>> RetrieveUsersWhoCanAccessTargetUserGroup(string? userId);
        Task<TaskResponse<bool>> RemoveUserFromGroup(int? groupId, string? userIdToRevoke);
        Task<UserGroup?> GetGroupByUserId(string? userId);
        Task<UserGroup?> GetGroupByGroupId(int? userGroupId);
    }
}
