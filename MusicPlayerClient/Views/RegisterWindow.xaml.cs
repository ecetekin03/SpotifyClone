using Microsoft.Extensions.DependencyInjection;
using MusicPlayerClient.ViewModels;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MusicPlayerClient
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameBox.Text.Trim();
            string lastName = LastNameBox.Text.Trim();
            string username = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Alanlar boşsa kullanıcıyı uyar
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await RegisterUserAsync(firstName, lastName, username, password, email);
        }


        private async Task RegisterUserAsync(string firstName, string lastName, string username, string password, string email)
        {
            string connectionString = @"Data Source=TEKIN-HP-HOME;Initial Catalog=SpotifyAppDB;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string query = "INSERT INTO Users (Username, Password, Email, FirstName, LastName) " +
                                   "VALUES (@Username, @Password, @Email, @FirstName, @LastName) ";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password); // Güvenlik için hash önerilir
                        cmd.Parameters.AddWithValue("@Email", email);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Kayıt başarıyla tamamlandı!");

                            var app = (App)Application.Current;
                            var mainViewModel = app.ServiceProvider.GetRequiredService<MainViewModel>();
                            var homeViewModel = app.ServiceProvider.GetRequiredService<HomeViewModel>();

                            mainViewModel.CurrentView = homeViewModel;

                            MainWindow mainWindow = new MainWindow
                            {
                                DataContext = mainViewModel
                            };

                            mainWindow.Show();

                            await mainViewModel.InitViewModel();

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Kayıt sırasında bir hata oluştu.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı bağlantısı sırasında bir hata oluştu: " + ex.Message);
                }
            }
        }

        // Pencere sürükleme
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Küçültme
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Kapatma
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
