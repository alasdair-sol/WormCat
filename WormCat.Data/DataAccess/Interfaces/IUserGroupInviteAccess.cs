
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess.Interfaces
{
    public interface IUserGroupInviteAccess
    {
        Task<List<UserGroupInvite>> GetInvitesReceived(string? userId);
        Task<List<UserGroupInvite>> GetInvitesSent(string? userId);
        Task<UserGroupInvite?> GetInvite(int? inviteId);
        Task<TaskResponse<bool>> TryAcceptInviteAsync(int? inviteId);
        Task<TaskResponse<bool>> TryCancelInviteAsync(int? inviteId);
        Task<TaskResponse<bool>> TryDeclineInviteAsync(int? inviteId);
        Task<TaskResponse<bool>> TrySendInviteAsync(string? userIdFrom, string? userIdTo);
    }
}
