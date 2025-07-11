using CRINGEGRAM.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using System.Numerics;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;

namespace CRINGEGRAM
{
    public partial class Chat : Window
    {
        private Dictionary<string, CryptoModes> CryptoModes = new Dictionary<string, CryptoModes>();
        private Dictionary<string, BigInteger> GeneralKeys = new Dictionary<string, BigInteger>();
        private HubConnection Connect;
        private readonly string Token;
        private readonly string Path;
        private byte a,b,c = 0;

        public Chat(string Path, string Token)
        {
            this.Path = Path;
            this.Token = Token;
            InitializeComponent();
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            Connect = new HubConnectionBuilder().WithUrl("https://localhost:7039/Hubs/ChatHub",
            options => { options.AccessTokenProvider = () => Task.FromResult(Token);}).Build();

            try
            {
                await Connect.StartAsync();
            }
            catch
            {
                await Task.Delay(5000);
                InitializeSignalR();
            }

            Connect.On<string, Dictionary<string, CryptoModes>>("UserConnected", (Login, CryptoModes) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    SenderNameBox.Content = Login;
                    this.CryptoModes = CryptoModes;
                    OnlineStatus.Content = "ONLINE";

                    foreach (string User in CryptoModes.Keys)
                    {
                        CreateButtonFriend(User);
                    }

                    LoadGeneralKeys();
                }));
            });

            Connect.On<List<string>>("OnlineUsersList", (OnlineUsers) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    OnlineBox.Items.Clear();
                    OnlineUsers.Remove(SenderNameBox.Content.ToString());

                    foreach (var User in OnlineUsers)
                    {
                         OnlineBox.Items.Add(User);
                    }
                }));
            });

            Connect.On<byte[], byte[], string>("ChatRoomA", (PublicKey, P, UserB) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    GeneralKeys[UserB] = P_D_H.PowWithMod(new BigInteger(PublicKey), GeneralKeys[UserB], new BigInteger(P));
                    SaveGeneralKeys();
                }));
            });

            Connect.On<CryptoData, CryptoModes, string>("ChatRoomB", async (CryptoData, CryptoModes, Sender) =>
            {
                await Dispatcher.Invoke(async () =>
                {
                    P_D_H P_D_H = new P_D_H(CryptoData);
                    
                    if (await Connect.InvokeAsync<bool>("AnswerChatRoom", P_D_H.GetPublicKey(), CryptoData.P, SenderNameBox.Content.ToString(), Sender))
                    {
                        this.GeneralKeys.Add(Sender, P_D_H.GetGeneralKey());
                        this.CryptoModes.Add(Sender, CryptoModes);
                        CreateButtonFriend(Sender);
                        SaveGeneralKeys();
                    }
                });
            });

            Connect.On<int, string>("RemoveItem", (Index, User) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    RemoveItem(Index, User);
                }));
            });

            Connect.On<string>("CloseChat", (User) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    CloseChat(User);
                }));
            });

            Connect.On<List<byte[]>, string>("ReceiveMessage", (Message, Sender) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageCrypt MessageCrypt = new MessageCrypt(CryptoModes[Sender], Message, GeneralKeys[Sender]);

                    var MessageData = new MessageData
                    {
                        DisTime = $"{DateTime.Now:HH:mm:ss}",
                        Sender = Sender,
                        Message = MessageCrypt.GetPlainText()
                    };

                    SaveChat(MessageData);

                    if (RecipientNameBox.Content.ToString() == Sender)
                    {
                        AddMessage(MessageData);
                        Z.ScrollIntoView(Z.Items[Z.Items.Count - 1]);
                    }
                    else
                    {
                        var Button = PanelOfButtons.Children.Cast<Button>().FirstOrDefault(zxc => zxc.Content.ToString() == Sender);
                        Button.Background = new SolidColorBrush(Color.FromRgb(135, 116, 225));
                    }
                }));
            });
        }

        private string GetPath()
        {
            return Path + SenderNameBox.Content.ToString() + "\\" ;
        }

        private async void SendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (RecipientNameBox.Content.ToString() != "" && MessageBox.Text != "")
                {
                    if (CryptoModes != null)
                    {
                        MessageCrypt MessageCrypt = new MessageCrypt(CryptoModes[RecipientNameBox.Content.ToString()], MessageBox.Text, GeneralKeys[RecipientNameBox.Content.ToString()]);

                        if (await Connect.InvokeAsync<bool>("SendMessage", MessageCrypt.GetClosedText(), SenderNameBox.Content.ToString(), RecipientNameBox.Content.ToString()))
                        {
                            var MessageData = new MessageData
                            {
                                DisTime = $"{DateTime.Now:HH:mm:ss}",
                                Sender = SenderNameBox.Content.ToString(),
                                Message = MessageBox.Text
                            };

                            MessageBox.Clear();
                            SaveChat(MessageData);
                            AddMessage(MessageData);
                            Z.ScrollIntoView(Z.Items[Z.Items.Count - 1]);
                        }
                        else
                        {
                            string Recipient = RecipientNameBox.Content.ToString();
                            RecipientNameBox.Content = "Пользователь не в сети";
                            await Task.Delay(1000);
                            RecipientNameBox.Content = Recipient;
                        }
                    }
                }
            }
            catch
            {
                RecipientNameBox.Content = "ЧТО-ТО СЛОМАЛОСЬ";
            }
        }

        private async void StartChat_Click(object sender, EventArgs e)
        {
            if (OnlineBox.SelectedItem != null)
            {
                string Recipient = OnlineBox.SelectedItem.ToString();

                if (PanelOfButtons.Children.Count > 0)
                {
                    foreach (Control Control in PanelOfButtons.Children)
                    {
                        if (Control is Button Button && Button.Content.ToString() == Recipient)
                        {
                            NewButton_Click(Button, null);
                            return;
                        }
                    }
                }

                P_D_H P_D_H = new P_D_H();
                var CryptoModes = new CryptoModes
                {
                    AlgMode = SwitchAlgMode.Content.ToString(),
                    StuffMode = SwitchStuffMode.Content.ToString(),
                    EncryptMode = SwitchEncryptMode.Content.ToString()
                };

                if (await Connect.InvokeAsync<bool>("CreateChatRoom", P_D_H.GetCryptoData(), CryptoModes, SenderNameBox.Content.ToString(), Recipient))
                {
                    this.CryptoModes.Add(Recipient, CryptoModes);
                    this.GeneralKeys.Add(Recipient, P_D_H.GetSecretKey());
                }

                Z.Items.Clear();
                CreateButtonFriend(Recipient);
                RecipientNameBox.Content = Recipient;
            }
        }

        private async void CloseChat_Click(object sender, RoutedEventArgs e)
        {
            if (RecipientNameBox.Content.ToString() != "")
            {
                if (await Connect.InvokeAsync<bool>("CloseChat", SenderNameBox.Content.ToString(), RecipientNameBox.Content.ToString()))
                {
                    CloseChat(RecipientNameBox.Content.ToString());
                }
            }
        }

        private void CloseChat(string User)
        {
            Z.Items.Clear();

            foreach (Control Control in PanelOfButtons.Children)
            {
                if (Control is Button Button && Button.Content.ToString() == User)
                {
                    string PathChat = GetPath() + User + ".json";

                    if (File.Exists(PathChat))
                    { 
                        File.Delete(PathChat);
                    }

                    PanelOfButtons.Children.Remove(Control);
                    RecipientNameBox.Content = "";
                    break;
                }
            }
            GeneralKeys.Remove(User);
            CryptoModes.Remove(User);
            SaveGeneralKeys();
        }

        private void LoadGeneralKeys()
        {
            string PathChat = Path + "Data\\Keys_" + SenderNameBox.Content.ToString() + ".json";

            if (File.Exists(PathChat))
            {
                GeneralKeys = JsonConvert.DeserializeObject<Dictionary<string, BigInteger>>(File.ReadAllText(PathChat));
            }
        }

        private void SaveGeneralKeys()
        {
            File.WriteAllText(Path + "Data\\Keys_" + SenderNameBox.Content.ToString() + ".json", JsonConvert.SerializeObject(GeneralKeys, Formatting.Indented));
        }

        private void LoadChat()
        {
            string PathChat = GetPath() + RecipientNameBox.Content.ToString() + ".json";

            if (File.Exists(PathChat))
            {
                string Doc = File.ReadAllText(PathChat);
                var Chat = JsonConvert.DeserializeObject<List<MessageData>>(Doc);
                Z.Items.Clear();

                foreach (var Message in Chat)
                {
                    AddMessage(Message);
                }
            }
        }

        private void SaveChat(MessageData MessageData)
        { 
            string ChatPath = GetPath() + MessageData.Sender + ".json";
            List<MessageData> LoadData = new List<MessageData>();

            if (MessageData.Sender == SenderNameBox.Content.ToString())
            {
                ChatPath = Path + SenderNameBox.Content.ToString() + "\\" + RecipientNameBox.Content.ToString() + ".json";
            }

            if (!Directory.Exists(Path + SenderNameBox.Content.ToString()))
            {
                Directory.CreateDirectory(Path + SenderNameBox.Content.ToString());
            }

            if (File.Exists(ChatPath))
            {
                string Doc = File.ReadAllText(ChatPath);
                LoadData = JsonConvert.DeserializeObject<List<MessageData>>(Doc);
            }

            LoadData.Add(MessageData);
            string json = JsonConvert.SerializeObject(LoadData, Formatting.Indented);
            File.WriteAllText(ChatPath, json);
        }

        private void CreateButtonFriend(string name)
        {
            Button Button = new Button
            {
                Height = 50,
                Width = 200,
                FontSize = 20,
                Content = name,
                Margin = new Thickness(0, 10, 0, 0),
                Foreground = new SolidColorBrush(Colors.White),
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Color.FromRgb(23, 23, 23)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(135, 116, 225)),
            };

            Button.Click += NewButton_Click;
            PanelOfButtons.Children.Add(Button);
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            try
            {
                Button Button = sender as Button;
                RecipientNameBox.Content = Button.Content;
                Button.Background = new SolidColorBrush(Color.FromRgb(23, 23, 23));
                Z.Items.Clear();
                LoadChat();

                if (Z.Items.Count > 0)
                {
                    Z.ScrollIntoView(Z.Items[Z.Items.Count - 1]);
                }
            }
            catch
            {
                RecipientNameBox.Content = "ЧТО-ТО СЛОМАЛОСЬ";
            }
        }

        private void AddMessage(MessageData MessageData)
        {
            Z.Items.Add(new ComboData { DisTimeSender = MessageData.Sender + '\n' + MessageData.DisTime, Message = MessageData.Message });
        }

        private void SwitchAlgMode_Click(object sender, RoutedEventArgs e)
        {
            a++;

            switch (a % 2)
            {
                case 0:
                    SwitchAlgMode.Content = "MARS";
                    break;

                case 1:
                    SwitchAlgMode.Content = "MacGuffin";
                    break;
            }
        }

        private void SwitchEncryptMode_Click(object sender, RoutedEventArgs e)
        {
            b++;

            switch (b % 7)
            {
                case 0:
                    SwitchEncryptMode.Content = "ECB";
                    break;

                case 1:
                    SwitchEncryptMode.Content = "CBC";
                    break;
                case 2:

                    SwitchEncryptMode.Content = "PCBC";
                    break;
                case 3:

                    SwitchEncryptMode.Content = "CFB";
                    break;
                case 4:

                    SwitchEncryptMode.Content = "OFB";
                    break;
                case 5:

                    SwitchEncryptMode.Content = "CTR";
                    break;

                case 6:
                    SwitchEncryptMode.Content = "RD";
                    break;
            }
        }

        private void SwitchStuffMode_Click(object sender, RoutedEventArgs e)
        {
            c++;

            switch (c % 4)
            {
                case 0:
                    SwitchStuffMode.Content = "Zeros";
                    break;

                case 1:
                    SwitchStuffMode.Content = "ANSIX.923";
                    break;

                case 2:
                    SwitchStuffMode.Content = "PKCS7";
                    break;

                case 3:
                    SwitchStuffMode.Content = "ISO10126";
                    break;
            }
        }

        private async void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            int Index = Z.SelectedIndex;

            if (await Connect.InvokeAsync<bool>("RemoveItem", Index, SenderNameBox.Content.ToString(), RecipientNameBox.Content.ToString()))
            {
                RemoveItem(Index, RecipientNameBox.Content.ToString());
            }
        }

        private void RemoveItem(int Index, string User)
        {
            if (RecipientNameBox.Content.ToString() == User || SenderNameBox.Content.ToString() == User)
            {
                Z.Items.RemoveAt(Index);
            }

            string ChatPath = GetPath() + User + ".json";
            string Doc = File.ReadAllText(ChatPath);

            var LoadData = JsonConvert.DeserializeObject<List<MessageData>>(Doc);
            LoadData.RemoveAt(Index);

            string json = JsonConvert.SerializeObject(LoadData, Formatting.Indented);
            File.WriteAllText(ChatPath, json);
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(Path + "Data\\Token.json");
            Application.Current.Shutdown();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
