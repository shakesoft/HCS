using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using HC.Chat.Messages;

namespace HC.Blazor.Components.Chat;

public class ChatHubConnectionService : IChatHubConnectionService, IScopedDependency, IAsyncDisposable
{
     private readonly List<Func<ChatMessageRdto, Task>> _messageReceived;
     private readonly List<Func<Guid, Task>> _messageDeleted;
     private readonly List<Func<Guid, Task>> _conversationDeleted;
     private readonly ILogger<ChatHubConnectionService> _logger;
     private readonly IJSRuntime _jsRuntime;

     private DotNetObjectReference<ChatHubConnectionService>? _objRef;

     public ChatHubConnectionService(ILogger<ChatHubConnectionService> logger, IJSRuntime jsRuntime)
     {
          _messageReceived = new List<Func<ChatMessageRdto, Task>>();
          _messageDeleted = new List<Func<Guid, Task>>();
          _conversationDeleted = new List<Func<Guid, Task>>();
          _logger = logger;
          _jsRuntime = jsRuntime;
     }
     
     public Guid LastNotificationMessageId { get; set; }

     public async Task ReceivedMessageAsync(ChatMessageRdto message)
     {
          Console.WriteLine($"ChatHubConnectionService: ReceivedMessageAsync called with {message.Id}, calling {_messageReceived.Count} registered callbacks");

          foreach (var func in _messageReceived)
          {
               Console.WriteLine("ChatHubConnectionService: Calling callback...");
               await func(message);
               Console.WriteLine("ChatHubConnectionService: Callback completed");
          }

          Console.WriteLine("ChatHubConnectionService: All callbacks completed");
     }

     public Task OnReceiveMessageAsync(Func<ChatMessageRdto, Task> func)
     {
          Console.WriteLine("ChatHubConnectionService: Registering OnReceiveMessageAsync callback");
          _messageReceived.Add(func);
          Console.WriteLine($"ChatHubConnectionService: Total callbacks registered: {_messageReceived.Count}");
          return Task.CompletedTask;
     }

     public async Task DeletedMessageAsync(Guid messageId)
     {
          foreach (var func in _messageDeleted)
          {
               await func(messageId);
          }
     }

     public Task OnDeletedMessageAsync(Func<Guid, Task> func)
     {
          _messageDeleted.Add(func);
          return Task.CompletedTask;
     }

     public async Task DeletedConversationAsync(Guid userId)
     {
          foreach (var func in _conversationDeleted)
          {
               await func(userId);
          }
     }

     public Task OnDeletedConversationAsync(Func<Guid, Task> func)
     {
          _conversationDeleted.Add(func);
          return Task.CompletedTask;
     }

     public async Task InitializeAsync(string hubUrl, string accessToken)
     {
          try
          {
               Console.WriteLine("ChatHubConnectionService: Initializing SignalR connection...");

               // Create object reference for JavaScript callbacks
               _objRef = DotNetObjectReference.Create(this);
               Console.WriteLine($"ChatHubConnectionService: Created DotNetObjectReference: {_objRef != null}");

               // Start the JavaScript SignalR connection
               Console.WriteLine("ChatHubConnectionService: Calling chatHub.start...");
               await _jsRuntime.InvokeVoidAsync("chatHub.start", _objRef);
               Console.WriteLine("ChatHubConnectionService: chatHub.start completed");

               Console.WriteLine("ChatHubConnectionService: Chat SignalR connection initialized successfully");
               _logger.LogInformation("Chat SignalR connection initialized via JavaScript");
          }
          catch (Exception ex)
          {
               Console.WriteLine($"ChatHubConnectionService: Failed to initialize SignalR connection: {ex.Message}");
               _logger.LogError(ex, "Failed to initialize chat SignalR connection");
               throw;
          }
     }

    // Test method for JS interop
    [JSInvokable]
    public void TestJSInterop()
    {
        Console.WriteLine("ChatHubConnectionService: TestJSInterop called successfully!");
    }

    // Direct SignalR message handler
    [JSInvokable]
    public async Task HandleSignalRMessage(object messageData)
    {
        Console.WriteLine($"ChatHubConnectionService: HandleSignalRMessage called with: {System.Text.Json.JsonSerializer.Serialize(messageData)}");

        // Convert dynamic object to ChatMessageRdto
        var message = new ChatMessageRdto
        {
            Id = Guid.Parse(messageData.GetType().GetProperty("Id")?.GetValue(messageData)?.ToString() ?? Guid.Empty.ToString()),
            ConversationId = Guid.TryParse(messageData.GetType().GetProperty("ConversationId")?.GetValue(messageData)?.ToString(), out var convId) ? convId : null,
            SenderUserId = Guid.Parse(messageData.GetType().GetProperty("SenderUserId")?.GetValue(messageData)?.ToString() ?? Guid.Empty.ToString()),
            SenderUsername = messageData.GetType().GetProperty("SenderUsername")?.GetValue(messageData)?.ToString(),
            SenderName = messageData.GetType().GetProperty("SenderName")?.GetValue(messageData)?.ToString(),
            SenderSurname = messageData.GetType().GetProperty("SenderSurname")?.GetValue(messageData)?.ToString(),
            Text = messageData.GetType().GetProperty("Text")?.GetValue(messageData)?.ToString()
        };

        Console.WriteLine($"ChatHubConnectionService: Forwarding message to registered callbacks");
        await ReceivedMessageAsync(message);
    }

    // Methods called by JavaScript
    [JSInvokable]
    public void OnMessageReceivedSync(object messageData)
    {
        Console.WriteLine($"ChatHubConnectionService: OnMessageReceivedSync called with: {System.Text.Json.JsonSerializer.Serialize(messageData)}");
    }

    [JSInvokable]
    public async Task OnMessageReceived(object messageData)
    {
          try
          {
               Console.WriteLine($"ChatHubConnectionService: OnMessageReceived called with data: {System.Text.Json.JsonSerializer.Serialize(messageData)}");

               // Convert dynamic object to ChatMessageRdto
               var message = new ChatMessageRdto
               {
                    Id = Guid.Parse(messageData.GetType().GetProperty("Id")?.GetValue(messageData)?.ToString() ?? Guid.Empty.ToString()),
                    ConversationId = Guid.TryParse(messageData.GetType().GetProperty("ConversationId")?.GetValue(messageData)?.ToString(), out var convId) ? convId : null,
                    SenderUserId = Guid.Parse(messageData.GetType().GetProperty("SenderUserId")?.GetValue(messageData)?.ToString() ?? Guid.Empty.ToString()),
                    SenderUsername = messageData.GetType().GetProperty("SenderUsername")?.GetValue(messageData)?.ToString(),
                    SenderName = messageData.GetType().GetProperty("SenderName")?.GetValue(messageData)?.ToString(),
                    SenderSurname = messageData.GetType().GetProperty("SenderSurname")?.GetValue(messageData)?.ToString(),
                    Text = messageData.GetType().GetProperty("Text")?.GetValue(messageData)?.ToString()
               };

               Console.WriteLine($"ChatHubConnectionService: Converted to ChatMessageRdto - Id: {message.Id}, Sender: {message.SenderUsername}, Text: {message.Text}, ConversationId: {message.ConversationId}");

               // Use Task.Run to ensure we're not blocking the JS interop thread
               Task.Run(async () =>
               {
                    Console.WriteLine("ChatHubConnectionService: Calling ReceivedMessageAsync in Task.Run...");
                    try
                    {
                         await ReceivedMessageAsync(message);
                         Console.WriteLine("ChatHubConnectionService: ReceivedMessageAsync completed");
                    }
                    catch (Exception ex)
                    {
                         Console.WriteLine($"ChatHubConnectionService: Error in Task.Run: {ex.Message}");
                    }
               });
          }
          catch (Exception ex)
          {
               _logger.LogError(ex, "Error processing received message");
               Console.WriteLine($"ChatHubConnectionService: Error processing message: {ex.Message}");
          }
     }

     [JSInvokable]
     public async Task OnMessageDeleted(Guid messageId)
     {
          await DeletedMessageAsync(messageId);
     }

     [JSInvokable]
     public async Task OnConversationDeleted(Guid userId)
     {
          await DeletedConversationAsync(userId);
     }

     public async ValueTask DisposeAsync()
     {
          try
          {
               // Stop the JavaScript SignalR connection
               await _jsRuntime.InvokeVoidAsync("chatHub.stop");
               _logger.LogInformation("Chat SignalR connection disposed");
          }
          catch (Exception ex)
          {
               _logger.LogWarning(ex, "Error disposing chat SignalR connection");
          }

          // Dispose object reference
          _objRef?.Dispose();
          _objRef = null;
     }

     public bool IsConnected => true; // For now, assume connected since we can't check JS connection status easily
}
