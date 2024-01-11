using Messager.ViewModels;

namespace Messager.Pages;

public partial class ListChatPage : ContentPage
{
	public  ListChatPage(ListChatViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;
    }
    protected override bool OnBackButtonPressed()
    {
        return true;
    }
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        var qwe = 123;
    }

    private void ListChatPage_OnAppearing(object? sender, EventArgs e)
    {
        (this.BindingContext as ListChatViewModel).Initialize();
    }
}