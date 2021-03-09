using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace MTUCIchatTCP
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "ChatBackEnd" в коде и файле конфигурации.

    public delegate void ListOfNames(List<string> names, object sender);

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatSend_BackEnd
    {

        Dictionary<string, IChatResive_BackEnd> names = new Dictionary<string, IChatResive_BackEnd>();

        public static event ListOfNames ChatListOfNames;

        IChatResive_BackEnd callback = null;

        public ChatService() { }

        public void Close()
        {
            callback = null;
            names.Clear();
        }

        public void Start(string Name)
        {
            try
            {
                if (!names.ContainsKey(Name))
                {
                    callback = OperationContext.Current.GetCallbackChannel<IChatResive_BackEnd>();
                    AddUser(Name, callback);
                    SendNamesToAll();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop(string Name)
        {
            try
            {
                if (names.ContainsKey(Name))
                {
                    names.Remove(Name);
                    SendNamesToAll();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void SendNamesToAll()
        {
            foreach (KeyValuePair<string, IChatResive_BackEnd> name in names)
            {
                IChatResive_BackEnd proxy = name.Value;
                proxy.SendNames(names.Keys.ToList());
            }

            if (ChatListOfNames != null)
                ChatListOfNames(names.Keys.ToList(), this);
        }

        void IChatSend_BackEnd.SendMessage(byte[] msg, string sender, string receiver)
        {
            foreach (KeyValuePair<string, IChatResive_BackEnd> User in names)
            {
                if (User.Key != sender)
                {
                    callback = User.Value;
                    callback.ReceiveMessage(msg, sender);
                }
            }
        }

        private void AddUser(string name, IChatResive_BackEnd callback)
        {
            names.Add(name, callback);
            if (ChatListOfNames != null)
                ChatListOfNames(names.Keys.ToList(), this);

        }
    }
}
