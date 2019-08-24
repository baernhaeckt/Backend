﻿using AspNetCore.MongoDB;
using Backend.Core.Security.Extensions;
using Backend.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Core.Services
{
    public class FriendsService : PersonalizedService
    {
        public FriendsService(IMongoOperation<User> userResponseRepository, ClaimsPrincipal principal)
            : base(userResponseRepository, principal)
        { }

        public async Task AddFriend(string friendEmail)
        {
            if (String.IsNullOrEmpty(friendEmail))
            {
                throw new WebException("email must not be empty", System.Net.HttpStatusCode.BadRequest);
            }

            User friendUser = UserRepository.GetQuerableAsync().SingleOrDefault(u => u.Email == friendEmail);

            if (friendUser == null)
            {
                // TODO: Invite to Platform
                throw new WebException($"no user with email: {friendEmail} found.", System.Net.HttpStatusCode.BadRequest);
            }

            if (CurrentUser.Id == friendUser.Id)
            {
                throw new WebException($"can't be your own friend.", System.Net.HttpStatusCode.BadRequest);
            }

            var friends = (CurrentUser.Friends ?? Enumerable.Empty<string>()).ToList();
            if (friends.Contains(friendUser.Id))
            {
                throw new WebException("user is already your friend", System.Net.HttpStatusCode.BadRequest);
            }
            
            friends.Add(friendUser.Id);
            var user = CurrentUser;
            user.Friends = friends;
            await UserRepository.UpdateAsync(user.Id, user);

            var friendsFriend = (friendUser.Friends ?? Enumerable.Empty<string>()).ToList();
            if (!friendsFriend.Contains(CurrentUser.Id))
            {
                friendsFriend.Add(CurrentUser.Id);
                friendUser.Friends = friendsFriend;
                await UserRepository.UpdateAsync(friendUser.Id, friendUser);
            }
        }

        public async Task RemoveFriend(string friendUserId)
        {
            User user = CurrentUser;

            var friends = user.Friends.ToList();
            if (!friends.Contains(friendUserId))
            {
                throw new WebException("user is not your friend", System.Net.HttpStatusCode.BadRequest);
            }

            friends.Remove(friendUserId);
            user.Friends = friends;
            await UserRepository.UpdateAsync(user.Id, user);

            User exFriend = await UserRepository.GetByIdAsync(friendUserId);

            var exFriendFriends = exFriend.Friends.ToList();
            if (exFriendFriends.Contains(CurrentUser.Id))
            {
                exFriendFriends.Remove(CurrentUser.Id);
                exFriend.Friends = exFriendFriends;
                await UserRepository.UpdateAsync(exFriend.Id, exFriend);
            }
        }

        public IEnumerable<User> Friends
        {
            get
            {
                if (CurrentUser.Friends == null)
                {
                    return Enumerable.Empty<User>();
                }

                var friends =  CurrentUser.Friends.Select(refId => UserRepository.GetByIdAsync(refId.ToString()).Result).ToList();
                return friends;
            }
        }
    }
}
