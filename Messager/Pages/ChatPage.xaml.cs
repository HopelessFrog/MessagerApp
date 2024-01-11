using CommunityToolkit.Maui.Animations;
using Messager.ViewModels;

namespace Messager.Pages;

public partial class ChatPage : ContentPage
{
    private ChatViewModel temp;
	public ChatPage(ChatViewModel viewModel)
	{
		InitializeComponent();
        temp = viewModel;
        this.BindingContext = viewModel;


    }
    private async void ContentPage_NavigatedTo(object? sender, NavigatedToEventArgs e)
    {
        var fadeAnimation = new FadeAnimation();
        fadeAnimation.Animate(this);

    }

    private void ChatPage_OnAppearing(object? sender, EventArgs e)
    {
        (this.BindingContext as ChatViewModel).Initialize().GetAwaiter().OnCompleted(async () =>
        {
            await Task.Delay(1000);
            messages.ScrollTo(temp.Messages.Count - 1, position: ScrollToPosition.End, animate: true);
        });
    }
}