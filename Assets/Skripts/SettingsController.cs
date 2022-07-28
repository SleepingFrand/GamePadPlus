using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ��������� �������� ��� ������� ��������
/// </summary>
public class SettingsController : MonoBehaviour
{
    #region ����
    /// <summary>
    /// ������ ��� ������ ������ ����� �����
    /// </summary>
    [SerializeField] private GameObject MapNameText;
    /// <summary>
    /// ������ ��� ������ ������ ����� ��������
    /// </summary>
    [SerializeField] private GameObject WayNameText;
    /// <summary>
    /// ������ ��� ������ ���-�� ����� � ��������
    /// </summary>
    [SerializeField] private GameObject WayPointCountText;

    // ������� �������� Rect �������������� ��������� �����
    [SerializeField] private GameObject Left_Edit;
    [SerializeField] private GameObject Top_Edit;
    [SerializeField] private GameObject Right_Edit;
    [SerializeField] private GameObject Bottom_Edit;
    #endregion

    #region ��������� ����
    private DataStore dataStore;
    #endregion

    #region ������
    private void Start()
    {
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
        AtionsSystem.UpdateValueOnSettings += UpdateValue_For_DataStore;
        dataStore = FindObjectOfType<DataStore>();
    }

    private void UpdateValue_For_DataStore()
    {
        SetMapInfo(dataStore.CurrentMap.name_map, dataStore.CurrentMap.RectMap);
        SetWayInfo(dataStore.CurrentWay.name_WAY, dataStore.CurrentWay.positionWayPoints.Count);
        LoadServerImputText();
    }

    /// <summary>
    /// ������������� ���������� � ��������
    /// </summary>
    /// <param name="name">��� ��������</param>
    /// <param name="count">���-�� �����</param>
    public void SetWayInfo(string name, int count)
    {
        WayNameText.GetComponent<Text>().text = name;
        WayPointCountText.GetComponent<Text>().text = count.ToString();
    }
    /// <summary>
    /// �������������� ���������� � �����
    /// </summary>
    /// <param name="name">��� �����</param>
    /// <param name="graf_point">Rect �������������� �����</param>
    public void SetMapInfo(string name, Rect graf_point)
    {
        SetGrafValue(graf_point);
        MapNameText.GetComponent<Text>().text = name;
    }

    /// <summary>
    /// �������� �������������� ���������� � UI
    /// </summary>
    /// <returns></returns>
    public Rect GetGrafValue()
    {
        Rect rect = new Rect();
        rect.x = float.Parse(Left_Edit.GetComponent<InputField>().text);
        rect.y = float.Parse(Top_Edit.GetComponent<InputField>().text);
        rect.xMax = float.Parse(Right_Edit.GetComponent<InputField>().text);
        rect.yMax = float.Parse(Bottom_Edit.GetComponent<InputField>().text);
        return rect;
    }

    /// <summary>
    /// ���������� �������������� ���������� � UI
    /// </summary>
    void SetGrafValue(float left, float top, float right, float bottom)
    {
        Left_Edit.GetComponent<InputField>().text       = left.ToString();
        Top_Edit.GetComponent<InputField>().text        = top.ToString();
        Right_Edit.GetComponent<InputField>().text      = right.ToString();
        Bottom_Edit.GetComponent<InputField>().text     = bottom.ToString();
    }
    /// <summary>
    /// ���������� �������������� ���������� � UI
    /// </summary>
    void SetGrafValue(Rect rect)
    {
        Left_Edit.GetComponent<InputField>().text       = rect.x.ToString();
        Top_Edit.GetComponent<InputField>().text        = rect.y.ToString();
        Right_Edit.GetComponent<InputField>().text      = rect.width.ToString();
        Bottom_Edit.GetComponent<InputField>().text     = rect.height.ToString();
    }
    #endregion

    #region ��������� ������
    /// <summary>
    /// ��������� ����� �������� � ������� �� ��� ����� �����
    /// </summary>
    public void LoadNewMap()
    {
        dataStore.CreateMap();
    }
    /// <summary>
    /// ��������� ������� �����
    /// </summary>
    public void SaveMap()
    {
        dataStore.CurrentMap.RectMap = GetGrafValue();
        dataStore.SaveMap();
    }
    /// <summary>
    /// ��������� ������� �������
    /// </summary>
    public void SaveWay()
    {
        dataStore.SaveWay();
    }
    /// <summary>
    /// �������� ��� �����
    /// </summary>
    public void ChangeNameMap(string Name) 
    {
        dataStore.ChangeNameMap(Name);
    }
    /// <summary>
    /// �������� ��� ��������
    /// </summary>
    public void ChangeNameWay(string Name)
    {
        dataStore.ChangeNameWay(Name);
    }
    /// <summary>
    /// ������� �������
    /// </summary>
    public void CreateWay()
    {
        dataStore.CreateWay();
    }
    /// <summary>
    /// ������� ������� �����
    /// </summary>
    public void DeleteMap()
    {
        dataStore.DeleteCurrentMap();
    }
    /// <summary>
    /// ������� ������� �������
    /// </summary>
    public void DeleteWay()
    {
        dataStore.DeleteCurrentWay();
    }
    #endregion

    #region ��������� ������ �������
    private string TempIp = "";
    private int TempPort = 0;
    [SerializeField] private GameObject SettingsServerPanel;
    [SerializeField] private Text InputTextIp;
    [SerializeField] private Text InputTextPort;

    public void SwapAtcivePanelSettingsServer()
    {
        SettingsServerPanel.SetActive(!SettingsServerPanel.active);
    }

    private void LoadServerImputText()
    {
        TempIp = InputTextIp.text = dataStore.GetIPServer();
        
        InputTextPort.text = (TempPort = dataStore.GetPortServer()).ToString();
    }

    public void SetTempIP(string text)
    {
        TempIp = text;
    }
    public void SetTempPort(string text)
    {
        TempPort = Convert.ToInt32(text);
    }

    public void AseptServerSettings()
    {
        dataStore.SetAdressServer(TempIp,TempPort);
        SwapAtcivePanelSettingsServer();
    }

    public void CanselServerSettings()
    {
        LoadServerImputText();
        SwapAtcivePanelSettingsServer();
    }
    #endregion
}
