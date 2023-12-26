using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Messager.Models;
using Messager.Services;
using Messager.Services.ChatHub;
using Microsoft.UI.Xaml.Media.Imaging;
using ServiceProvider = Messager.Services.ServiceProvider;

namespace Messager.ViewModels
{
    public partial  class ListChatViewModel : ObservableObject, IQueryAttributable
    {
        private ChatHub _chatHub;
        private ServiceProvider _serviceProvider;

        [ObservableProperty]
        private User userInfo;
        [ObservableProperty]
        private ObservableCollection<User> userFriends;
        [ObservableProperty]
        private ObservableCollection<LastestMessage> lastestMessages;
        [ObservableProperty]
        private bool isRefreshing;


       
        public ListChatViewModel(ChatHub chatHub)
        {
            UserInfo = new User();
            UserFriends = new ObservableCollection<User>();
            LastestMessages = new ObservableCollection<LastestMessage>();
            _serviceProvider = ServiceProvider.GetInstance();
            _chatHub = chatHub;

            _chatHub.Connect();
            _chatHub.AddReceivedMessageHandler(OnReceivedMessage);

            MessagingCenter.Send<string>("StartService", "MessageForegroundService");
            MessagingCenter.Send<string, string[]>("StartService", "MessageNotificationService", new string[] { });
            
        }

        [RelayCommand]
        public async Task Refresh()
        {
            Task.Run(async() =>
            {
                IsRefreshing = true;
                await GetListFriends();
            }).GetAwaiter().OnCompleted(() =>
            {
                IsRefreshing = false;
            });
           
        }

        [RelayCommand]
        public async Task OpenChatPage(int param)
        {
            await Shell.Current.GoToAsync($"ChatPage?fromUserId={UserInfo.Id}&toUserId={param}");

        }

        async Task GetListFriends()
        {
            var response = await _serviceProvider.CallWebApi<int,ListChatInitializeResponse>
                ("/ListChat/Initialize", HttpMethod.Post, UserInfo.Id);

            if (response.StatusCode == 200)
            {
                UserInfo = response.User;
                UserInfo.AvatarSourceName = $"https://94.19.228.225:6666/ListChat/Image?userId={UserInfo.Id}";

                UserFriends = new ObservableCollection<User>(response.UserFriends);
                LastestMessages = new ObservableCollection<LastestMessage>(response.LastestMessages);
            }
            else
            {
                await AppShell.Current.DisplayAlert("ChatApp", response.StatusMessage, "OK");
            }
        }

        public async void Initialize()
        {
            Task.Run(async () =>
            {
                IsRefreshing = true;
                await GetListFriends();
            }).GetAwaiter().OnCompleted(() =>
            {
                IsRefreshing = false;
            });
        }
        public void ApplyQueryAttributes(IDictionary<string,object> query)
        {
            if (query == null || query.Count == 0) return;

            UserInfo.Id = int.Parse(HttpUtility.UrlDecode(query["userId"].ToString()));

        }

        void OnReceivedMessage(int fromUserId, string message)
        {
            var lastestMessage = lastestMessages.Where(x => x.UserFriendInfo.Id == fromUserId).FirstOrDefault();
            if (lastestMessage != null)
                lastestMessages.Remove(lastestMessage);

            var newLastestMessage = new LastestMessage
            {
                UserId = UserInfo.Id,
                Content = message,
                UserFriendInfo = UserFriends.Where(x => x.Id == fromUserId).FirstOrDefault()
            };

            lastestMessages.Insert(0, newLastestMessage);
            OnPropertyChanged("LastestMessages");

            MessagingCenter.Send<string, string[]>("Notify", "MessageNotificationService",
                new string[] { newLastestMessage.UserFriendInfo.UserName, newLastestMessage.Content });
        }

    }
}
