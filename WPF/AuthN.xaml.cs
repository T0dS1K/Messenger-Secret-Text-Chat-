using System;
using System.IO;
using System.Windows;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace CRINGEGRAM
{
    public partial class AuthN : Window
    {
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("https://localhost:7039/") };
        private readonly string Path = "D:\\CRINGEGRAM\\";
        private readonly string TokenPath = "Data\\Token.json";

        public AuthN()
        {
            InitializeComponent();
            ReAuthN();
        }

        private async void ReAuthN()
        {
            var Token = await ReadTokenFromFileAsync(Path + TokenPath);

            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var Response = await client.GetAsync("ReAuthN");

                if (Response.IsSuccessStatusCode)
                {
                    ChatShow(Path, Token);
                }
            }
            catch
            {
                ReportBox.Content = "СОЕДИНЕНИЕ С СЕРВЕРОМ НЕ УСТАНОВЛЕНО";
            }
        }

        private static async Task<string> ReadTokenFromFileAsync(string TokenPath)
        {
            try
            {
                using (JsonDocument Doc = JsonDocument.Parse(await File.ReadAllTextAsync(TokenPath)))

                if (Doc.RootElement.TryGetProperty("token", out JsonElement TokenElement))
                {
                    return TokenElement.GetString();
                }
            }
            catch {}

            return null;
        }

        private async void SingIn_Click(object sender, EventArgs e)
        {
            ReportBox.Content = "";

            if (UserLoginBox.Text.Length > 2 && UserPassBox.Text.Length > 2)
            {
                try
                {
                    var UserData = new { Login = UserLoginBox.Text, Password = UserPassBox.Text };
                    var Response = await client.PostAsJsonAsync("AuthN/SignIn", UserData);
                    var Token = await Response.Content.ReadAsStringAsync();

                    if (Response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                    {
                        if (!Directory.Exists(Path + "Data" + "\\"))
                        {
                            Directory.CreateDirectory(Path + "Data" + "\\");
                        }

                        File.WriteAllText(Path + TokenPath, Token);
                        ReAuthN();
                    }
                    else
                    {
                        ReportBox.Content = "НЕВЕРНЫЙ ЛОГИН ИЛИ ПАРОЛЬ";
                    }
                }
                catch
                {
                    ReportBox.Content = "СОЕДИНЕНИЕ НЕ УСТАНОВЛЕНО";
                }
            }
            else
            {
                ReportBox.Content = "СЛИШКОМ КОРОТКИЙ ЛОГИН ИЛИ ПАРОЛЬ";
            }
        }

        private async void RegIn_Click(object sender, EventArgs e)
        {
            ReportBox.Content = "";

            if (UserLoginBox.Text.Length > 2 && UserPassBox.Text.Length > 2)
            {
                try
                {
                    var NewUser = new { Login = UserLoginBox.Text, Password = UserPassBox.Text };
                    var Response = await client.PostAsJsonAsync("AuthN/RegIn", NewUser);
                    ReportBox.Content = await Response.Content.ReadAsStringAsync();
                }
                catch
                {
                    ReportBox.Content = "СОЕДИНЕНИЕ НЕ УСТАНОВЛЕНО";
                }
            }
            else
            {
                ReportBox.Content = "СЛИШКОМ КОРОТКИЙ ЛОГИН ИЛИ ПАРОЛЬ";
            }
        }

        private void ChatShow(string DirectoryPath, string Token)
        {
            this.Hide();
            Chat Chat = new Chat(DirectoryPath, Token);
            Chat.ShowDialog();
        }

        private void AuthN_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}