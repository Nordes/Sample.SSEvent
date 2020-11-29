
using System;
using Microsoft.Extensions.Logging;
using Sample.SSEvent.Models;
using Sample.SSEvent.Providers.Events;

namespace Sample.SSEvent.Providers
{
    public interface IMessageRepository
    {
        event EventHandler<NotificationArgs> NotificationEvent;
        void Broadcast(Notification notification);
    }

    public class MessageRepository : IMessageRepository
    {
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(ILogger<MessageRepository> logger)
        {
            _logger = logger;
        }

        public event EventHandler<NotificationArgs> NotificationEvent;

        public void Broadcast(Notification notification)
        {
            _logger.LogInformation("Broadcasting event to all event listener");
            NotificationEvent?.Invoke(this, new NotificationArgs(notification));
        }
    }
}
