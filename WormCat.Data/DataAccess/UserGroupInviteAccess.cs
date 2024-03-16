using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using WormCat.Data.Data;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;

namespace WormCat.Data.DataAccess
{
    public class UserGroupInviteAccess : IUserGroupInviteAccess
    {
        private readonly ILogger<UserGroupInviteAccess> logger;
        private readonly WormCatRazorContext context;
        private readonly IUserAccess userAccess;

        public UserGroupInviteAccess(ILogger<UserGroupInviteAccess> logger, WormCatRazorContext context, IUserAccess userAccess)
        {
            this.logger = logger;
            this.context = context;
            this.userAccess = userAccess;
        }

        public async Task<List<UserGroupInvite>> GetInvitesSent(string? userId)
        {
            try
            {
                return await context.UserGroupInvite.Where(x => x.UserIdFrom == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Issue retrieving sent invites for user {userId}");
                return new List<UserGroupInvite>();
            }
        }

        public async Task<List<UserGroupInvite>> GetInvitesReceived(string? userId)
        {
            try
            {
                return await context.UserGroupInvite.Where(x => x.UserIdTo == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"Issue retrieving invites received for user {userId}");
                return new List<UserGroupInvite>();
            }
        }

        public async Task<TaskResponse<bool>> TrySendInviteAsync(string? userIdFrom, string? userIdTo)
        {
            try
            {
                User? userFrom = await userAccess.FindUserByCustomUsernameOrEmailAddressOrId(userIdFrom);
                User? userTo = await userAccess.FindUserByCustomUsernameOrEmailAddressOrId(userIdTo);

                if (userFrom == null || userTo == null)
                    return new TaskResponse<bool>(false, "Could not find user");

                // You cant send an invite to yourself!
                if (userFrom.Id == userTo.Id)
                    return new TaskResponse<bool>(false, "Can't send an invite to yourself");

                // Don't send invite if an invite already exists
                UserGroupInvite? existingInvite = await context.UserGroupInvite.FirstOrDefaultAsync(x => x.UserIdFrom == userFrom.Id && x.UserIdTo == userTo.Id);

                if (existingInvite != null)
                    return new TaskResponse<bool>(false, $"You have already invited {userTo.CustomUsername} to your group");

                // Don't send invite if these two users exist in a group already
                UserGroup? existingUserGroup = await context.UserGroups.FirstOrDefaultAsync(x => x.UserId == userTo.Id);

                // User does not have an existing group
                // So invite can be sent
                if (existingUserGroup == null)
                    return await SendInviteAsync(userFrom.Id, userTo.Id);

                // Ensure list is initialised
                if (existingUserGroup.OtherUserIds == null)
                    existingUserGroup.OtherUserIds = new List<string>();

                if (existingUserGroup.OtherUserIds.Contains(userFrom.Id))
                {
                    // User Group pair already exists
                    // No need to send invite
                    return new TaskResponse<bool>(false, $"{userTo.CustomUsername} is already a member of your group");
                }
                else
                {
                    // User group pair does NOT exist
                    // Send the invite
                    return await SendInviteAsync(userFrom.Id, userTo.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponse<bool>(false, "An error occurred, please try again");
            }
        }

        private async Task<TaskResponse<bool>> SendInviteAsync(string userIdFrom, string userIdTo)
        {
            try
            {
                EntityEntry<UserGroupInvite> entityEntry = await context.UserGroupInvite.AddAsync(new UserGroupInvite()
                {
                    UserIdFrom = userIdFrom,
                    UserIdTo = userIdTo
                });

                if (entityEntry.Entity == null)
                    throw new Exception("Database entity returned null");

                await context.SaveChangesAsync();
                return new TaskResponse<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponse<bool>(false, $"An error occurred, please try again");
            }
        }

        public async Task<TaskResponse<bool>> TryAcceptInviteAsync(int? inviteId)
        {
            try
            {
                UserGroupInvite? userGroupInvite = await context.UserGroupInvite.FindAsync(inviteId);

                if (userGroupInvite == null)
                    return new TaskResponse<bool>(false, "Invite is no longer valid");

                // Fetch an existing user group, or null
                UserGroup? existingUserGroup = await context.UserGroups.FirstOrDefaultAsync(x => x.UserId == userGroupInvite.UserIdTo);

                // No existing group for this user
                // Create one, and add the pair
                if (existingUserGroup == null)
                {
                    UserGroup newUserGroup = new UserGroup()
                    {
                        UserId = userGroupInvite.UserIdTo,
                        OtherUserIds = new List<string>() { userGroupInvite.UserIdFrom }
                    };

                    // Create the user group
                    await context.UserGroups.AddAsync(newUserGroup);

                    // Delete the invite
                    context.UserGroupInvite.Remove(userGroupInvite);

                    await context.SaveChangesAsync();
                    return new TaskResponse<bool>(true);
                }

                // Ensure list is initialised
                if (existingUserGroup.OtherUserIds == null)
                    existingUserGroup.OtherUserIds = new List<string>();

                if (existingUserGroup.OtherUserIds.Contains(userGroupInvite.UserIdFrom))
                {
                    // User Group pair already exists
                    // No need to add again
                    // Delete the invite
                    context.UserGroupInvite.Remove(userGroupInvite);

                    await context.SaveChangesAsync();
                    return new TaskResponse<bool>(false, $"You are already a member of this group");
                }
                else
                {
                    // User group pair does NOT exist
                    context.UserGroups.Attach(existingUserGroup);
                    existingUserGroup.OtherUserIds.Add(userGroupInvite.UserIdFrom);

                    // Delete the invite
                    context.UserGroupInvite.Remove(userGroupInvite);

                    await context.SaveChangesAsync();    
                    return new TaskResponse<bool>(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponse<bool>(false, "An error occurred, please try again");
            }
        }

        public async Task<TaskResponse<bool>> TryDeclineInviteAsync(int? inviteId)
        {
            try
            {
                UserGroupInvite? userGroupInvite = await context.UserGroupInvite.FindAsync(inviteId);

                if (userGroupInvite == null)
                    return new TaskResponse<bool>(false, "Invite not found");

                context.UserGroupInvite.Remove(userGroupInvite);

                await context.SaveChangesAsync();
                return new TaskResponse<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponse<bool>(false, "An error occurred, please try again");
            }
        }

        public async Task<TaskResponse<bool>> TryCancelInviteAsync(int? inviteId)
        {
            try
            {
                UserGroupInvite? userGroupInvite = await context.UserGroupInvite.FindAsync(inviteId);

                if (userGroupInvite == null)
                    return new TaskResponse<bool>(false, "Invite not found");

                context.UserGroupInvite.Remove(userGroupInvite);

                await context.SaveChangesAsync();
                return new TaskResponse<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new TaskResponse<bool>(false, "An error occurred, please try again");
            }
        }

        public async Task<UserGroupInvite?> GetInvite(int? inviteId)
        {
            return await context.UserGroupInvite.FindAsync(inviteId);
        }
    }
}