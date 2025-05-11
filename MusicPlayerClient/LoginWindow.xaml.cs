using MusicPlayerClient.ViewModels;
using Microsoft.Extensions.DependencyInjection; // EKLEMEYİ UNUTMA
using System;
using System.Windows;
using System.Windows.Controls;

namespace MusicPlayerClient
{
    /// <summary>
    /// LoginWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameBox.Text == "zeynep" && PasswordBox.Password == "1234")
            {
                // ServiceProvider’dan gerekli ViewModel’leri al
                var app = (App)Application.Current;
                var mainViewModel = app.ServiceProvider.GetRequiredService<MainViewModel>();
                var homeViewModel = app.ServiceProvider.GetRequiredService<HomeViewModel>();

                // Giriş sonrası ana ekran HomeView olacak
                mainViewModel.CurrentView = homeViewModel;

                var mainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };

                mainWindow.Show();
                await mainViewModel.InitViewModel();
                this.Close();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
