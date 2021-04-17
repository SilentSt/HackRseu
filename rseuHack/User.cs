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
        public string lastFile;
    }
}
