using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messager.Helpers;

namespace Messager.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        
        public string Ip
        {
            get
            {
                return DevHttpsConnectionHelper.DevServerName;
            }
            set
            {
                if (value != DevHttpsConnectionHelper.DevServerName)
                {
                    DevHttpsConnectionHelper.DevServerName = value;
                    OnPropertyChanged(nameof(Ip));
                }
            }
        }
    }
}
