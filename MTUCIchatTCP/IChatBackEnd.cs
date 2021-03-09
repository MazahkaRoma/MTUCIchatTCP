using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Reflection;

namespace MTUCIchatTCP
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IChatBackEnd" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IChatResive_BackEnd))]
    public interface IChatSend_BackEnd
    {
        
        [OperationContract(IsOneWay = true)]
        void SendMessage(byte[] msg, string sender, string receiver);
        [OperationContract(IsOneWay = true)]
        void Start(string Name);
        [OperationContract(IsOneWay = true)]
        void Stop(string Name);
    }
    public interface IChatResive_BackEnd
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(byte[] msg, string receiver);
        [OperationContract(IsOneWay = true)]
        void SendNames(List<string> names);
    }
}
