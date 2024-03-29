﻿using CommunityToolkit.Maui;
using Material.Components.Maui.Extensions;
using Messager.Pages;
using Messager.Services.ChatHub;
using Messager.ViewModels;
using Microsoft.Extensions.Logging;
using WinRT;
using ServiceProvider = Messager.Services.ServiceProvider;

namespace Messager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMaterialComponents()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("OpenSans-Regular.ttf", "IconFontTypes");
                });
            builder.UseMaterialComponents();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<ChatHub>();
            builder.Services.AddSingleton<ListChatPage>();
            builder.Services.AddSingleton<ListChatViewModel>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<ChatPage>();
            builder.Services.AddSingleton<ChatViewModel>();
            builder.Services.AddSingleton<ServiceProvider>();
            builder.Services.AddSingleton<Settings>();
            builder.Services.AddSingleton<SettingsViewModel>();








#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
