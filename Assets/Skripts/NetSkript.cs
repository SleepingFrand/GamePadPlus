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
    private static int PortReceive = 11000;


    public static void Init()
    {
        ipHost = Dns.GetHostEntry("localhost");
        ipAddr = ipHost.AddressList[1];
        
    }

    public static void SendMessageFromSocket(SendMessage message)
    {
        Task.Run(() => {
            // Устанавливаем удаленную точку для сокета
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, PortSend);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            /*
            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(message.GetByteFromJSON());

            Debug.Log("Сообщение отправленно!");

            sender.Shutdown(SocketShutdown.Send);
            */
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(buffer, 0, buffer.Length);
            e.Completed += new System.EventHandler<SocketAsyncEventArgs>(SendCallback);
            bool completedAsync = false;
            try
            {
                completedAsync = sender.SendAsync(e);
            }
            catch (SocketException se)
            {
                Debug.Log("Socket Exception: " + se.ErrorCode + " Message: " + se.Message);
            }
            if (!completedAsync)
            {
                // The call completed synchronously so invoke the callback ourselves
                SendCallback(this, e);
            }

        });
    }

    private static void SendCallback(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            // You may need to specify some type of state and
            // pass it into the BeginSend method so you don't start
            // sending from scratch
            BeginSend();
            
        }
        else
        {
            Console.WriteLine("Socket Error: {0} when sending to {1}",
                   e.SocketError,
                   _asyncTask.Host);
        }
    }

    static bool DataGeted = true;

    static Socket Receiver;
    static byte[] buffer = new byte[1024];

    private static void Connect()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, PortReceive);
        Receiver = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Receiver.Connect(ipEndPoint);
    }

    private static void ReadSocket()
    {
        Receiver.BeginReceive(buffer, 0, 1024, SocketFlags.None, ReceiveCallback, null);
    }

    private static void DisConnect()
    {
        // Освобождаем сокет
        Receiver.Shutdown(SocketShutdown.Both);
        Receiver.Close();
    }

    private static void ReceiveCallback(System.IAsyncResult asyncResult)
    {
        ReceiveMessage msg = new ReceiveMessage();
        int bytesTransferred = Receiver.EndReceive(asyncResult);
        msg.SetValueFromJSON(buffer);
        if(msg.name == "CharacterPosition")
            DataStore.SetCharaterTransform(msg);
        
        DataGeted = true;
        DisConnect();
    }

    public static void ReceiveMessageFromSocket()
    {
        if (DataGeted)
        {
            DataGeted = false;
            Connect();

            ReadSocket();
        }
    }
}
