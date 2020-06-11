using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Bonkles_Bot
{
    class Programm
    {
        private static TelegramBotClient bot;
        
        static void Main(string[] args)
        {

            string TOKEN = "TOKEN";
            
            var httpProxy = new WebProxy("http://51.158.111.242:8811")
            {
                UseDefaultCredentials = false,
            };

            bot = new TelegramBotClient(TOKEN, httpProxy);

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
            
            Console.ReadKey();
        }

        private static async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            
            string text = $"{e.Message.Date.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id}\n" +
                          $"{e.Message.Text}";

            Console.WriteLine($"{text}\nType: {e.Message.Type.ToString()}");

            if (e.Message.Text != null &&
                (e.Message.Text.ToLower().Contains("hi") || e.Message.Text.ToLower().Contains("hello") ||
                 e.Message.Text.ToLower().Contains("привет") || e.Message.Text.ToLower().Contains("/start")))
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, 
                    "Привет! Я могу скачивать файлы, которые ты мне отправляешь!");
            }

            if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/whatsup"))
            {
                await bot.SendTextMessageAsync(e.Message.Chat, "Все четко" + "😎");
            }

            if (e.Message.Text != null && e.Message.Text.ToLower().Contains("/gavkoshmyg"))
            {
                Console.WriteLine("yay");
                using (FileStream fileStream = File.OpenRead("gavkoshmyg.jpg"))
                {
                    var file = new InputOnlineFile(fileStream, "гавкошмыг");
                    await bot.SendPhotoAsync(e.Message.Chat, file);
                }
            }
            
            if (e.Message.Type == MessageType.Photo)
            {
                Console.WriteLine($"Count of sizes: {e.Message.Photo.Length.ToString()}");

                long max = 0;

                string fileId = "";
                string fileUniqueId = "";
                foreach (var photo in e.Message.Photo)
                {
                    if (photo.FileSize >= max)
                    {
                        max = photo.FileSize;
                        fileId = photo.FileId;
                        fileUniqueId = photo.FileUniqueId;
                    }
                }
                Download(fileId, fileUniqueId + ".jpg"); 
                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл загружен!");
            }
            else if (e.Message.Type == MessageType.Document)
            {
                Console.WriteLine($"File name: {e.Message.Document.FileName}");
                Console.WriteLine($"File size: {e.Message.Document.FileSize / 1000} KB");
                
                Download(e.Message.Document.FileId, e.Message.Document.FileName);
                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл загружен!");
            }
            else if (e.Message.Type == MessageType.Video)
            {
                Console.WriteLine($"Duration: {e.Message.Video.Duration} sec");
                Console.WriteLine($"Resolution: {e.Message.Video.Width} x {e.Message.Video.Height}");
                
                Download(e.Message.Video.FileId, e.Message.Video.FileId + ".mp4");
                await bot.SendTextMessageAsync(e.Message.Chat.Id, "Файл загружен!");
            }
        }

        
        private static async void Download(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);

            await using FileStream fileStream = new FileStream("_" + path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fileStream);
        }
        
    }
}