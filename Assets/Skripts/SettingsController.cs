using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Реализует контроль над панелью настроек
/// </summary>
public class SettingsController : MonoBehaviour
{
    #region Поля
    /// <summary>
    /// Объект для вывода текста имени карты
    /// </summary>
    [SerializeField] private GameObject MapNameText;
    /// <summary>
    /// Объект для вывода текста имени маршрута
    /// </summary>
    [SerializeField] private GameObject WayNameText;
    /// <summary>
    /// Объект для вывода кол-ва точек в маршруте
    /// </summary>
    [SerializeField] private GameObject WayPointCountText;

    // Объекты хранящие Rect географических координат карты
    [SerializeField] private GameObject Left_Edit;
    [SerializeField] private GameObject Top_Edit;
    [SerializeField] private GameObject Right_Edit;
    [SerializeField] private GameObject Bottom_Edit;
    #endregion

    #region Приватные поля
    private DataStore dataStore;
    #endregion

    #region Методы
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
    /// Устанавливает информацию о маршруте
    /// </summary>
    /// <param name="name">Имя маршрута</param>
    /// <param name="count">Кол-во точек</param>
    public void SetWayInfo(string name, int count)
    {
        WayNameText.GetComponent<Text>().text = name;
        WayPointCountText.GetComponent<Text>().text = count.ToString();
    }
    /// <summary>
    /// Устанавливаете информацию о карте
    /// </summary>
    /// <param name="name">Имя карты</param>
    /// <param name="graf_point">Rect географических точек</param>
    public void SetMapInfo(string name, Rect graf_point)
    {
        SetGrafValue(graf_point);
        MapNameText.GetComponent<Text>().text = name;
    }

    /// <summary>
    /// Получить географические координаты с UI
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
    /// Установить географические координаты в UI
    /// </summary>
    void SetGrafValue(float left, float top, float right, float bottom)
    {
        Left_Edit.GetComponent<InputField>().text       = left.ToString();
        Top_Edit.GetComponent<InputField>().text        = top.ToString();
        Right_Edit.GetComponent<InputField>().text      = right.ToString();
        Bottom_Edit.GetComponent<InputField>().text     = bottom.ToString();
    }
    /// <summary>
    /// Установить географические координаты в UI
    /// </summary>
    void SetGrafValue(Rect rect)
    {
        Left_Edit.GetComponent<InputField>().text       = rect.x.ToString();
        Top_Edit.GetComponent<InputField>().text        = rect.y.ToString();
        Right_Edit.GetComponent<InputField>().text      = rect.width.ToString();
        Bottom_Edit.GetComponent<InputField>().text     = rect.height.ToString();
    }
    #endregion

    #region Публичные методы
    /// <summary>
    /// Загрузить новую картинку и создать по ней новую карту
    /// </summary>
    public void LoadNewMap()
    {
        dataStore.CreateMap();
    }
    /// <summary>
    /// Сохранить текущую карту
    /// </summary>
    public void SaveMap()
    {
        dataStore.CurrentMap.RectMap = GetGrafValue();
        dataStore.SaveMap();
    }
    /// <summary>
    /// Сохранить текущий маршрут
    /// </summary>
    public void SaveWay()
    {
        dataStore.SaveWay();
    }
    /// <summary>
    /// Ихменить имя карты
    /// </summary>
    public void ChangeNameMap(string Name) 
    {
        dataStore.ChangeNameMap(Name);
    }
    /// <summary>
    /// Изменить имя маршрута
    /// </summary>
    public void ChangeNameWay(string Name)
    {
        dataStore.ChangeNameWay(Name);
    }
    /// <summary>
    /// Создать маршрут
    /// </summary>
    public void CreateWay()
    {
        dataStore.CreateWay();
    }
    /// <summary>
    /// Удалить текущую карту
    /// </summary>
    public void DeleteMap()
    {
        dataStore.DeleteCurrentMap();
    }
    /// <summary>
    /// Удалить текущий маршрут
    /// </summary>
    public void DeleteWay()
    {
        dataStore.DeleteCurrentWay();
    }
    #endregion

    #region Настройка адреса сервера
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
