using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using ZOV.Models;

namespace ZOV.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, ChatRoom> ChatRooms = new ConcurrentDictionary<string, ChatRoom>();

        public override async Task OnConnectedAsync()
        {
            var Login = Context?.User?.Claims.FirstOrDefault(ZOV => ZOV.Type == "Login")?.Value;

            if (Login != null)
            {
                if (ConnectedUsers.TryAdd(Context!.ConnectionId, Login))
                {
                    await Task.Delay(100);
                    await Clients.Caller.SendAsync("UserConnected", Login, GetRooms(Login));
                    Console.Write($"ONLINE {ConnectedUsers.Count}\tUSER LOGIN  :\t{Context.ConnectionId}   <-->   {Login}\n");
                    SendOnlineUsersList();
                }
            }
            else
            {
                Console.Write($"\nОшибка подключения\n");
                return;
            }

            await base.OnConnectedAsync();
        }

        private Dictionary<string, CryptoModes> GetRooms(string User)
        {
            Dictionary<string, CryptoModes> SettingRooms = new Dictionary<string, CryptoModes>();

            var Rooms = ChatRooms.Values.Where(ChatRoom => (ChatRoom.UserA == User) || (ChatRoom.UserB == User)).ToList();

            foreach (var Room in Rooms)
            {
                var CryptoModes = new CryptoModes
                {
                    AlgMode = Room.AlgMode,
                    StuffMode = Room.StuffMode,
                    EncryptMode = Room.EncryptMode,
                };

                if (User == Room.UserA)
                {
                    SettingRooms.Add(Room.UserB!, CryptoModes);
                }
                else if (User == Room.UserB)
                {
                    SettingRooms.Add(Room.UserA!, CryptoModes);
                }
            }

            return SettingRooms;
        }

        private async void SendOnlineUsersList()
        {
            await Clients.All.SendAsync("OnlineUsersList", ConnectedUsers.Values.ToList());
        }

        public async Task<bool> CreateChatRoom(CryptoData CryptoData, CryptoModes CryptoModes, string Sender, string Recipient)
        {
            if (!ChatRooms.Values.Any(ZOV => (ZOV.UserA == Sender && ZOV.UserB == Recipient) || (ZOV.UserB == Sender && ZOV.UserA == Recipient)))
            {
                var ChatRoom = new ChatRoom
                {
                    UserA = Sender,
                    UserB = Recipient,
                    AlgMode = CryptoModes.AlgMode,
                    StuffMode = CryptoModes.StuffMode,
                    EncryptMode = CryptoModes.EncryptMode
                };

                var RecipientID = ConnectedUsers.FirstOrDefault(ZOV => ZOV.Value == Recipient).Key;

                if (RecipientID != null)
                {
                    string RoomID = Context.ConnectionId;

                    if (ChatRooms.TryAdd(RoomID, ChatRoom))
                    {
                        Console.Write($"\nКомната {RoomID} создана\n");
                        await Clients.Client(RecipientID).SendAsync("ChatRoomB", CryptoData, CryptoModes, Sender);
                        return true;
                    }
                }
            }
            else
            {
                Console.Write("\nКомната уже существует\n");
            }

            Console.Write("\nНе удалось создать комнату\n");
            return false; 
        }

        public async Task<bool> AnswerChatRoom(byte[] PublicKey, byte[] P, string Sender, string Recipient)
        {
            var RecipientID = ConnectedUsers.FirstOrDefault(ZOV => ZOV.Value == Recipient).Key;

            if (RecipientID != null)
            {
                try
                {
                    await Clients.Client(RecipientID).SendAsync("ChatRoomA", PublicKey, P, Sender);
                    return true;
                }
                catch { }
            }

            return false;
        }

        public async Task<bool> RemoveItem(int Index, string Sender, string Recipient)
        {
            var RecipientID = ConnectedUsers.FirstOrDefault(ZOV => ZOV.Value == Recipient).Key;

            if (RecipientID != null)
            {
                try
                {
                    await Clients.Client(RecipientID).SendAsync("RemoveItem", Index, Sender);
                    return true;
                }
                catch { }
            }

            return false;
        }

        public async Task<bool> CloseChat(string Sender, string Recipient)
        {
            var RecipientID = ConnectedUsers.FirstOrDefault(ZOV => ZOV.Value == Recipient).Key;

            if (RecipientID != null)
            {
                try
                {
                    await Clients.Client(RecipientID).SendAsync("CloseChat", Sender);

                    foreach (var Room in ChatRooms)
                    {
                        if ((Room.Value.UserA == Sender && Room.Value.UserB == Recipient) || (Room.Value.UserA == Recipient && Room.Value.UserB == Sender))
                        {
                            ChatRooms.TryRemove(Room.Key, out _);
                            Console.Write($"\nКомната {Room.Key} удалена\n");
                            break;
                        }
                    }

                    return true;
                }
                catch { }
            }

            return false;
        }

        public async Task<bool> SendMessage(List<byte[]> Message, string Sender, string Recipient)
        {
            if (Message[0].Count() != 0)
            {
                var RecipientID = ConnectedUsers.FirstOrDefault(ZOV => ZOV.Value == Recipient).Key;

                if (RecipientID != null)
                { 
                    await Clients.Client(ConnectedUsers.FirstOrDefault(ZOV => ZOV.Value == Recipient).Key).SendAsync("ReceiveMessage", Message, Sender);
                    
                    Console.Write($"\n\t{Sender}\t-->\t{Recipient}\n");

                    foreach (byte[] ZOV in Message)
                    {
                        Console.Write(new BigInteger(ZOV) + "\n");
                    }

                    return true;
                }
            }

            Console.Write("\nНе удалось отправить сообщение\n");
            return false;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var Login = Context?.User?.Claims.FirstOrDefault(ZOV => ZOV.Type == "Login")?.Value;

            if (Login != null)
            {
                if (ConnectedUsers.TryRemove(Context!.ConnectionId, out _))
                {
                    Console.Write($"ONLINE {ConnectedUsers.Count}\tUSER LOGOUT :\t{Context.ConnectionId}   <-->   {Login}\n");
                    SendOnlineUsersList();
                }
            }
            else
            {
                Console.Write($"\nОшибка отключения\n");
                return;
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}