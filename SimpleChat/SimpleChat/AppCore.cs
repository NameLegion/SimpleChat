using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleChat
{
    class AppCore
    {
        // Тип приложения --------------------------------------------
        public enum AppType { Server, Client }
        private AppType ApplicationType;
        public AppType ApplicationTypeProp
        {
            get { return ApplicationType; }
            set { ApplicationType = value; }
        }
        // Ник в чате -----------------------------------------------
        private string ChatNick = "BadUser";
        public string ChatNickProp
        {
            get { return ChatNick; }
            set { ChatNick = value; }
        }
        // ----------------------------------------------------------
    }
}
