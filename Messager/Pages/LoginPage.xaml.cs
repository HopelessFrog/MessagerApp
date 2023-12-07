using Messager.ViewModels;

namespace Messager.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		this.BindingContext = loginViewModel;
	}
}