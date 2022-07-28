using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;


public class NetSkript : MonoBehaviour
{
    [SerializeField] private JoyMsg MSGjoystickMsg;
    [SerializeField] private PointCloudMsg WayPointsMsg;

    public string IpAdres = "127.0.0.1";
    public int port = 10000;

    private ROSConnection ros;
    private List<string> topicNames = new List<string>(){ "JoyMsg", "WayPoints" };
    public bool Connected { get; private set; } = false;

    private DataStore _dataStore;
    private void Start()
    {
        _dataStore = FindObjectOfType<DataStore>();
        Init();
    }
    private void Update()
    {
        if (!Connected)
        {
            Init();
        }
        else
        {
            _dataStore.ShowErrorMessage("Error connecting to server...");
        }
    }
    private void Init()
    {
        try
        {
            ros = ROSConnection.GetOrCreateInstance();
            ros.ShowHud = false;
            ros.Connect(IpAdres, port);
            ros.RegisterPublisher<JoyMsg>(topicNames[0]);
            ros.RegisterPublisher<PointCloudMsg>(topicNames[1]);
            ROSConnection.GetOrCreateInstance().Subscribe<PoseMsg>("CharacterPose", SetCharacterPosition);
            ROSConnection.GetOrCreateInstance().Subscribe<JoyMsg>("EndWay", SetWayStatusEnd);
            Connected = true;
            _dataStore.DisableErrorMessage();
        }
        catch (System.Exception e)
        {
            Connected = false;
            _dataStore.ShowErrorMessage("Error connecting to server...");
        }
    }
    public void ReInit()
    {
        ros.Disconnect();
        ros.Connect(IpAdres, port);
    }

    public void SendMessageJoyMsg(Vector2 JoyStick, int AutoContol, int HandContol)
    {
        if (Connected)
            try
            {
                float[] axes = new float[] { JoyStick.x, JoyStick.y };
                int[] bottoms = new int[] { AutoContol, HandContol };
                MSGjoystickMsg = new JoyMsg(new HeaderMsg(), axes, bottoms);
                ros.Publish(topicNames[0], MSGjoystickMsg);
                _dataStore.DisableErrorMessage();
            }
            catch
            {
                _dataStore.ShowErrorMessage("Error send JoyMsg to server!");
            }
    }
    public void SendMessageWay(List<Vector2> points)
    {
        if (Connected)
            try
            {
                List<Point32Msg> Point = new List<Point32Msg>();
                foreach (Vector2 item in points)
                {
                    Point.Add(new Point32Msg(item.x, item.y, 0));
                }
                WayPointsMsg = new PointCloudMsg(new HeaderMsg(), Point.ToArray(), new ChannelFloat32Msg[0]);
                ros.Publish(topicNames[1], WayPointsMsg);
                _dataStore.DisableErrorMessage();
            }
            catch
            {
                _dataStore.ShowErrorMessage("Error send PointCloudMsg to server!");
            }
        
    }

    private void SetCharacterPosition(PoseMsg pose)
    {
        _dataStore.CharacterPosition = new Vector2((float)pose.position.x, (float)pose.position.y);
        _dataStore.CharacterDirection = new Quaternion((float)pose.orientation.x, (float)pose.orientation.y, (float)pose.orientation.z, (float)pose.orientation.w);
    }
    private void SetWayStatusEnd(JoyMsg joy)
    {
        if (joy.buttons[1] == 0)
        {
            _dataStore.EndAutoWay();
        }
    }
}
