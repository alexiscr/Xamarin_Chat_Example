using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheChat.Core.Helpers;
using TheChat.Core.Services;
using TheChatMessages;

namespace TheChat.Client
{
    class Program
    {
        static ChatService service;
        static string userName;
        static string room;
        static async Task Main(string[] args)
        {
            Console.WriteLine("User Name:");
            userName = Console.ReadLine();

            service = new ChatService();
            service.OnReceivedMessage += Service_OnReceivedMessage;

            await service.InitAsync(userName);

            Console.WriteLine("You are now connected");

            await JoinRoom();

            var keepGoing = true;

            do
            {
                var text = Console.ReadLine();
                if (text == "exit")
                {
                    await service.DisconnectAsyn();
                    keepGoing = false;
                }
                else if (text == "leave")
                {
                    var message = new UserConnectedMessage(userName, room);
                    await service.LeaveChannelAsync(message);
                    await JoinRoom();
                }
                else if (text == "private")
                {
                    Console.WriteLine("Enter UserName");
                    var user = Console.ReadLine();

                    Console.WriteLine("Enter private message: ");
                    text = Console.ReadLine();

                    var message = new SimpleTextMessage(userName)
                    {
                        Text = text,
                        Recipient = user
                    };

                    await service.SendMessageAsync(message);
                }
                else if (text == "image") {
                    var imagePath = @"G:\CursoXamarin\temp\celica_icon.png";
                    var imagesStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    // Convertimos a byte
                    var bytes = ImageHelper.ReadFully(imagesStream);
                    var base64Photo = Convert.ToBase64String(bytes);
                    var message = new PhotoMessage(userName)
                    {
                        Base64Photo = base64Photo,
                        FileEnding = imagePath.Split('.').Last(),
                        GroupName = room
                    };
                    await service.SendMessageAsync(message);
                }
                else
                {
                    var message = new SimpleTextMessage(userName)
                    {
                        Text = text,
                        GroupName = room
                    };

                    await service.SendMessageAsync(message);
                }
            } while (keepGoing);
        }

        private async static Task JoinRoom()
        {
            var rooms = await service.GetRooms();
            foreach (var room in rooms)
            {
                Console.WriteLine(room.Name);                
            }

            room = Console.ReadLine();
            var message = new UserConnectedMessage(userName, room);

            await service.JoinChannelAsync(message);

            var usersInRoom = await service.GetUsersGroup(room);
            Console.WriteLine($"There are currently {usersInRoom.Count} users in the room");
        }

        private static void Service_OnReceivedMessage(object sender, Core.EventHandlers.MessageEventArgs e)
        {
            if (e.Message.Sender == userName)
                return;
            if (e.Message.TypeInfo.Name == nameof(SimpleTextMessage))
            {
                var simpleText =
                    e.Message as SimpleTextMessage;
                var message = $"{simpleText.Sender}: {simpleText.Text}";
                Console.WriteLine(message);
            }
            else if (e.Message.TypeInfo.Name == nameof(UserConnectedMessage))
            {
                var userConnected = e.Message as UserConnectedMessage;
                string message = string.Empty;
                if (userConnected.IsEntering)
                {
                    message = $"{userConnected.Sender} has connected";
                }
                else
                {
                    message = $"{userConnected.Sender} has left";
                }
                Console.WriteLine(message);
            }
            else if (e.Message.TypeInfo.Name == nameof(PhotoUrlMessage)) {
                var photoMessage = e.Message as PhotoUrlMessage;
                string message = $"{ photoMessage.Sender} sent {photoMessage.Url}";
                Console.WriteLine(message);
            }
        }
    }
}
