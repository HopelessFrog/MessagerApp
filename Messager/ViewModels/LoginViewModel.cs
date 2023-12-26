using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Messager.Models;
using Messager.Pages;
using Messager.Pages.PopUps;
using Messager.Services.Authenticate;
using ServiceProvider = Messager.Services.ServiceProvider;

namespace Messager.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string password;


        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task  Login()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    await AppShell.Current.DisplayAlert("", "Enter password and username", "OK");
                    return;
                }
              
                var request = new AuthenticateRequest()
                {
                    LoginId = userName,
                    Password = password
                };
                isBusy = true;
                var response = await ServiceProvider.GetInstance().Authenticate(request);
                isBusy = false;
                if (response.StatusCode == 200)
                {
                    await Shell.Current.GoToAsync($"ListChatPage?userId={response.Id}"); 
                }
                else
                {
                    await AppShell.Current.DisplayAlert("", response.StatusMessage , "OK");

                }

            }
            else
            {
                App.Current.MainPage.ShowPopup(new NoInternetPopUp());
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}/{nameof(RegisterPage)}");

            }
            else
            {
                App.Current.MainPage.ShowPopup(new NoInternetPopUp());
            }
           
        }
    }
}
