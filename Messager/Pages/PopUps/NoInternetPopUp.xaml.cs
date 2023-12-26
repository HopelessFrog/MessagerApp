using CommunityToolkit.Maui.Views;

namespace Messager.Pages.PopUps;

public partial class NoInternetPopUp : Popup
{
	public NoInternetPopUp()
	{
		InitializeComponent();
	}

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        this.Close();
    }
}