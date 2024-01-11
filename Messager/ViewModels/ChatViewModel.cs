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
using Crypt;

namespace Messager.ViewModels
{
    public partial class ChatViewModel : ObservableObject, IQueryAttributable
    {
        public ChatViewModel(ChatHub chatHub , ServiceProvider serviceProvider)
        {
            Messages = new ObservableCollection<Message>();
            _serviceProvider = serviceProvider;
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
        [NotifyPropertyChangedFor(nameof(NoRefreshing))]
        private bool isRefreshing;

        public bool NoRefreshing
        {
            get
            {
                return !isRefreshing;
            }
        }

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
                _chatHub.Connect();
                if (Message.Trim() != "")
                {
                    await _chatHub.SendMessageToUser(FromUserId, ToUserId, Crypter.Encrypt(Message, FromUserId + ToUserId));

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
                var tempMesaage = response.Messages.ToList();
                for (int i = 0; i < tempMesaage.Count; i++)
                {
                    tempMesaage[i].Content = Crypter.Decrypt(tempMesaage[i].Content,
                        tempMesaage[i].FromUserId + tempMesaage[i].ToUserId);
                }

                Messages = new ObservableCollection<Message>(tempMesaage);
            }
            else
            {
                await AppShell.Current.DisplayAlert("ChatApp", response.StatusMessage, "OK");
            }
        }
        public async Task Initialize()
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
                Content = Crypter.Decrypt(message, ToUserId + FromUserId),
                FromUserId = ToUserId,
                ToUserId = FromUserId,
                SendDateTime = DateTime.Now
            });
        }


    }


}
