using MusicPlayerClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicPlayerClient
{
    public partial class LoginWindow : Window
    {
        private readonly string connectionString = @"Data Source=TEKIN-HP-HOME;Initial Catalog=SpotifyAppDB;Integrated Security=True";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (await IsLoginValid(username, password))
            {
                var app = (App)Application.Current;
                var mainViewModel = app.ServiceProvider.GetRequiredService<MainViewModel>();
                var homeViewModel = app.ServiceProvider.GetRequiredService<HomeViewModel>();

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

        private async Task<bool> IsLoginValid(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password); // Şifreleme gerekiyorsa burada hash uygulanmalı

                    await conn.OpenAsync();
                    int result = (int)await cmd.ExecuteScalarAsync();
                    return result > 0;
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close(); // veya Hide() tercih edilebilir
        }

        private void UsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Gerekirse kullanıcı adı için canlı validasyon
        }

        // Sürükleme özelliği
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // Pencereyi kapat
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Pencereyi küçült
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
