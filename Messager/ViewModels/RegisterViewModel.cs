using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Messager.Pages.PopUps;
using Messager.Services.Authenticate;
using Messager.Services.Register;
using ServiceProvider = Messager.Services.ServiceProvider;


namespace Messager.ViewModels
{
    partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string loginId;


        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task Register()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(loginId))
                {
                    await AppShell.Current.DisplayAlert("", "Enter password, login  and username", "OK");
                    return;
                }

                var request = new RegisterRequest()
                {
                    LoginId = loginId,
                    Password = password,
                    UserName = userName
                };
                isBusy = true;
                var response = await ServiceProvider.GetInstance().Register(request);
                isBusy = false;
                if (response.StatusCode == 200)
                {
                    await AppShell.Current.DisplayAlert("", response.StatusMessage, "OK");
                }
                else
                {
                    await AppShell.Current.DisplayAlert("", response.StatusMessage, "OK");

                }

            }
            else
            {
                App.Current.MainPage.ShowPopup(new NoInternetPopUp());
            }
        }
    }
}
