using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс контролирующий работу загрузки карт через UI
/// </summary>
public class FileMagadger_UIControl : MonoBehaviour
{
    #region Поля
    /// <summary>
    /// Хранит номер выбранной вкладки
    /// </summary>
    [SerializeField] private int StateManedger = 0;
    /// <summary>
    /// Панель для генерации ярлыков
    /// </summary>
    [SerializeField] private GameObject ShotcatZone;
    /// <summary>
    /// Главная панель менеджера
    /// </summary>
    [SerializeField] private GameObject PanelMain;
    /// <summary>
    /// Состояние главной панели
    /// </summary>
    [SerializeField] private bool MainState = false;

    /// <summary>
    /// Префаб ярлыка
    /// </summary>
    [SerializeField] private GameObject ShotCatPrefab;

    /// <summary>
    /// Ярлык текущей выбранной карты
    /// </summary>
    [SerializeField] private GameObject CurrentMapShotcat;

    /// <summary>
    /// Коррактеровка позиции ярлыка
    /// </summary>
    [SerializeField] private Vector2 AdjustPosition;
    #endregion

    #region Приватные поля
    /// <summary>
    /// Библиотека ярлыков (Карта) : (Список Маршрутов)
    /// </summary>
    private Dictionary<GameObject,  List<GameObject>> Shotcats = new Dictionary<GameObject, List<GameObject>>();

    private DataStore dataStore;
    private RectTransform RectTransform;
    #endregion

    #region Методы
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(SetStete_Main);
        RectTransform = this.gameObject.GetComponent<RectTransform>();
        dataStore = FindObjectOfType<DataStore>();
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
    }

    /// <summary>
    /// Получить количество объектов, которые поместяться по высоте
    /// </summary>
    /// <param name="obj">Размеры объекта</param>
    /// <param name="zone">Размеры зоны</param>
    /// <returns></returns>
    int GetCountHieght(Rect obj, Rect zone)
    {
        return Mathf.CeilToInt(zone.height / (obj.height + Mathf.Abs(obj.y)));
    }
    /// <summary>
    /// Создает ярлык в соответствие с его номером на поле
    /// </summary>
    /// <param name="count">Номер в списке</param>
    /// <param name="text">Надпись на ярлыке</param>
    /// <param name="IsMap">Параметр фильтрующий карты</param>
    /// <returns>Объект карты</returns>
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
    /// Удаляет ярлыки и очищает из список
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
    /// Загружает ярлыки с главного хранилища
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
    /// Обновляет данные, получаемые из главного хранилища
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
    /// Устанавливает значение активности списка объектов
    /// </summary>
    /// <param name="objects">Список объектов</param>
    /// <param name="state">Требуемый статус</param>
    void SetActuve(List<GameObject> objects, bool state)
    {
        foreach(GameObject item in objects)
        {
            item.SetActive(state);
        }
    }
    #endregion

    #region Публичные методы
    /// <summary>
    /// Отобразить список путей, для полученной карты
    /// </summary>
    /// <param name="obj">Объект ярлыка карты</param>
    public void SetWayState(GameObject obj)
    {
        SetActuve(Shotcats[CurrentMapShotcat], false);
        CurrentMapShotcat = obj;
        SetActuve(Shotcats[CurrentMapShotcat], true);
    }
    /// <summary>
    /// Смена вкладки менеджера
    /// </summary>
    /// <param name="state">Вкладка которую нужно отобразить</param>
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
    /// Отобразает\прячет менеджер
    /// </summary>
    public void SetStete_Main()
    {
        MainState = !MainState;
        PanelMain.SetActive(MainState);

        RectTransform.anchoredPosition -= new Vector2(0, PanelMain.GetComponent<RectTransform>().rect.yMax * (MainState?2:-2));
    }
    #endregion
}
