using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Messager.Models;
using Messager.Services.Interfaces;

namespace Messager.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        public LoginViewModel(ILoginService loginService)
        {
            this.loginService = loginService;
        }
        private readonly ILoginService loginService;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        private async Task  Login()
        {
            await loginService.Login(new UserInfo() { Login = this.userName, Password = this.Password });
        }
    }
}
