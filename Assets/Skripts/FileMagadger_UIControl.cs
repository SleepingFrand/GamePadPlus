using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� �������������� ������ �������� ���� ����� UI
/// </summary>
public class FileMagadger_UIControl : MonoBehaviour
{
    #region ����
    /// <summary>
    /// ������ ����� ��������� �������
    /// </summary>
    [SerializeField] private int StateManedger = 0;
    /// <summary>
    /// ������ ��� ��������� �������
    /// </summary>
    [SerializeField] private GameObject ShotcatZone;
    /// <summary>
    /// ������� ������ ���������
    /// </summary>
    [SerializeField] private GameObject PanelMain;
    /// <summary>
    /// ��������� ������� ������
    /// </summary>
    [SerializeField] private bool MainState = false;

    /// <summary>
    /// ������ ������
    /// </summary>
    [SerializeField] private GameObject ShotCatPrefab;

    /// <summary>
    /// ����� ������� ��������� �����
    /// </summary>
    [SerializeField] private GameObject CurrentMapShotcat;

    /// <summary>
    /// ������������� ������� ������
    /// </summary>
    [SerializeField] private Vector2 AdjustPosition;
    #endregion

    #region ��������� ����
    /// <summary>
    /// ���������� ������� (�����) : (������ ���������)
    /// </summary>
    private Dictionary<GameObject,  List<GameObject>> Shotcats = new Dictionary<GameObject, List<GameObject>>();

    private DataStore dataStore;
    private RectTransform RectTransform;
    #endregion

    #region ������
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(SetStete_Main);
        RectTransform = this.gameObject.GetComponent<RectTransform>();
        dataStore = FindObjectOfType<DataStore>();
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
    }

    /// <summary>
    /// �������� ���������� ��������, ������� ����������� �� ������
    /// </summary>
    /// <param name="obj">������� �������</param>
    /// <param name="zone">������� ����</param>
    /// <returns></returns>
    int GetCountHieght(Rect obj, Rect zone)
    {
        return Mathf.CeilToInt(zone.height / (obj.height + Mathf.Abs(obj.y)));
    }
    /// <summary>
    /// ������� ����� � ������������ � ��� ������� �� ����
    /// </summary>
    /// <param name="count">����� � ������</param>
    /// <param name="text">������� �� ������</param>
    /// <param name="IsMap">�������� ����������� �����</param>
    /// <returns>������ �����</returns>
    GameObject CreateShotcat(int count, string text, bool IsMap)
    {
        Rect rect = ShotCatPrefab.GetComponent<RectTransform>().rect;

        rect.position = AdjustPosition;

        Rect target_Rect = rect;

        int temp = -1;
        int temp_max = GetCountHieght(rect, ShotcatZone.GetComponent<RectTransform>().rect);

        for (int i = 0; i < count; i++)
        {
            temp++;
            if (temp == temp_max)
            {
                temp = 0;
                target_Rect.x += rect.x + rect.width;
            }
        }

        target_Rect.y -= (Mathf.Abs(rect.y) + rect.height) * temp;
         
        GameObject lable = Instantiate(ShotCatPrefab,  new Vector3(target_Rect.x, target_Rect.y, 0), Quaternion.identity);
        lable.transform.SetParent(ShotcatZone.transform, false);
        lable.GetComponent<Text>().text = text;
        lable.SetActive(false);
        if(IsMap)
            lable.GetComponent<Button>().onClick.AddListener(() => dataStore.ChengeMap(text));
        else
            lable.GetComponent<Button>().onClick.AddListener(() => dataStore.ChengeWay(text));
        return lable;
    }
    /// <summary>
    /// ������� ������ � ������� �� ������
    /// </summary>
    void ClearShotcats()
    {
        foreach (GameObject map in Shotcats.Keys)
        {
            foreach (GameObject way in Shotcats[map])
            {
                Destroy(way);
            }
            Destroy(map);
        }
        Shotcats.Clear();
    }
    /// <summary>
    /// ��������� ������ � �������� ���������
    /// </summary>
    void LoadMapShotcat()
    {
        ClearShotcats();

        int count_map = 1;

        foreach (Shotcat item in dataStore.shotcats)
        {
            int count_way = 1;
            List<GameObject> ways = new List<GameObject>();
            foreach (string way in item.WayName)
            {
                ways.Add(CreateShotcat(count_way++, way, false));
            }

            Shotcats.Add(CreateShotcat(count_map++, item.MapName, true), ways);
        }
    }
    /// <summary>
    /// ��������� ������, ���������� �� �������� ���������
    /// </summary>
    void UpdateValue_For_DataStore()
    {
        LoadMapShotcat();


        foreach (GameObject item in Shotcats.Keys)
        {
            if (item.GetComponent<Text>().text == dataStore.CurrentMap.name_map)
            {
                CurrentMapShotcat = item;
                break;
            }
        }
         

        SetZonetate(1);
    }
    /// <summary>
    /// ������������� �������� ���������� ������ ��������
    /// </summary>
    /// <param name="objects">������ ��������</param>
    /// <param name="state">��������� ������</param>
    void SetActuve(List<GameObject> objects, bool state)
    {
        foreach(GameObject item in objects)
        {
            item.SetActive(state);
        }
    }
    #endregion

    #region ��������� ������
    /// <summary>
    /// ���������� ������ �����, ��� ���������� �����
    /// </summary>
    /// <param name="obj">������ ������ �����</param>
    public void SetWayState(GameObject obj)
    {
        SetActuve(Shotcats[CurrentMapShotcat], false);
        CurrentMapShotcat = obj;
        SetActuve(Shotcats[CurrentMapShotcat], true);
    }
    /// <summary>
    /// ����� ������� ���������
    /// </summary>
    /// <param name="state">������� ������� ����� ����������</param>
    public void SetZonetate(int state)
    {
        switch (state)
        {
            case 1:
                SetActuve(new List<GameObject>(Shotcats.Keys), true);
                SetActuve(new List<GameObject>(Shotcats[CurrentMapShotcat]), false);
                break;
            case 2:
                SetActuve(new List<GameObject>(Shotcats.Keys), false);
                SetActuve(new List<GameObject>(Shotcats[CurrentMapShotcat]), true);
                break;
        }
    }
    /// <summary>
    /// ����������\������ ��������
    /// </summary>
    public void SetStete_Main()
    {
        MainState = !MainState;
        PanelMain.SetActive(MainState);

        RectTransform.anchoredPosition -= new Vector2(0, PanelMain.GetComponent<RectTransform>().rect.yMax * (MainState?2:-2));
    }
    #endregion
}
