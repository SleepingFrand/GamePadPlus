using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Реализует контроли над маршрутными точками
/// </summary>
public class PointControl : MonoBehaviour
{
    #region Поля
    [SerializeField] private DataStore dataStore;

    /// <summary>
    /// Список точек маршрута
    /// </summary>
    [SerializeField] private List<GameObject> Points;
    /// <summary>
    /// Префаб точки маршрута
    /// </summary>
    [SerializeField] private GameObject PointPrefab;
    /// <summary>
    /// Карта для растановки точек
    /// </summary>
    [SerializeField] private GameObject Map;

    /// <summary>
    /// Выбранная точка
    /// </summary>
    [SerializeField] private GameObject CurrentPoint;

    /// <summary>
    /// Позиция клика на карту
    /// </summary>
    [SerializeField] private Vector2 clickPosition = new Vector2(361, 361);
    #endregion

    #region Методы
    private void Start()
    {
        AtionsSystem.UpdateValueForDataStore += UpdateValue_For_DataStore;
        dataStore = FindObjectOfType<DataStore>();
    }

    void UpdateValue_For_DataStore()
    {
        CleanPoints();
        if (dataStore.CurrentWay.positionWayPoints.Count > 0)
        {
            PlaseWayPoints(dataStore.CurrentWay.positionWayPoints);
        }

    }
    /// <summary>
    /// Расталяет точки маршрута на карте
    /// </summary>
    /// <param name="pointsPositions">Список позиций точек</param>
    void PlaseWayPoints(List<Vector2> pointsPositions)
    {
        foreach (Vector2 pointPos in pointsPositions)
        {
            Points.Add((Instantiate(PointPrefab, new Vector2(pointPos.x, pointPos.y + PointPrefab.gameObject.GetComponent<RectTransform>().rect.height / 2),
                Quaternion.identity).transform.parent = Map.transform).GetChild(Map.transform.childCount - 1).gameObject);
        }
        DrawLineWay(pointsPositions);
    }
    /// <summary>
    /// Очищает точки маршрута с поля
    /// </summary>
    void CleanPoints()
    {
        this.gameObject.GetComponent<LineRenderer>().positionCount = 0;

        foreach (GameObject item in Points)
        {
            Destroy(item);
        }
        Points.Clear();
    }
    #endregion

    #region Публичные методы
    /// <summary>
    /// Метод обрабатывает нажатия на экран
    /// </summary>
    public void GetTouch()
    {
        PointerEventData m_PointerEventData = new PointerEventData(FindObjectOfType<EventSystem>());
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        GraphicRaycaster Graycast = FindObjectOfType<GraphicRaycaster>();

        Graycast.Raycast(m_PointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if(result.gameObject.tag == "Map")
            {
                clickPosition = result.worldPosition;
                CurrentPoint = null;
                break;
            }
            else if(result.gameObject.tag == "WayPoint")
            {
                CurrentPoint = result.gameObject;
                clickPosition = new Vector2(361, 361);
                break;
            }
        }
    }
    /// <summary>
    /// Рисует линию маршрута
    /// </summary>
    /// <param name="pointsPositions">Список позиций для рисования</param>
    void DrawLineWay(List<Vector2> pointsPositions)
    {
        this.gameObject.GetComponent<LineRenderer>().ResetBounds();

        List<Vector3> PosOnVect3 = new List<Vector3>();

        foreach (Vector2 item in pointsPositions)
        {
            PosOnVect3.Add(new Vector3(item.x, item.y, 0));
        }
        this.gameObject.GetComponent<LineRenderer>().positionCount = PosOnVect3.Count;
        this.gameObject.GetComponent<LineRenderer>().SetPositions(PosOnVect3.ToArray());
    }
    /// <summary>
    /// Метод создает точку маршрута
    /// </summary>
    public void CreatePoint()
    {
        if (clickPosition != new Vector2(361, 361)) 
        { 
            Points.Add((Instantiate(PointPrefab, new Vector2(clickPosition.x, clickPosition.y + PointPrefab.gameObject.GetComponent<RectTransform>().rect.height / 2),
                Quaternion.identity).transform.parent = Map.transform).GetChild(Map.transform.childCount - 1).gameObject);
            dataStore.CurrentWay.positionWayPoints.Add(clickPosition);
        }
        DrawLineWay(dataStore.CurrentWay.positionWayPoints);
        clickPosition = new Vector2(361, 361);

        AtionsSystem.UpdateValueOnSettings();
    }
    /// <summary>
    /// Метод удаляет точку маршрута
    /// </summary>
    public void DeletePoint()
    {
        if (CurrentPoint != null)
        {
            Vector2 item = CurrentPoint.transform.position;
            item.y -= PointPrefab.gameObject.GetComponent<RectTransform>().rect.height / 2;
            dataStore.CurrentWay.positionWayPoints.Remove(dataStore.CurrentWay.positionWayPoints.Find((Vector2 itemFind) => { return itemFind == item; }));
            Points.Remove(Points.Find((GameObject itemFind) => { return itemFind == CurrentPoint; }));
            Destroy(CurrentPoint);

            DrawLineWay(dataStore.CurrentWay.positionWayPoints);
            CurrentPoint = null;

            AtionsSystem.UpdateValueOnSettings();
        }
    }
    #endregion
}
