using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� ����������� ���� �������� ���������
/// ��������� ���� ��������, ����������� � ���������� ������
/// </summary>
public class DataStore : MonoBehaviour
{
    #region ����
    /// <summary>
    /// ��������� � ����������� �����
    /// </summary>
    public MAP CurrentMap;
    /// <summary>
    /// ��������� � ����������� �������
    /// </summary>
    public WAY CurrentWay;

    /// <summary>
    /// ������ ������ ������ ��� �������� ������� �� �����
    /// </summary>
    public List<Shotcat> shotcats;

    public static Vector2 CharacterPosition = new Vector2();
    public static float CharacterDirection = 0;
    #endregion

    #region ��������� ����
    /// <summary>
    /// ��������� ������� �������������� �������
    /// </summary>
    private DataMagedger _dataManedger;
    #endregion

    #region ������� ������
    //�������� �������� ������ � ������ ����� � ���� ��� ���
    //���������� ������
    //���������� "��������" �� ��������� ������ �� ���������
    private void Start()
    {
        NetSkript.Init();

        _dataManedger = this.gameObject.GetComponent<DataMagedger>();

        if (!_dataManedger.TryLoadMap(_dataManedger.pathsMap[0], out CurrentMap))
        {
            Debug.Log("Error on load current map by id 0");
        }

        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, CurrentMap.names_WAY[0], out CurrentWay))
        {
            Debug.Log("Error on load current way by id 0");
        }

        shotcats = _dataManedger.GetShortName();

       AtionsSystem.UpdateValueForDataStore.Invoke();
    }
    #endregion

    #region ������ ����������� � ������
    /// <summary>
    /// ���������� � ������������� ����� �� ����� �� �����
    /// </summary>
    /// <param name="name">��� ��������� �����</param>
    public void ChengeMap(string name)
    {
        if (!_dataManedger.TryLoadMap(name, out CurrentMap))
        {
            Debug.Log("Error on load current map for name" + name);
        }

        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, CurrentMap.names_WAY[0], out CurrentWay))
        {
            Debug.Log("Error on load current way for name" + name);
        }

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }
    /// <summary>
    /// ��������� ������� �����
    /// </summary>
    public void SaveMap()
    {
        _dataManedger.SaveMap(CurrentMap);
    }
    /// <summary>
    /// ��������� ����� �� �������
    /// </summary>
    /// <param name="other">��������� �����</param>
    public void SaveMap(MAP other)
    {
        _dataManedger.SaveMap(other);
    }
    /// <summary>
    /// ������� ����� ����� � �������� �����������
    /// </summary>
    public void CreateMap()
    {
        MAP newMap = new MAP();

        if(!_dataManedger.CreateMap(out newMap))
        {
            return;
        }

        shotcats = _dataManedger.GetShortName();

        ChengeMap(newMap.name_map);

    }
    /// <summary>
    /// �������� ��� ������� �����
    /// </summary>
    /// <param name="NewName">����� ���</param>
    public void ChangeNameMap(string NewName)
    {
        if (NewName == "" || NewName == CurrentMap.name_map)
            return;

        _dataManedger.ChangeName(CurrentMap, NewName);

        shotcats = _dataManedger.GetShortName();

        ChengeMap(NewName);
    }
    /// <summary>
    /// ������� ������� �����
    /// </summary>
    public void DeleteCurrentMap()
    {
        if (_dataManedger.TryRemoveMap(CurrentMap.name_map))
        {
            ChengeMap(_dataManedger.pathsMap[0]);
        }

        shotcats = _dataManedger.GetShortName();

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }
    #endregion

    #region ������ ����������� � ���������
    /// <summary>
    /// ���������� � ������������� ������� �� ����� �� �����
    /// </summary>
    /// <param name="name">��� ���������� ��������</param>
    public void ChengeWay(string name)
    {
        if (!_dataManedger.TryLoadWay(CurrentMap.name_map, name, out CurrentWay))
        {
            Debug.Log("Error on load current way by id 0");
        }

        AtionsSystem.UpdateValueForDataStore.Invoke();
    }

    /// <summary>
    /// ��������� ������� �������
    /// </summary>
    public void SaveWay()
    {
        _dataManedger.SaveWay(CurrentMap, CurrentWay);
    }

    /// <summary>
    /// ������� ����� ������� � �������� �����������
    /// </summary>
    public void CreateWay()
    {

        WAY newWay = new WAY();

        newWay.positionWayPoints = new List<Vector2>();
        newWay.name_WAY = "NewWay_" + CurrentMap.names_WAY.Count.ToString();
        
        _dataManedger.SaveWay(CurrentMap, newWay);

        CurrentMap.names_WAY.Add(newWay.name_WAY);
        _dataManedger.SaveMap(CurrentMap);

        shotcats = _dataManedger.GetShortName();

        ChengeWay(newWay.name_WAY);
    }

    /// <summary>
    /// �������� ��� �������� ��������
    /// </summary>
    /// <param name="name">����� ��� ��������</param>
    public void ChangeNameWay(string name)
    {
        if (name == "" || name == CurrentWay.name_WAY)
            return;

        _dataManedger.ChangeName(CurrentMap, name, CurrentWay);

        shotcats = _dataManedger.GetShortName();

        ChengeWay(name);
    }

    /// <summary>
    /// ������� ������� �������
    /// </summary>
    public void DeleteCurrentWay()
    {
        if (_dataManedger.TryRemoveWay(CurrentMap.name_map, CurrentWay.name_WAY))
        {
            CurrentMap.names_WAY.Remove(CurrentWay.name_WAY);
            SaveMap();
            ChengeWay(CurrentMap.names_WAY[0]);
        }

        shotcats = _dataManedger.GetShortName();

        AtionsSystem.UpdateValueForDataStore.Invoke();
        AtionsSystem.UpdateValueOnSettings();
    }
    #endregion

    #region ������ ��� ����
    private bool HandControl = false;
    public void SendAutoWay(bool state)
    {
        SendMessage msg = new SendMessage();
        msg.CreateStateWayAutoSend(state);

        if (state)
            HandControl = false;

        NetSkript.SendMessageFromSocket(msg);
    }
    public void SendHandoWay(bool state)
    {
        SendMessage msg = new SendMessage();
        msg.CreateStateWayHandSend(state);
        HandControl = state;

        NetSkript.SendMessageFromSocket(msg);
    }

    private void SendWay()
    {
        SendMessage msg = new SendMessage();
        msg.CreateWaySend(CurrentWay);

        NetSkript.SendMessageFromSocket(msg);
    }

    public void SendJoystick(Vector2 value)
    {
        if (HandControl && (value.x != float.PositiveInfinity && value.y != float.NegativeInfinity) && (value.y != float.PositiveInfinity && value.x != float.NegativeInfinity))
        {
            SendMessage msg = new SendMessage();
            msg.CreateJoystickSend(value);

            NetSkript.SendMessageFromSocket(msg);
        } 
    }

    public static void SetCharaterTransform(ReceiveMessage msg)
    {
        CharacterPosition = (Vector2)msg.values[0];
        CharacterDirection = (float)msg.values[1];
        AtionsSystem.UpdateValueOnCharacter();
    }

    private void Update()
    {
        NetSkript.ReceiveMessageFromSocket();
    }

    #endregion
}