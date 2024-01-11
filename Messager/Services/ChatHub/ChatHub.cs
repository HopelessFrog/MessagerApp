using System.Diagnostics;
using Messager.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using ServiceProvider = Messager.Services.ServiceProvider;
namespace Messager.Services.ChatHub;

public class ChatHub
{
    private readonly HubConnection hubConnection;
    private readonly ServiceProvider _serviceProvider;
    private readonly List<Action<int, string>> onReceiveMessageHandler;

    public ChatHub(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var devSslHelper = new DevHttpsConnectionHelper(sslPort: 6666);


        hubConnection = new HubConnectionBuilder()
            .WithUrl(devSslHelper.DevServerRootUrl + "/ChatHub", options =>
            {
                options.Headers.Add("ChatHubBearer", _serviceProvider._accessToken);

                options.HttpMessageHandlerFactory = m => devSslHelper.GetPlatformMessageHandler();
            }).Build();

        onReceiveMessageHandler = new List<Action<int, string>>();
        hubConnection.On<int, string>("ReceiveMessage", OnReceiveMessage);
    }

    public async Task Connect()
    {
        try
        {
            Debug.Print("try connect ");
            await hubConnection.StartAsync();
            Debug.Print("connected");
        }
        catch (Exception e)
        {

            throw e;
        }
       
        var qwe = hubConnection.State;
        var www = hubConnection.State;
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