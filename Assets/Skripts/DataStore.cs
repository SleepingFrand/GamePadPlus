using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Vector2 CharacterPosition = new Vector2();
    public Quaternion CharacterDirection = new Quaternion();

    public Vector2 JoyStickValue = new Vector2();

    public List<int> ButtonsStatus = new List<int>() { 0, 1 };

    [SerializeField] private GameObject ErrorText;
    #endregion

    #region ��������� ����
    /// <summary>
    /// ��������� ������� �������������� �������
    /// </summary>
    private DataMagedger _dataManedger;

    private NetSkript _netSkript;
    #endregion

    #region ������� ������
    //�������� �������� ������ � ������ ����� � ���� ��� ���
    //���������� ������
    //���������� "��������" �� ��������� ������ �� ���������
    private void Start()
    {
        _dataManedger = this.gameObject.GetComponent<DataMagedger>();
        _netSkript = this.gameObject.GetComponent<NetSkript>();

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

        SendWayPoints();
    }


    public void ShowErrorMessage(string text)
    {
        ErrorText.SetActive(true);
        ErrorText.GetComponent<Text>().text = text;
    }
    public void DisableErrorMessage()
    {
        ErrorText.SetActive(false);
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
        SendWayPoints();
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
    public void SetXValueStick(float value)
    {
        JoyStickValue.x = value;
        SendValueStick();
    }
    public void SetYValueStick(float value)
    {
        JoyStickValue.y = value;
        SendValueStick();
    }
    public void SendValueStick()
    {
        _netSkript.SendMessageJoyMsg(JoyStickValue, ButtonsStatus[0], ButtonsStatus[1]);
    }

    private void SendWayPoints()
    {
        _netSkript.SendMessageWay(CurrentWay.positionWayPoints);
    }


    public void SetAdressServer(string ip, int port)
    {
        _netSkript.IpAdres = ip;
        _netSkript.port = port;
        _netSkript.ReInit();
    }

    public string GetIPServer()
    {
        return _netSkript.IpAdres;
    }

    public int GetPortServer()
    {
        return _netSkript.port;
    }

    public void EndAutoWay()
    {
        GameObject.FindGameObjectWithTag("EndWayCB").GetComponent<CheakBox_UI>().OffBoxWithoutEvent();
    }

    #endregion
}