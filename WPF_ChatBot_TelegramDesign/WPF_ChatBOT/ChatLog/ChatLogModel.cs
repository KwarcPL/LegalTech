using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_ChatBOT.Base;
using System.Collections.ObjectModel;

using WPF_ChatBOT.ChatBot;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace WPF_ChatBOT.ChatLog
{
    class ChatLogNode
    {
        public string imagePath { get; set; }
        public string userName { get; set; }
        public string message { get; set; }
        public string dateTime { get; set; }

        public string borderForeground { get; set; }
        public string borderPosition { get; set; }
        public ChatLogNode()
        {
            imagePath = @"C:\imgs\Robo.jpg";
            userName = "Balera";
            message = "Cześć, mów do mnie :)";
            dateTime = DateTime.Now.ToShortTimeString();
            borderForeground = "#FF17212B";
            borderPosition = "Left";
        }
    }

    class ChatLogModel
    {

        public XmlSerializableDictionary<string, List<ChatLogNode>> messagesLog { get; set; }

        #region Singleton
        private static ChatLogModel instance;

        private ChatLogModel()
        {
            this.messagesLog = new XmlSerializableDictionary<string, List<ChatLogNode>>();
            this.messagesLog.Add("For testing", new List<ChatLogNode> { new ChatLogNode() }); // Что бы было что видеть
        }

        public static ChatLogModel getInstance()
        {
            if (instance == null)
                instance = new ChatLogModel();
            return instance;
        }
        #endregion

        public ObservableCollection<ChatLogNode> getChat(string correspondenceNickName)
        {
            ICollection<ChatLogNode> temp = messagesLog[correspondenceNickName];

            foreach (var msg in temp)
            {
                if (msg.userName == "It's me")
                {
                    msg.borderForeground = "#FF2B5278";
                    msg.borderPosition = "Right";
                    msg.imagePath = "";
                }
                else
                {
                    msg.imagePath = ChatBotModel.getInstance().imagePathBot;
                }
            }
            return new ObservableCollection<ChatLogNode>(temp);
        }


        public void addMessageToLog(string correspondenceNickName, string userName, string msg, string imagePath = @"C:\Users\User\Source\Repos\WPF_ChatBOT_MVVM\WPF_ChatBOT\WPF_ChatBOT\Images\Background.jpg")
        {
            ChatLogNode cln = new ChatLogNode();
            cln.imagePath = imagePath;
            cln.userName = userName;
            cln.message = msg;
            messagesLog[correspondenceNickName].Add(cln);
        }


        public void loadLog()
        {

        }


        public void saveLog()
        {
           
        }


    }
}
