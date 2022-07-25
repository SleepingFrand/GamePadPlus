using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;


[System.Serializable]
public class SendMessage
{
    public string name;
    public List<object> values;

    [System.NonSerialized]
    private string json;

    public void CreateWaySend(WAY way)
    {
        name = "WayPoints";
        values = new List<object>();
        values.Add(new List<float>());
        values.Add(new List<float>());
        foreach (Vector2 item in way.positionWayPoints)
        {
            (values[0] as List<float>).Add(item.x);
            (values[1] as List<float>).Add(item.y);
        }
        CreateJson();
    }

    public void CreateStateWayAutoSend(bool start)
    {
        if (start)
            name = "Start";
        else
            name = "Stop";

        name += "Way";
        values = new List<object>();
        CreateJson();
    }

    public void CreateStateWayHandSend(bool start)
    {
        if (start)
            name = "Start";
        else
            name = "Stop";

        name += "Control";
        values = new List<object>();
        CreateJson();
    }

    public void CreateJoystickSend(Vector2 value)
    {
        name = "JoystickValue";

        values = new List<object>();

        values.Add(value.x);
        values.Add(value.y);

        CreateJson();
    }

    private void CreateJson()
    {
        json = JsonConvert.SerializeObject(this);
    }

    public byte[] GetByteFromJSON()
    {
        return Encoding.UTF8.GetBytes(json);
    }
}
[System.Serializable]
public class ReceiveMessage
{
    public string name;
    public List<object> values = new List<object>();

    public void SetValueFromJSON(byte[] json)
    {
        ReceiveMessage temp = JsonConvert.DeserializeObject<ReceiveMessage>(System.Text.Encoding.UTF8.GetString(json));
        if(temp.name == "CharacterPosition" || temp.name == "EndWay")
        {
            this.name = temp.name;
            values = temp.values;
        }
    }

    
}

public class NetSkript : MonoBehaviour
{
    private static IPHostEntry ipHost;
    private static IPAddress ipAddr;
    private static int PortSend = 11000;
    private static int PortReceive = 11001;

    static bool dataReading = true;

    static Socket Receiver;
    static byte[] buffer = new byte[1024];
    public static void Init()
    {
        ipHost = Dns.GetHostEntry("localhost");
        ipAddr = ipHost.AddressList[1];
        Receiver = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, PortReceive);
        Receiver.Bind(ipEndPoint);
        Receiver.Listen(100);
    }



    public static void ReceiveMessageFromSocket()
    {
        if (dataReading)
        {
            dataReading = false;
            Task.Run(() =>
            {
                // Программа приостанавливается, ожидая входящее соединение
                Socket handler = Receiver.Accept();
                string data = null;

                // Мы дождались клиента, пытающегося с нами соединиться

                byte[] bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);

                ReceiveMessage msg = new ReceiveMessage();
                msg.SetValueFromJSON(bytes);
                DataStore.SetCharaterTransform(msg);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                dataReading = true;
            });
            
        }
    }

    public static void SendMessageFromSocket(SendMessage Message)
    {
        // Буфер для входящих данных
        byte[] bytes = new byte[1024];

        // Соединяемся с удаленным устройством

        // Устанавливаем удаленную точку для сокета
        IPHostEntry ipHost = Dns.GetHostEntry("localhost");
        IPAddress ipAddr = ipHost.AddressList[1];
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, PortSend);

        Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Соединяем сокет с удаленной точкой
        sender.Connect(ipEndPoint);

        byte[] msg = Message.GetByteFromJSON();

        // Отправляем данные через сокет
        int bytesSent = sender.Send(msg);
        
        // Освобождаем сокет
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }
}
