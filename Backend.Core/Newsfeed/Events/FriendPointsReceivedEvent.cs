﻿using Backend.Database;

namespace Backend.Core.Newsfeed
{
    public class FriendPointsReceivedEvent : FriendEvent
    {
        public FriendPointsReceivedEvent(User user, int points) :
            base(user)
        {
            title = "Neue Punkte";
            message = "Dein Freund " + user.DisplayName + " hat " + points + " Suffizienz Punkte erhalten!";
        }
    }
}