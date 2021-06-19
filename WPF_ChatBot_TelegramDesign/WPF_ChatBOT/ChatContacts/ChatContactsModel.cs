using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ChatBOT.ChatContacts
{
    class ChatContactNode
    {
        public string imagePath { get; set; }
        public string groupImagePath { get; set; }
        public string contactName { get; set; }
        public DateTime dateTime { get; set; }
        public string lastMessage { get; set; }
        //public string numberOfUnreadMessages { get; set; }

        public ChatContactNode()
        {
            imagePath = @"C:\Users\User\Source\Repos\WPF_ChatBOT_MVVM\WPF_ChatBOT\WPF_ChatBOT\Images\Background.jpg";
            groupImagePath = @"C:\Users\User\Source\Repos\WPF_ChatBOT_MVVM\WPF_ChatBOT\WPF_ChatBOT\Images\ico_Triangle.png";
            contactName = "For testing";
            dateTime = DateTime.Now.Date;
            lastMessage = "TestUser: Last message hello world";
        }

        
    }

    class ChatContactsModel
    {
        List<ChatContactNode> contactsList { get; set; }

        private static ChatContactsModel instance;

        public ChatContactsModel()
        {
            contactsList = new List<ChatContactNode>();
            contactsList.Add(new ChatContactNode());
            contactsList.Add(new ChatContactNode());
        }

        public static ChatContactsModel getInstance()
        {
            if (instance == null)
                instance = new ChatContactsModel();
            return instance;
        }

        public ObservableCollection<ChatContactNode> getContacts()
        {
            return new ObservableCollection<ChatContactNode>(contactsList);
        }
    }
}
