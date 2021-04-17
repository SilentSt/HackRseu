using System;
using System.Collections.Generic;
using System.Text;

namespace rseuHack
{

    class Menu
    {
        public static string[] inlineMenu = new string[] { "PDF", "RTF","DOCX","SVG" };
        public static string[] menuButtons = new string[3]{ "/help", "/start", "/status" };

        public static void SendMenuButtons(long? userId)
        {
            Program.SendMessage(menuButtons,userId,"Привет",false);
        }
        
    }
}
