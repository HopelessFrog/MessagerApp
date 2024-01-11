using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Crypt;
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


       
        public ListChatViewModel(ChatHub chatHub, ServiceProvider serviceProvider)
        {
            UserInfo = new User();
            UserFriends = new ObservableCollection<User>();
            LastestMessages = new ObservableCollection<LastestMessage>();
            _serviceProvider = serviceProvider;
            _chatHub = chatHub;

            _chatHub.Connect();
            _chatHub.AddReceivedMessageHandler(OnReceivedMessage);

            MessagingCenter.Send<string>("StartService", "MessageForegroundService");
            MessagingCenter.Send<string, string[]>("StartService", "MessageNotificationService", new string[] { });
            
        }

        [RelayCommand]
        public  async void LogOut()
        {
            UserInfo = new User();
            UserFriends = new ObservableCollection<User>();
            LastestMessages = new ObservableCollection<LastestMessage>();
            await Shell.Current.GoToAsync("..", true);


        }
        [RelayCommand]
        public async Task Refresh()
        {
            Task.Run(async() =>
            {
                IsRefreshing = true;
                await GetListFriends();
            }).GetAwaiter().OnCompleted( async () =>
            {
                await Task.Delay(500);
                IsRefreshing = false;
            });
           
        }

        [RelayCommand]
        public async Task OpenChatPage(int param)
        {
            await Shell.Current.GoToAsync($"ChatPage?fromUserId={UserInfo.Id}&toUserId={param}", true);

        }

        async Task GetListFriends()
        {
            var response = await _serviceProvider.CallWebApi<int,ListChatInitializeResponse>
                ("/ListChat/Initialize", HttpMethod.Post, UserInfo.Id);

            if (response.StatusCode == 200)
            {
                UserInfo = response.User;


                var tempMesaage = response.LastestMessages.ToList();
                for (int i = 0; i < tempMesaage.Count; i++)
                {
                    tempMesaage[i].Content = Crypter.Decrypt(tempMesaage[i].Content,
                        tempMesaage[i].UserId + tempMesaage[i].UserFriendInfo.Id);
                }

                UserFriends = new ObservableCollection<User>(response.UserFriends);
                LastestMessages = new ObservableCollection<LastestMessage>(tempMesaage);
                
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
            }).GetAwaiter().OnCompleted(async () =>
            {
                await Task.Delay(500);

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

            Crypter.Decrypt("qwe", 3);

            var newLastestMessage = new LastestMessage
            {
                UserId = UserInfo.Id,
                Content = Crypter.Decrypt(message, UserInfo.Id + fromUserId),
                UserFriendInfo = UserFriends.Where(x => x.Id == fromUserId).FirstOrDefault()
            };

            lastestMessages.Insert(0, newLastestMessage);
            OnPropertyChanged("LastestMessages");

            MessagingCenter.Send<string, string[]>("Notify", "MessageNotificationService",
                new string[] { newLastestMessage.UserFriendInfo.UserName, newLastestMessage.Content });
        }

    }
}
