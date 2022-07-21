using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;

public class FileMagadger_UIControl : MonoBehaviour
{
    [SerializeField] private int StateManedger = 0;
    [SerializeField] private GameObject ShotcatZone;
    [SerializeField] private GameObject PanelMain;
    [SerializeField] private bool MainState = false;

    [SerializeField] private GameObject ShotCatPrefab;

    [SerializeField] private GameObject CurrentMapShotcat;

    [SerializeField] private Vector2 AdjustPosition;

    Dictionary<GameObject,  List<GameObject>> Shotcats = new Dictionary<GameObject, List<GameObject>>();
    DataStore dataStore;

    private RectTransform RectTransform;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(SetStete_Main);
        RectTransform = this.gameObject.GetComponent<RectTransform>();
        dataStore = FindObjectOfType<DataStore>();
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
    }

    int GetCountHieght(Rect obj, Rect zone)
    {
        return Mathf.CeilToInt(zone.height / (obj.height + Mathf.Abs(obj.y)));
    }

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



// Update is called once per frame
    void Update()
    {

    }

    void SetActuve(List<GameObject> objects, bool state)
    {
        foreach(GameObject item in objects)
        {
            item.SetActive(state);
        }
    }

    public void SetWayState(GameObject obj)
    {
        SetActuve(Shotcats[CurrentMapShotcat], false);
        CurrentMapShotcat = obj;
        SetActuve(Shotcats[CurrentMapShotcat], true);
    }

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

    public void SetStete_Main()
    {
        MainState = !MainState;
        PanelMain.SetActive(MainState);

        RectTransform.anchoredPosition -= new Vector2(0, PanelMain.GetComponent<RectTransform>().rect.yMax * (MainState?2:-2));
    }
}
