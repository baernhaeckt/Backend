﻿using Backend.Core.Entities;
using Backend.Core.Entities.Awards;

namespace Backend.Core.Features.Newsfeed.Events
{
    public class FriendNewsfeedBadgeReceivedEvent : FriendNewsfeedEvent
    {
        public FriendNewsfeedBadgeReceivedEvent(User user, Award award)
            : base(user)
        {
            Title = "Neuer Award";
            Message = "Dein Freund " + user.DisplayName + " hat den Award '" + award.Name + "' erhalten!";
        }
    }
}