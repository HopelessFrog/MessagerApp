using Messager.ViewModels;

namespace Messager.Pages;

public partial class ListChatPage : ContentPage
{
	public  ListChatPage(ListChatViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (this.BindingContext as ListChatViewModel).Initialize();
    }
}