using Messager.Pages;

namespace Messager
{
    public partial class AppShell : Shell
    {
        public AppShell(LoginPage loginPage)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage)+"/" + nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(LoginPage) + "/" + nameof(Settings), typeof(Settings));

            Routing.RegisterRoute("ListChatPage", typeof(ListChatPage));
            Routing.RegisterRoute("ChatPage", typeof(ChatPage));

            this.CurrentItem = loginPage;
        }
    }
}
