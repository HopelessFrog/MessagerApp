using Microsoft.AspNetCore.SignalR.Client;
using ServiceProvider = Messager.Services.ServiceProvider;
namespace Messager.Services.ChatHub;

public class ChatHub
{
    private readonly HubConnection hubConnection;
    private readonly ServiceProvider _serviceProvider;
    private readonly List<Action<int, string>> onReceiveMessageHandler;

    public ChatHub()
    {
        _serviceProvider = ServiceProvider.GetInstance();

        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://94.19.228.225:6666" + "/ChatHub",
                options => { options.Headers.Add("ChatHubBearer", _serviceProvider._accessToken); }).Build();

        onReceiveMessageHandler = new List<Action<int, string>>();
        hubConnection.On<int, string>("ReceiveMessage", OnReceiveMessage);
    }

    public async Task Connect()
    {
        await hubConnection.StartAsync();
    }

    public async Task Disconnect()
    {
        await hubConnection?.StopAsync();
    }

    public async Task SendMessageToUser(int fromUserId, int toUserId, string message)
    {
        await hubConnection.InvokeAsync("SendMessageToUser", fromUserId, toUserId, message);
    }

    public void AddReceivedMessageHandler(Action<int, string> handler)
    {
        onReceiveMessageHandler.Add(handler);
    }

    private void OnReceiveMessage(int userId, string message)
    {
        foreach (var handler in onReceiveMessageHandler) handler(userId, message);
    }
}