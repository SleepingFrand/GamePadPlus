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
    private Rect ShotcatZoneRectBase;
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

    [SerializeField] private int List = 1;
    private int ListMax = 1;
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
        ShotcatZoneRectBase = ShotcatZone.GetComponent<RectTransform>().rect;
    }

    /// <summary>
    /// �������� ���������� ��������, ������� ����������� �� ������
    /// </summary>
    /// <param name="obj">������� �������</param>
    /// <param name="zone">������� ����</param>
    /// <returns></returns>
    int GetCountHieght(Rect obj, Rect zone)
    {
        return Mathf.RoundToInt(zone.height / (obj.height + Mathf.Abs(obj.y)));
    }
    /// <summary>
    /// �������� ���������� ��������, ������� ����������� �� ��������
    /// </summary>
    /// <param name="obj">������� �������</param>
    /// <param name="zone">������� ����</param>
    /// <returns></returns>
    int GetCountListSize(Rect obj, Rect zone)
    {
        return Mathf.RoundToInt(zone.width / (obj.width + Mathf.Abs(obj.x)));
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

        int temp = - 1;
        int temp_max = GetCountHieght(rect, ShotcatZone.GetComponent<RectTransform>().rect);
        int max_ON_list = GetCountListSize(rect, ShotcatZone.GetComponent<RectTransform>().rect) * temp_max;

        

        float list = (count - 1) / max_ON_list;

        if(list > 0)
        {
            target_Rect.x += ShotcatZone.GetComponent<RectTransform>().rect.width * list;
        }

        for (int i = 0; i < count - (list * max_ON_list); i++)
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

    public void SetSize(RectTransform trans, Vector2 newSize)
    {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y)) * 2;
    }

    void ReSizeZone(bool ISmap)
    {
        int count_of_list_max = GetCountListSize(ShotCatPrefab.GetComponent<RectTransform>().rect, ShotcatZone.GetComponent<RectTransform>().rect);
        count_of_list_max *= GetCountHieght(ShotCatPrefab.GetComponent<RectTransform>().rect, ShotcatZone.GetComponent<RectTransform>().rect) - 1;

        ListMax = 1;

        if (ISmap)
        {
            ListMax = (Shotcats.Count / count_of_list_max) + 1;
        }
        else
        {
            ListMax = (Shotcats[CurrentMapShotcat].Count / count_of_list_max) + 1;
        }

        SetSize(ShotcatZone.GetComponent<RectTransform>(), new Vector2(ShotcatZoneRectBase.width * ListMax, ShotcatZoneRectBase.height));
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

    void SetList()
    {
        float startX = (ShotcatZoneRectBase.width * Mathf.Round(ListMax / 2));
        ShotcatZone.GetComponent<RectTransform>().localPosition = new Vector3(startX - (ShotcatZoneRectBase.width * (List - 1)), ShotcatZone.GetComponent<RectTransform>().localPosition.y, ShotcatZone.GetComponent<RectTransform>().localPosition.z);
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
                ReSizeZone(true);
                break;
            case 2:
                SetActuve(new List<GameObject>(Shotcats.Keys), false);
                SetActuve(new List<GameObject>(Shotcats[CurrentMapShotcat]), true);
                ReSizeZone(false);
                break;
        }
        List = 1;
        SetList();
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

    public void NextList()
    {
        if(ShotcatZone.GetComponent<RectTransform>().offsetMax.x > 0)
            List++;
        SetList();
    }

    public void BackList()
    {
        if (List != 1)
            List--;
        SetList();
    }
    #endregion
}
