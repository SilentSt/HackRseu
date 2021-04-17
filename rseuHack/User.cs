using System;
using System.Collections.Generic;
using System.Text;

namespace rseuHack
{
    class User
    {
        public string userName;
        public Program.MenuItems menuItems = Program.MenuItems.Start;
        public DateTime lastMessage;
        public List<string> lastFile = new List<string>();
    }
}
