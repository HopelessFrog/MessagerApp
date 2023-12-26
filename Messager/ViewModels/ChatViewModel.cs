using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CommunityToolkit.Mvvm.Input;
using Messager.Models;
using Messager.Services.ChatHub;
using Messager.Services.Message;
using ServiceProvider = Messager.Services.ServiceProvider;

namespace Messager.ViewModels
{
    public partial class ChatViewModel : ObservableObject, IQueryAttributable
    {
        public ChatViewModel(ChatHub chatHub)
        {
            Messages = new ObservableCollection<Message>();
            _serviceProvider = ServiceProvider.GetInstance();
            _chatHub = chatHub;
            _chatHub.AddReceivedMessageHandler(OnReceiveMessage);
            _chatHub.Connect();
        }
        private ChatHub _chatHub;
        private ServiceProvider _serviceProvider;

        [ObservableProperty] 
        private int fromUserId;
        [ObservableProperty]
        private int toUserId;

        [ObservableProperty]
        private User friendInfo;
        [ObservableProperty]
        private ObservableCollection<Message> messages;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string message;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query == null || query.Count == 0) return;

            FromUserId = int.Parse(HttpUtility.UrlDecode(query["fromUserId"].ToString()));
            ToUserId = int.Parse(HttpUtility.UrlDecode(query["toUserId"].ToString()));
        }

        [RelayCommand]
        public async Task SendMessage()
        {
            try
            {
                if (Message.Trim() != "")
                {
                    await _chatHub.SendMessageToUser(FromUserId, ToUserId, Message);

                    Messages.Add(new Models.Message
                    {
                        Content = Message,
                        FromUserId = FromUserId,
                        ToUserId = ToUserId,
                        SendDateTime = DateTime.Now
                    });

                    Message = "";
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("ChatApp", ex.Message, "OK");
            }
        }

        private async Task GetMessages()
        {
            var request = new MessageInitializeRequest
            {
                FromUserId = FromUserId,
                ToUserId = ToUserId,
            };

            var response = await _serviceProvider.CallWebApi<MessageInitializeRequest, MessageInitializeReponse>
                ("/Message/Initialize", HttpMethod.Post, request);

            if (response.StatusCode == 200)
            {
                FriendInfo = response.FriendInfo;
                Messages = new ObservableCollection<Message>(response.Messages);
            }
            else
            {
                await AppShell.Current.DisplayAlert("ChatApp", response.StatusMessage, "OK");
            }
        }
        public void Initialize()
        {
            Task.Run(async () =>
            {
                IsRefreshing = true;
                await GetMessages();
            }).GetAwaiter().OnCompleted(() =>
            {
                IsRefreshing = false;
            });
        }

        private void OnReceiveMessage(int fromUserId, string message)
        {
            Messages.Add(new Models.Message
            {
                Content = message,
                FromUserId = ToUserId,
                ToUserId = FromUserId,
                SendDateTime = DateTime.Now
            });
        }


    }


}
