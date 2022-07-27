using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Реализует управление персонажем
/// </summary>
public class CharacterPos : MonoBehaviour
{
    #region Поля
    /// <summary>
    /// Карта (Игровой объект)
    /// </summary>
    [SerializeField] private RectTransform Map;
    /// <summary>
    /// Нарвавление пути
    /// </summary>
    [SerializeField] private float Direction;
    private float BaseDirection;
    /// <summary>
    /// Текущаа позиция в географических координатах
    /// </summary>
    [SerializeField] private Vector2 GeoPos;
    #endregion

    #region Приватные поля
    private DataStore dataStore;
    #endregion

    #region Методы
    private void Start()
    {
        dataStore = FindObjectOfType<DataStore>();
        AtionsSystem.UpdateValueOnCharacter += SetPosition;
        BaseDirection = this.GetComponent<RectTransform>().localEulerAngles.z;
    }

    /// <summary>
    /// Установить позицию персонажа в географических координатах
    /// </summary>
    /// <param name="position">Географическая координата</param>
    /// <param name="direction">Напраление пути</param>
    public void SetPosition()
    {
        GeoPos = DataStore.CharacterPosition;
        Direction = DataStore.CharacterDirection;
        Vector2 newpos = SetGeotransformToScreen();
        GetComponent<RectTransform>().localPosition = newpos;
        this.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, BaseDirection + Direction);

    }

    /// <summary>
    /// Перевод географических корординат в корординаты на экране
    /// </summary>
    Vector2 SetGeotransformToScreen()
    {

        Rect map_rect = dataStore.CurrentMap.RectMap;

        Rect Geo_RectMap = dataStore.CurrentMap.RectMap;

        Vector2 outVect = new Vector2();

        if (Geo_RectMap.x > Geo_RectMap.width)
        {
            float temp = Geo_RectMap.x;
            Geo_RectMap.x = Geo_RectMap.width;
            Geo_RectMap.width = temp;
        }
        if (Geo_RectMap.y > Geo_RectMap.height)
        {
            float temp = Geo_RectMap.y;
            Geo_RectMap.y = Geo_RectMap.height;
            Geo_RectMap.height = temp;
        }


        double correct = (GeoPos.x - Geo_RectMap.x) / (Geo_RectMap.width - Geo_RectMap.x);
        outVect.x = (float)(Map.rect.width * correct) + Map.localPosition.x;

        correct = (GeoPos.y - Geo_RectMap.y) / (Geo_RectMap.height - Geo_RectMap.y);
        outVect.y = (float)(Map.rect.height * correct) + Map.localPosition.y;

        return outVect;
    }
    #endregion
}
