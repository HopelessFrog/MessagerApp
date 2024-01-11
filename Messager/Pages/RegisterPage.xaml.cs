using Messager.ViewModels;

namespace Messager.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel registerViewModel)
	{
		InitializeComponent();
		this.BindingContext = registerViewModel;
		
	}
}