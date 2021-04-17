using FastReport.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

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
        public static QueueController queue = QueueController.GetQueueController();
        static void Main(string[] args)
        {
            menu.Add("/start", MenuItems.Start);
            menu.Add("/help", MenuItems.Help);
            menu.Add("/status", MenuItems.Status);

            tgBot = new TelegramBotClient("1435986427:AAH7tfPHWBpUQvWFDylWVR1NEswh6zJANbo");
            var me = tgBot.GetMeAsync().Result;
            tgBot.OnMessage += OnNewMessage;
            tgBot.StartReceiving();
            auth = Authorization.GetAuthorization();
            //await auth.GetTemplatesRoot(httpClient);
            //httpClient = auth.AuthAsync();
            tgBot.OnCallbackQuery += OnCallback;
            Console.ReadKey();
        }

        public async static void GetFile(long mId)
        {
            if (users[mId].lastFile == null)
                return;
            var file = tgBot.GetFileAsync(users[mId].lastFile).Result;
            var url = "https://api.telegram.org/file/" + "bot" + /*bot_token*/"1435986427:AAH7tfPHWBpUQvWFDylWVR1NEswh6zJANbo" + "/" + file.FilePath;
            byte[] data;
            using (var client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                data = await content.ReadAsByteArrayAsync();
                using (FileStream f = System.IO.File.Create(document.FileName)) ////path = "wwwroot\\XML\\1.zip"
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
            
            var type = e.CallbackQuery.Data;
            queue.Add(mId, type);
            
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
                        SendMessage(Menu.menuButtons, userID, "Описание будет готово позже, в кратце обработка файлов rfx и fpx в pdf и другое", false);
                        break;
                    case MenuItems.Help:
                        SendMessage(Menu.menuButtons, userID, "скидываете файл, вам будет предложенно выбор в какое разрешение этот файл конвентирован и далее прислан готовый файл." +
                            "Все доступные команды: /help,/start,/status. " +
                            "Бот разработан командой Silent, под руководсвом 'fast report ' https://fastreport.cloud/ru/", false);
                        break;
                    case MenuItems.Status:
                        SendMessage(Menu.menuButtons, userID, "чет делается", false);
                        break;
                }
            }
            if (document != null)
            {
                users[userID].lastFile = document.FileId;
                //Console.WriteLine(document.MimeType);
                //CreateInlineKeyboard(Menu.inlineMenu[0], userID);
                SendMessage(CreateInlineKeyboard(Menu.inlineMenu, userID), userID, "Выберете нужный тип файла");
               
            }      
        }
        public static void SendMessage(InlineKeyboardMarkup keyboardMarkup,long? userId,string text)
        {
            tgBot.SendTextMessageAsync(
                replyMarkup:keyboardMarkup,
                chatId: userId,
                text:text
                ) ;
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
        
        private static InlineKeyboardMarkup CreateInlineKeyboard(string []keyboard, long? userId)
        {
            //InlineKeyboardMarkup key = new InlineKeyboardMarkup();
            InlineKeyboardMarkup key = null;
            InlineKeyboardButton[] keyboardButtons = new InlineKeyboardButton[keyboard.Length];
            for (int i = 0; i < keyboard.Length; i++)
            {
                keyboardButtons[i] = new InlineKeyboardButton { Text = keyboard[i], CallbackData = keyboard[i] };
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
