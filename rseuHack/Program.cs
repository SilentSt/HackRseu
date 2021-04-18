using FastReport.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO.Compression;
using Newtonsoft.Json.Linq;

namespace rseuHack
{
    class Program
    {
        public enum MenuItems
        {
            Start,
            Help,
            Status
        }
        static List<string> files = new List<string>();
        public const long ADMIN_ID = 729098950;
        public static Dictionary<string, MenuItems> menu = new Dictionary<string, MenuItems>();
        public static ITelegramBotClient tgBot;
        private static ReplyKeyboardMarkup keyboard;
        private static KeyboardButton[] oneRowButtons;
        private static KeyboardButton[][] buttons;
        public static Dictionary<long?, User> users = new Dictionary<long?, User>();
        private static HttpClient httpClient;
        private static Authorization auth;
        private static FileVM fileVM;
        public static Document document;
        public static string filename;
        public static QueueController queue = QueueController.GetQueueController();
        
        private static string token = System.IO.File.ReadAllText("spt");
        static void Main(string[] args)
        {
            menu.Add("/start", MenuItems.Start);
            menu.Add("/help", MenuItems.Help);
            menu.Add("/status", MenuItems.Status);
            if (System.IO.File.Exists("fls"))
            {
                var lst = System.IO.File.ReadAllText("fls");
                files = JObject.Parse(lst)["files"].ToObject<List<string>>();
            }
            tgBot = new TelegramBotClient(token);
            var me = tgBot.GetMeAsync().Result;
            tgBot.OnMessage += OnNewMessage;
            tgBot.StartReceiving();
            auth = Authorization.GetAuthorization();
            //await auth.GetTemplatesRoot(httpClient);
            //httpClient = auth.AuthAsync();
            tgBot.OnCallbackQuery += OnCallback;
            Console.ReadKey();
        }

        public async static Task GetFile(long mId, string fileid)
        {
            if (string.IsNullOrWhiteSpace(fileid))
                return;
            var file = tgBot.GetFileAsync(fileid).Result;
            filename = "file." + file.FilePath.Split(".").Last();
            var url = "https://api.telegram.org/file/" + "bot" + token + "/" + file.FilePath;
            byte[] data;
            using (var client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                data = await content.ReadAsByteArrayAsync();
                using (FileStream f = System.IO.File.Create(filename)) ////path = "wwwroot\\XML\\1.zip"
                    f.Write(data, 0, data.Length);
            }
        }

        private async static void OnCallback(object sender, CallbackQueryEventArgs e)
        {
            var mId = e.CallbackQuery.Message.Chat.Id;
            if (!users.ContainsKey(mId))
            {
                users.Add(mId, new User());
            }
            
            var type = e.CallbackQuery.Data.Split(":")[0];

            var fileid = files[int.Parse(e.CallbackQuery.Data.Split(":")[1])];
            queue.Add(mId, type, fileid);
            
        }

        private static async void OnNewMessage(object sender, MessageEventArgs e)
        {            
            long userID = (long)e?.Message?.Chat.Id;
            if(!users.ContainsKey(userID))
            {
                users.Add(userID, new User());
            }
            string mes = e?.Message?.Text;
            document = e?.Message?.Document;
            //queue, and killer ficha


            if (mes != null)
            {
                if (!menu.ContainsKey(mes))
                {
                    SendMessage(Menu.menuButtons, userID, "такой команды не существует", false);
                    return;
                }
                
                if (!users.ContainsKey(userID))
                {
                    users.Add(userID, new User() { menuItems = Program.MenuItems.Start });
                }
                users[userID].lastMessage = DateTime.Now;
                if (menu.ContainsKey(mes))
                {
                    users[userID].menuItems = menu[mes];
                }

                users[userID].userName = e?.Message?.Chat?.FirstName + " " + e?.Message?.Chat?.LastName;

                switch (users[userID].menuItems)
                {
                    case MenuItems.Start:
                        SendMessage(Menu.menuButtons, userID,
                            "Данный бот принимает файлы FRX и FPX форматов, затем преобразует их в один из возможных форматов через сервис Fast Report"+
                            " и отправляет пользователю.\n"+
                             "Бот разработан командой Silent, под руководсвом 'fast report ' https://fastreport.cloud/ru/", false);
                        break;
                    case MenuItems.Help:
                        SendMessage(Menu.menuButtons, userID, "Вам нужно просто отправить один или несколько файлов боту, "+
                             "затем для каждого из файлов вам будет предложено выбрать формат, который нужен вам." +
                             "Для каждого недавнего файла в истории сообщений с ботом вы можете преобразовывать файл неограниченное число раз, "+
                            "а также в несколько форматов.\n"+
                            "Все доступные команды:\n" +
                            "/help - расскажет о том, как пользоваться ботом\n" +
                            "/start - краткая информация о боте\n" +
                            "/status - позволяет получить информацию о месте ваших файло в очереди "
                            , false);
                        break;
                    case MenuItems.Status:
                        var que = queue.GetQueues(userID);
                        if (que==null||que.Length==0)
                        {
                            SendMessage(Menu.menuButtons, userID, "Ваших файлов нет в очереди", false);
                            return;
                        }
                        string str = "Ваши файлы в очереди под номерами:";
                        for (int i = 0; i < que.Length; i++)
                        {
                            str += "\n\t"+que[i];
                        }
                        SendMessage(Menu.menuButtons, userID, str, false);
                        break;
                }
            }
            if (document != null&&document.MimeType == "application/xml")
            {
                users[userID].lastFile.Add(document.FileId);
                //Console.WriteLine(document.MimeType);
                //CreateInlineKeyboard(Menu.inlineMenu[0], userID);
                var buttons = Menu.inlineMenu.ToList().Select(x => {
                    var tmp = x + ":" + files.Count;
                    return tmp;
                    }).ToArray();
                files.Add(e.Message.Document.FileId);
                JObject lst = new JObject();
                lst["files"] = JToken.FromObject(files);
                string flst = lst.ToString();
                if (System.IO.File.Exists("fls"))
                {
                    System.IO.File.Delete("fls");
                }
                System.IO.File.WriteAllText("fls", flst);
                Console.WriteLine(buttons[0].Length);
                SendMessage(CreateInlineKeyboard(Menu.inlineMenu, buttons, userID), userID, "Выберете нужный тип файла " + e.Message.Document.FileName);
               
            }      
        }


        public static void SendMessage(InlineKeyboardMarkup keyboardMarkup,long? userId,string text)
        {
            tgBot.SendTextMessageAsync(
                replyMarkup:keyboardMarkup,
                chatId: userId,
                text:text
                ).ConfigureAwait(false);
        }
        public static void SendMessage(long? userId, string text)
        {
            tgBot.SendTextMessageAsync(
                chatId: userId,
                text: text
            );
        }
        public static void SendMessage(string[][] keyboard, long? userId, string text, bool oneTimeKeyboard)
        {
            tgBot.SendTextMessageAsync(
            chatId: userId,
            text: text,
            //replyMarkup: CreateKeyboard(keyboard, keyCallBacks),
            replyMarkup: CreateKeyboard(keyboard, oneTimeKeyboard)
            ).ConfigureAwait(false);
        }

        public static void SendMessage(string[] keyboard, long? userId, string text, bool oneTimeKeyboard)
        {
            tgBot.SendTextMessageAsync(
                chatId: userId,
                text: text,
                //replyMarkup: CreateKeyboard(keyboard, keyCallBacks),
                replyMarkup: CreateKeyboard(keyboard, oneTimeKeyboard)
            ).ConfigureAwait(false);
        }
        
        private static InlineKeyboardMarkup CreateInlineKeyboard(string []keyboard, string[] callback, long? userId)
        {
            //InlineKeyboardMarkup key = new InlineKeyboardMarkup();
            InlineKeyboardMarkup key = null;
            InlineKeyboardButton[][] keyboardButtons = new InlineKeyboardButton[keyboard.Length/4][];            
            for (int i = 0; i < keyboard.Length;)
            {
                keyboardButtons[i/4] = new InlineKeyboardButton[4];
                for (int j = 0; j < 4; j++)
                {
                    keyboardButtons[i/4][j] = new InlineKeyboardButton { Text = keyboard[i], CallbackData = callback[i] };
                    i++;
                }                
            }            
            key = new InlineKeyboardMarkup(keyboardButtons);                            
           
            return key;
        }


        private static ReplyKeyboardMarkup CreateKeyboard(string[][] buttonNames, bool oneTimeKeyboard)
        {
            buttons = new KeyboardButton[buttonNames.Length][];
            for (int i = 0; i < buttonNames.Length; i++)
            {
                buttons[i] = new KeyboardButton[buttonNames[i].Length];
                for (int j = 0; j < buttonNames[i].Length; j++)
                {
                    buttons[i][j] = new KeyboardButton(buttonNames[i][j]);
                }
            }
            keyboard = new ReplyKeyboardMarkup(buttons, true, oneTimeKeyboard);
            return keyboard;
        }

        private static ReplyKeyboardMarkup CreateKeyboard(string[] buttonNames, bool oneTimeKeyboard)
        {
            oneRowButtons = new KeyboardButton[buttonNames.Length];
            for (int i = 0; i < buttonNames.Length; i++)
            {
                oneRowButtons[i] = new KeyboardButton(buttonNames[i]);
            }
            keyboard = new ReplyKeyboardMarkup(oneRowButtons, true, oneTimeKeyboard);
            return keyboard;
        }

    }
}
