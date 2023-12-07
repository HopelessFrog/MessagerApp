using Messager.Models;

namespace Messager
{
    public partial class App : Application
    {
        
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
