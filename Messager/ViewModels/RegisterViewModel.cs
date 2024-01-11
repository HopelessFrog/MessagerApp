using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Messager.Helpers.ImageHelper;
using Messager.Pages.PopUps;
using Messager.Services.Authenticate;
using Messager.Services.Register;
using Microsoft.Maui.Platform;
using ServiceProvider = Messager.Services.ServiceProvider;


namespace Messager.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {

        private ServiceProvider _serviceProvider;

        public RegisterViewModel(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            AvatarSource = ImageSource.FromFile("user.svg");
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("Messager.Resources.Images.dotnet_bot.png"))
            {

                sourceByte = new byte[stream.Length];
                stream.Read(sourceByte, 0, sourceByte.Length);

            }
        }

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private ImageSource avatarSource;

        
        private byte[] sourceByte;

       
     
        


        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string loginId;


        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task SetAvatar()
        {
            var img = await UploadImage.OpenMediaPickerAsync();
            if (img != null)
            {
               
                var imageFile = await UploadImage.Upload(img);

                sourceByte = UploadImage.StringToByteBase64(imageFile.byteBase64);
                AvatarSource = ImageSource.FromStream(() =>
                    UploadImage.ByteArrayToStream(sourceByte));

            }
        }

        private void ClearFields()
        {
            AvatarSource = ImageSource.FromFile("user.svg");
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("Messager.Resources.Images.dotnet_bot.png"))
            {

                sourceByte = new byte[stream.Length];
                stream.Read(sourceByte, 0, sourceByte.Length);

            }

            Password = "";
            LoginId = "";
            UserName = "";

        }

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
                    UserName = userName,
                    Avatar = Convert.ToBase64String(sourceByte)
                };
                isBusy = true;
                var response = await _serviceProvider.Register(request);
                isBusy = false;
                if (response.StatusCode == 200)
                {
                    await AppShell.Current.DisplayAlert("", response.StatusMessage, "OK");
                    await Shell.Current.GoToAsync("..", true);
                    ClearFields();
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
