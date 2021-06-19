using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WPF_ChatBOT.Base;

namespace WPF_ChatBOT.ChatBot
{
    class ChatBotModel
    {
        XmlSerializableDictionary<string, List<string>> answers;

        public string imagePathBot { get; set; }

        public ChatBotModel()
        {
            answers = new XmlSerializableDictionary<string, List<string>>();
            loadSimpleData();
            imagePathBot = @"C:\Users\User\Source\Repos\WPF_ChatBOT_MVVM\WPF_ChatBOT\WPF_ChatBOT\Images\photo.ico";
        }


        #region Singleton
        private static ChatBotModel instance;

        public static ChatBotModel getInstance()
        {
            if (instance == null)
                instance = new ChatBotModel();
            return instance;
        }
        #endregion

        public void loadSimpleData()
        {
            this.answers.Add("Привет", new List<string>() { "Привет, как дела ?", "Здрарова, ты как там ?" });
            this.answers.Add("Пока", new List<string>() { "До встречи!", "Пока, был рад встречи" });
        }

        public ObservableCollection<string> getAnswers(string key)
        {
            ObservableCollection<string> Answers = new ObservableCollection<string>();
            foreach (KeyValuePair<string, List<string>> kv in this.answers)
            {
                if (kv.Key == key)
                {
                    foreach (var s in kv.Value)
                        Answers.Add(s);
                };
            }

            return Answers;
        }

        public ObservableCollection<string> getKeyWords()
        {
            ObservableCollection<string> keyWords = new ObservableCollection<string>();
            foreach (KeyValuePair<string, List<string>> kv in this.answers)
            {
                keyWords.Add(kv.Key);
            }
            return keyWords;
        }


        #region Serializable
        public void LoadAnswers()
        {

        }

        public void SaveAnswers()
        {

        }
        #endregion


    }
}
