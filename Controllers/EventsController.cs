#nullable enable
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.SSEvent.Models;
using Sample.SSEvent.Providers;
using Sample.SSEvent.Providers.Events;

namespace Sample.SSEvent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly ILogger<EventsController> _logger;

        public EventsController(ILogger<EventsController> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            // this.jsonSettings = new JsonSerializerSettings();
            // jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        /// <summary>
        /// Produce SSE
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Produces("text/event-stream")]
        [HttpGet]
        public async Task SubscribeEvents(CancellationToken cancellationToken)
        {
            SetServerSentEventHeaders();
            // On connect, welcome message ;)
            var data = new { Message = "connected!" };
            var jsonConnection = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            await Response.WriteAsync($"event:connection\n", cancellationToken);
            await Response.WriteAsync($"data: {jsonConnection}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);

            async void OnNotification(object? sender, NotificationArgs eventArgs)
            {
                try
                {
                    // idea: https://stackoverflow.com/a/58565850/80527
                    var json = JsonSerializer.Serialize(eventArgs.Notification, _jsonSerializerOptions);
                    await Response.WriteAsync($"id:{eventArgs.Notification.Id}\n", cancellationToken);
                    await Response.WriteAsync("retry: 10000\n", cancellationToken);
                    await Response.WriteAsync($"event:snackbar\n", cancellationToken);
                    await Response.WriteAsync($"data:{json}\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
                catch (Exception)
                {
                    _logger.LogError("Not able to send the notification");
                }
            }

            _messageRepository.NotificationEvent += OnNotification;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Spin until something break or stop...
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogDebug("User most likely disconnected");
            }
            finally
            {
                _messageRepository.NotificationEvent -= OnNotification;
            }
        }

        private void SetServerSentEventHeaders()
        {
            Response.StatusCode = 200;
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
        }

        [HttpPost("broadcast")]
        public Task Broadcast([FromBody] Notification notification)
        {
            _messageRepository.Broadcast(notification);

            return Task.CompletedTask;
        }
    }
}
