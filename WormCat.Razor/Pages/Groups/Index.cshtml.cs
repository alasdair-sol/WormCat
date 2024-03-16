using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using WormCat.Data.DataAccess.Interfaces;
using WormCat.Library.Models;
using WormCat.Library.Models.Dbo;
using WormCat.Razor.Areas.Identity.Data;
using WormCat.Razor.Models;
using WormCat.Razor.Utility;

namespace WormCat.Razor.Pages.Groups
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly IUserGroupAccess _userGroupAccess;
        private readonly IUserGroupInviteAccess userGroupInviteAccess;
        private readonly IAuthUtility _authUtility;

        public List<UserGroupInviteDisplay> InvitesSentDisplayModels { get; set; } = new List<UserGroupInviteDisplay>();
        public List<UserGroupInviteDisplay> InvitesReceivedDisplayModels { get; set; } = new List<UserGroupInviteDisplay>();
        public List<UserGroupDisplay>? GroupsUserCanAccess { get; set; } = null;
        public List<UserGroupDisplay>? UsersWhoCanAccessCurrentUserGroup { get; set; } = null;

        public IndexModel(ILogger<IndexModel> logger, IUserGroupAccess userGroupAccess, IUserGroupInviteAccess userGroupInviteAccess, UserManager<WormCatUser> userManager, IAuthUtility authUtility)
        {
            this.logger = logger;
            _userGroupAccess = userGroupAccess;
            this.userGroupInviteAccess = userGroupInviteAccess;
            _authUtility = authUtility;
        }

        private async Task RetrieveData()
        {
            await RetrieveInvites();
            await RetrieveUsersWhoCanAccessCurrentUser();
            await RetrieveGroupsCurrentUserCanAccess();
        }

        private async Task RetrieveInvites()
        {
            var tmpInvitesSent = await userGroupInviteAccess.GetInvitesSent(User.GetUserId<string>() ?? string.Empty);
            var tmpInvitesReceived = await userGroupInviteAccess.GetInvitesReceived(User.GetUserId<string>() ?? string.Empty);

            foreach (var item in tmpInvitesSent)
            {
                var user = new UserGroupInviteDisplay()
                {
                    Id = item.Id,
                    UserIdFrom = item.UserIdFrom,
                    UserIdTo = item.UserIdTo,
                    UsernameFrom = await _authUtility.GetCustomUsernameByIdAsync(item.UserIdFrom),
                    UsernameTo = await _authUtility.GetCustomUsernameByIdAsync(item.UserIdTo),
                };

                if (user.UsernameFrom == null)
                    continue;

                if (user.UsernameTo == null)
                    continue;

                InvitesSentDisplayModels.Add(user);
            }

            foreach (var item in tmpInvitesReceived)
            {
                var user = new UserGroupInviteDisplay()
                {
                    Id = item.Id,
                    UserIdFrom = item.UserIdFrom,
                    UserIdTo = item.UserIdTo,
                    UsernameFrom = await _authUtility.GetCustomUsernameByIdAsync(item.UserIdFrom),
                    UsernameTo = await _authUtility.GetCustomUsernameByIdAsync(item.UserIdTo),
                };

                if (user.UsernameFrom == null)
                    continue;

                if (user.UsernameTo == null)
                    continue;

                InvitesReceivedDisplayModels.Add(user);
            }
        }

        private async Task RetrieveUsersWhoCanAccessCurrentUser()
        {
            List<(int groupId, string userIdWhoCanAccessThisGroup)> list = await _userGroupAccess.RetrieveUsersWhoCanAccessTargetUserGroup(User.GetUserId<string>() ?? string.Empty);

            if (list == null || list.Count <= 0)
                return;

            UsersWhoCanAccessCurrentUserGroup = new List<UserGroupDisplay>();

            foreach (var item in list)
            {
                UserGroupDisplay display = new UserGroupDisplay()
                {
                    GroupId = item.groupId,
                    RootUserId = User.GetUserId<string>(),
                    AccessUserId = item.userIdWhoCanAccessThisGroup                    
                };

                display.RootCustomUsername = await _authUtility.GetCustomUsernameByIdAsync(display.RootUserId) ?? string.Empty;
                display.AccessCustomUsername = await _authUtility.GetCustomUsernameByIdAsync(display.AccessUserId) ?? string.Empty;

                UsersWhoCanAccessCurrentUserGroup.Add(display);
            }
        }

        private async Task RetrieveGroupsCurrentUserCanAccess()
        {
            List<(int groupId, string rootUserId)> list = await _userGroupAccess.GetGroupsTargetUserCanAccess(User.GetUserId<string>() ?? string.Empty);

            if (list == null || list.Count <= 0)
                return;

            GroupsUserCanAccess = new List<UserGroupDisplay>();

            foreach (var item in list)
            {
                UserGroupDisplay display = new UserGroupDisplay()
                {
                    GroupId = item.groupId,
                    RootUserId = item.rootUserId,
                    AccessUserId = User.GetUserId<string>()
                };

                display.RootCustomUsername = await _authUtility.GetCustomUsernameByIdAsync(display.RootUserId) ?? string.Empty;
                display.AccessCustomUsername = await _authUtility.GetCustomUsernameByIdAsync(display.AccessCustomUsername) ?? string.Empty;

                GroupsUserCanAccess.Add(display);
            }
        }

        public async Task<IActionResult> OnGet()
        {
            await RetrieveData();

            //if (param != null && param == "true")
            //    ViewData["InviteSent"] = true;

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveUserFromGroup(int? groupId, string? userId)
        {
            TaskResponse<bool> taskResponse = await _userGroupAccess.RemoveUserFromGroup(groupId, userId);

            if(taskResponse.Result)
            {
                ViewData["success"] = "Success!";
            }
            else
            {
                if (taskResponse.Output != null)
                    ModelState.AddModelError(string.Empty, taskResponse.Output);
            }

            await RetrieveData();
            return Page();
        }

        public async Task<IActionResult> OnPostSendInvite(string? targetUser)
        {
            TaskResponse<bool> taskResponse = await userGroupInviteAccess.TrySendInviteAsync(User.GetUserId<string>(), targetUser);

            if (taskResponse.Result)
            {
                ViewData["success"] = "Invite Sent!";
                //return LocalRedirect("/Groups/Index?param=true");
            }
            else
            {
                if (taskResponse.Output != null)
                    ModelState.AddModelError(string.Empty, taskResponse.Output);
            }

            await RetrieveData();
            return Page();
        }

        public async Task<IActionResult> OnPostAcceptInvite(int? inviteId)
        {
            UserGroupInvite? invite = await userGroupInviteAccess.GetInvite(inviteId);
            string? usernameFrom = await _authUtility.GetCustomUsernameByIdAsync(invite?.UserIdFrom ?? string.Empty);

            TaskResponse<bool> taskResponse = await userGroupInviteAccess.TryAcceptInviteAsync(inviteId);

            if (taskResponse.Result)
            {
                if (usernameFrom != null)
                    ViewData["success"] = $"Invite accepted! You are now a member of {usernameFrom}'s group.";
                else
                    ViewData["success"] = $"Invite accepted!";
            }
            else
            {
                if (taskResponse.Output != null)
                    ModelState.AddModelError(string.Empty, taskResponse.Output);
            }

            await RetrieveData();
            return Page();
        }

        public async Task<IActionResult> OnPostDeclineInvite(int? inviteId)
        {
            TaskResponse<bool> taskResponse = await userGroupInviteAccess.TryDeclineInviteAsync(inviteId);

            if (taskResponse.Result)
            {
                ViewData["success"] = $"Invite declined!";
            }
            else
            {
                if (taskResponse.Output != null)
                    ModelState.AddModelError(string.Empty, taskResponse.Output);
            }

            await RetrieveData();
            return Page();
        }

        public async Task<IActionResult> OnPostCancelInvite(int? inviteId)
        {
            TaskResponse<bool> taskResponse = await userGroupInviteAccess.TryCancelInviteAsync(inviteId);

            if (taskResponse.Result)
            {
                ViewData["success"] = $"Invite cancelled!";
            }
            else
            {
                if (taskResponse.Output != null)
                    ModelState.AddModelError(string.Empty, taskResponse.Output);
            }

            await RetrieveData();
            return Page();
        }
    }
}
