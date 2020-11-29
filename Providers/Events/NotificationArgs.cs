using System;
using Sample.SSEvent.Models;

namespace Sample.SSEvent.Providers.Events
{
    public class NotificationArgs : EventArgs
    {
        public Notification Notification { get; }

        public NotificationArgs(Notification notification)
        {
            Notification = notification;
        }
    }
}